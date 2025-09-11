using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MareSynchronosAuthService.Services;

public class DiscoveryPresenceService : IHostedService
{
    private readonly ILogger<DiscoveryPresenceService> _logger;

    // hash -> (uid, expiresAt, displayName?)
    private readonly ConcurrentDictionary<string, (string Uid, DateTimeOffset ExpiresAt, string? DisplayName)> _presence = new(StringComparer.Ordinal);

    // token -> (targetUid, expiresAt)
    private readonly ConcurrentDictionary<string, (string TargetUid, DateTimeOffset ExpiresAt)> _tokens = new(StringComparer.Ordinal);

    private Timer? _cleanupTimer;
    private readonly TimeSpan _presenceTtl = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _tokenTtl = TimeSpan.FromMinutes(2);

    public DiscoveryPresenceService(ILogger<DiscoveryPresenceService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cleanupTimer = new Timer(_ => Cleanup(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cleanupTimer?.Dispose();
        return Task.CompletedTask;
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

    public void Publish(string uid, IEnumerable<string> hashes, string? displayName = null)
    {
        var exp = DateTimeOffset.UtcNow.Add(_presenceTtl);
        foreach (var h in hashes.Distinct(StringComparer.Ordinal))
        {
            _presence[h] = (uid, exp, displayName);
        }
        _logger.LogDebug("Discovery presence published for {uid} with {n} hashes", uid, hashes.Count());
    }

    public (bool Found, string Token, string? DisplayName) TryMatchAndIssueToken(string requesterUid, string hash)
    {
        if (_presence.TryGetValue(hash, out var entry))
        {
            if (string.Equals(entry.Uid, requesterUid, StringComparison.Ordinal)) return (false, string.Empty, null);
            var token = Guid.NewGuid().ToString("N");
            _tokens[token] = (entry.Uid, DateTimeOffset.UtcNow.Add(_tokenTtl));
            return (true, token, entry.DisplayName);
        }
        return (false, string.Empty, null);
    }

    public bool ValidateToken(string token, out string targetUid)
    {
        targetUid = string.Empty;
        if (_tokens.TryGetValue(token, out var info))
        {
            if (info.ExpiresAt > DateTimeOffset.UtcNow)
            {
                targetUid = info.TargetUid;
                return true;
            }
            _tokens.TryRemove(token, out _);
        }
        return false;
    }
}

