using System.Collections.Concurrent;

namespace MareSynchronosAuthService.Services.Discovery;

public sealed class InMemoryPresenceStore : IDiscoveryPresenceStore
{
    private readonly ConcurrentDictionary<string, (string Uid, DateTimeOffset ExpiresAt, string? DisplayName, bool AllowRequests)> _presence = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, (string TargetUid, DateTimeOffset ExpiresAt)> _tokens = new(StringComparer.Ordinal);
    private readonly TimeSpan _presenceTtl;
    private readonly TimeSpan _tokenTtl;
    private readonly Timer _cleanupTimer;

    public InMemoryPresenceStore(TimeSpan presenceTtl, TimeSpan tokenTtl)
    {
        _presenceTtl = presenceTtl;
        _tokenTtl = tokenTtl;
        _cleanupTimer = new Timer(_ => Cleanup(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public void Dispose()
    {
        _cleanupTimer.Dispose();
    }

    private void Cleanup()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var kv in _presence.ToArray())
        {
            if (kv.Value.ExpiresAt <= now) _presence.TryRemove(kv.Key, out _);
        }
        foreach (var kv in _tokens.ToArray())
        {
            if (kv.Value.ExpiresAt <= now) _tokens.TryRemove(kv.Key, out _);
        }
    }

    public void Publish(string uid, IEnumerable<string> hashes, string? displayName = null, bool allowRequests = true)
    {
        var exp = DateTimeOffset.UtcNow.Add(_presenceTtl);
        foreach (var h in hashes.Distinct(StringComparer.Ordinal))
        {
            _presence[h] = (uid, exp, displayName, allowRequests);
        }
    }

    public void Unpublish(string uid)
    {
        // Remove all presence hashes owned by this uid
        foreach (var kv in _presence.ToArray())
        {
            if (string.Equals(kv.Value.Uid, uid, StringComparison.Ordinal))
            {
                _presence.TryRemove(kv.Key, out _);
            }
        }
    }

public (bool Found, string? Token, string TargetUid, string? DisplayName) TryMatchAndIssueToken(string requesterUid, string hash)
{
    if (_presence.TryGetValue(hash, out var entry))
    {
        // Refresh TTL for this presence whenever it is matched (regardless of AllowRequests)
        var refreshed = (entry.Uid, DateTimeOffset.UtcNow.Add(_presenceTtl), entry.DisplayName, entry.AllowRequests);
        _presence[hash] = refreshed;

        if (string.Equals(entry.Uid, requesterUid, StringComparison.Ordinal))
            return (false, null, string.Empty, null);

        // Visible but requests disabled → no token
        if (!entry.AllowRequests)
            return (true, null, entry.Uid, entry.DisplayName);

        var token = Guid.NewGuid().ToString("N");
        _tokens[token] = (entry.Uid, DateTimeOffset.UtcNow.Add(_tokenTtl));
        return (true, token, entry.Uid, entry.DisplayName);
    }
    return (false, null, string.Empty, null);
}

    public bool ValidateToken(string token, out string targetUid)
    {
        targetUid = string.Empty;
        if (_tokens.TryGetValue(token, out var info))
        {
            if (info.ExpiresAt > DateTimeOffset.UtcNow)
            {
                targetUid = info.TargetUid;

                // Optional robustness: refresh TTL for all presence entries of this target
                var newExp = DateTimeOffset.UtcNow.Add(_presenceTtl);
                foreach (var kv in _presence.ToArray())
                {
                    if (string.Equals(kv.Value.Uid, targetUid, StringComparison.Ordinal))
                    {
                        var v = kv.Value;
                        _presence[kv.Key] = (v.Uid, newExp, v.DisplayName, v.AllowRequests);
                    }
                }

                return true;
            }
            _tokens.TryRemove(token, out _);
        }
        return false;
    }
}

