using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MareSynchronosAuthService.Services;

public class DiscoveryWellKnownProvider : IHostedService
{
    private readonly ILogger<DiscoveryWellKnownProvider> _logger;
    private readonly object _lock = new();
    private byte[] _currentSalt = Array.Empty<byte>();
    private DateTimeOffset _currentSaltExpiresAt;
    private Timer? _rotationTimer;
    private readonly TimeSpan _saltTtl = TimeSpan.FromDays(30 * 6); 
    private readonly int _refreshSec = 86400; // 24h

    public DiscoveryWellKnownProvider(ILogger<DiscoveryWellKnownProvider> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RotateSalt();
        var period = _saltTtl;
        if (period.TotalMilliseconds > uint.MaxValue - 1)
        {
            _logger.LogInformation("DiscoveryWellKnownProvider: salt TTL {ttl} exceeds timer limit, skipping rotation timer in beta", period);
            _rotationTimer = new Timer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
        else
        {
            _rotationTimer = new Timer(_ => RotateSalt(), null, period, period);
        }
        _logger.LogInformation("DiscoveryWellKnownProvider started. Salt expires at {exp}", _currentSaltExpiresAt);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rotationTimer?.Dispose();
        return Task.CompletedTask;
    }

    private void RotateSalt()
    {
        lock (_lock)
        {
            _currentSalt = RandomNumberGenerator.GetBytes(32);
            _currentSaltExpiresAt = DateTimeOffset.UtcNow.Add(_saltTtl);
        }
    }

    public bool IsExpired()
    {
        lock (_lock)
        {
            return DateTimeOffset.UtcNow > _currentSaltExpiresAt;
        }
    }

    public string GetWellKnownJson(string scheme, string host)
    {
        var isHttps = string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase);
        var wsScheme = isHttps ? "wss" : "ws";
        var httpScheme = isHttps ? "https" : "http";

        byte[] salt;
        DateTimeOffset exp;
        lock (_lock)
        {
            salt = _currentSalt.ToArray();
            exp = _currentSaltExpiresAt;
        }

        var root = new WellKnownRoot
        {
            ApiUrl = $"{wsScheme}://{host}",
            HubUrl = $"{wsScheme}://{host}/mare",
            Features = new() { NearbyDiscovery = true },
            NearbyDiscovery = new()
            {
                Enabled = true,
                HashAlgo = "sha256",
                SaltB64 = Convert.ToBase64String(salt),
                SaltExpiresAt = exp,
                RefreshSec = _refreshSec,
                Endpoints = new()
                {
                    Publish = $"{httpScheme}://{host}/discovery/publish",
                    Query = $"{httpScheme}://{host}/discovery/query",
                    Request = $"{httpScheme}://{host}/discovery/request"
                },
                Policies = new()
                {
                    MaxQueryBatch = 100,
                    MinQueryIntervalMs = 2000,
                    RateLimitPerMin = 30,
                    TokenTtlSec = 120
                }
            }
        };

        return JsonSerializer.Serialize(root, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    }

    private sealed class WellKnownRoot
    {
        [JsonPropertyName("api_url")] public string ApiUrl { get; set; } = string.Empty;
        [JsonPropertyName("hub_url")] public string HubUrl { get; set; } = string.Empty;
        [JsonPropertyName("skip_negotiation")] public bool SkipNegotiation { get; set; } = true;
        [JsonPropertyName("transports")] public string[] Transports { get; set; } = new[] { "websockets" };
        [JsonPropertyName("features")] public Features Features { get; set; } = new();
        [JsonPropertyName("nearby_discovery")] public Nearby NearbyDiscovery { get; set; } = new();
    }

    private sealed class Features
    {
        [JsonPropertyName("nearby_discovery")] public bool NearbyDiscovery { get; set; }
    }

    private sealed class Nearby
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; }
        [JsonPropertyName("hash_algo")] public string HashAlgo { get; set; } = "sha256";
        [JsonPropertyName("salt_b64")] public string SaltB64 { get; set; } = string.Empty;
        [JsonPropertyName("salt_expires_at")] public DateTimeOffset SaltExpiresAt { get; set; }
        [JsonPropertyName("refresh_sec")] public int RefreshSec { get; set; }
        [JsonPropertyName("endpoints")] public Endpoints Endpoints { get; set; } = new();
        [JsonPropertyName("policies")] public Policies Policies { get; set; } = new();
    }

    private sealed class Endpoints
    {
        [JsonPropertyName("publish")] public string? Publish { get; set; }
        [JsonPropertyName("query")] public string? Query { get; set; }
        [JsonPropertyName("request")] public string? Request { get; set; }
    }

    private sealed class Policies
    {
        [JsonPropertyName("max_query_batch")] public int MaxQueryBatch { get; set; }
        [JsonPropertyName("min_query_interval_ms")] public int MinQueryIntervalMs { get; set; }
        [JsonPropertyName("rate_limit_per_min")] public int RateLimitPerMin { get; set; }
        [JsonPropertyName("token_ttl_sec")] public int TokenTtlSec { get; set; }
    }
}
