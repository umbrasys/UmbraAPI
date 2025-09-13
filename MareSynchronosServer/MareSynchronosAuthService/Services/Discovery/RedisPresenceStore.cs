using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace MareSynchronosAuthService.Services.Discovery;

public sealed class RedisPresenceStore : IDiscoveryPresenceStore
{
    private readonly ILogger<RedisPresenceStore> _logger;
    private readonly IDatabase _db;
    private readonly TimeSpan _presenceTtl;
    private readonly TimeSpan _tokenTtl;
    private readonly JsonSerializerOptions _jsonOpts = new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };

    public RedisPresenceStore(ILogger<RedisPresenceStore> logger, IConnectionMultiplexer mux, TimeSpan presenceTtl, TimeSpan tokenTtl)
    {
        _logger = logger;
        _db = mux.GetDatabase();
        _presenceTtl = presenceTtl;
        _tokenTtl = tokenTtl;
    }

    public void Dispose() { }

    private static string KeyForHash(string hash) => $"nd:hash:{hash}";
    private static string KeyForToken(string token) => $"nd:token:{token}";
    private static string KeyForUidSet(string uid) => $"nd:uid:{uid}";

    public void Publish(string uid, IEnumerable<string> hashes, string? displayName = null, bool allowRequests = true)
    {
        var entries = hashes.Distinct(StringComparer.Ordinal).ToArray();
        if (entries.Length == 0) return;
        var batch = _db.CreateBatch();
        foreach (var h in entries)
        {
            var key = KeyForHash(h);
            var payload = JsonSerializer.Serialize(new Presence(uid, displayName, allowRequests), _jsonOpts);
            batch.StringSetAsync(key, payload, _presenceTtl);
            // Index this hash under the publisher uid for fast unpublish
            batch.SetAddAsync(KeyForUidSet(uid), h);
            batch.KeyExpireAsync(KeyForUidSet(uid), _presenceTtl);
        }
        batch.Execute();
        _logger.LogDebug("RedisPresenceStore: published {count} hashes", entries.Length);
    }

    public void Unpublish(string uid)
    {
        try
        {
            var setKey = KeyForUidSet(uid);
            var members = _db.SetMembers(setKey);
            if (members is { Length: > 0 })
            {
                var batch = _db.CreateBatch();
                foreach (var m in members)
                {
                    var hash = (string)m;
                    var key = KeyForHash(hash);
                    // Defensive: only delete if the hash is still owned by this uid
                    var val = _db.StringGet(key);
                    if (val.HasValue)
                    {
                        try
                        {
                            var p = JsonSerializer.Deserialize<Presence>(val!);
                            if (p != null && string.Equals(p.Uid, uid, StringComparison.Ordinal))
                            {
                                batch.KeyDeleteAsync(key);
                            }
                        }
                        catch { /* ignore corrupted */ }
                    }
                }
                // Remove the uid index set itself
                batch.KeyDeleteAsync(setKey);
                batch.Execute();
            }
            else
            {
                // No index set: best-effort, just delete the set key in case it exists
                _db.KeyDelete(setKey);
            }
            _logger.LogDebug("RedisPresenceStore: unpublished all hashes for uid {uid}", uid);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RedisPresenceStore: Unpublish failed for uid {uid}", uid);
        }
    }

    public (bool Found, string? Token, string TargetUid, string? DisplayName) TryMatchAndIssueToken(string requesterUid, string hash)
    {
        var key = KeyForHash(hash);
        var val = _db.StringGet(key);
        if (!val.HasValue) return (false, null, string.Empty, null);
        try
        {
            var p = JsonSerializer.Deserialize<Presence>(val!);
            if (p == null || string.IsNullOrEmpty(p.Uid)) return (false, null, string.Empty, null);
            if (string.Equals(p.Uid, requesterUid, StringComparison.Ordinal)) return (false, null, string.Empty, null);

            // Refresh TTLs for this presence whenever it is matched
            _db.KeyExpire(KeyForHash(hash), _presenceTtl);
            _db.KeyExpire(KeyForUidSet(p.Uid), _presenceTtl);

            // Visible but requests disabled → return without token
            if (!p.AllowRequests)
            {
                return (true, null, p.Uid, p.DisplayName);
            }

            var token = Guid.NewGuid().ToString("N");
            _db.StringSet(KeyForToken(token), p.Uid, _tokenTtl);
            return (true, token, p.Uid, p.DisplayName);
        }
        catch
        {
            return (false, null, string.Empty, null);
        }
    }

    public bool ValidateToken(string token, out string targetUid)
    {
        targetUid = string.Empty;
        var key = KeyForToken(token);
        var val = _db.StringGet(key);
        if (!val.HasValue) return false;
        targetUid = val!;
        try
        {
            var setKey = KeyForUidSet(targetUid);
            var members = _db.SetMembers(setKey);
            if (members is { Length: > 0 })
            {
                var batch = _db.CreateBatch();
                foreach (var m in members)
                {
                    var h = (string)m;
                    batch.KeyExpireAsync(KeyForHash(h), _presenceTtl);
                }
                batch.KeyExpireAsync(setKey, _presenceTtl);
                batch.Execute();
            }
            else
            {
                // Still try to extend the set TTL even if empty
                _db.KeyExpire(setKey, _presenceTtl);
            }
        }
        catch { /* ignore TTL refresh issues */ }
        return true;
    }

    private sealed record Presence(string Uid, string? DisplayName, bool AllowRequests);
}
