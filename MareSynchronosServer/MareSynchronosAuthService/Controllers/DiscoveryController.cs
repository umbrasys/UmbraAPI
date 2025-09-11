using System.Text.Json.Serialization;
using MareSynchronosAuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MareSynchronosAuthService.Controllers;

[Authorize]
[ApiController]
[Route("discovery")]
public class DiscoveryController : Controller
{
    private readonly DiscoveryWellKnownProvider _provider;
    private readonly DiscoveryPresenceService _presence;

    public DiscoveryController(DiscoveryWellKnownProvider provider, DiscoveryPresenceService presence)
    {
        _provider = provider;
        _presence = presence;
    }

    public sealed class QueryRequest
    {
        [JsonPropertyName("hashes")] public string[] Hashes { get; set; } = Array.Empty<string>();
    }

    public sealed class QueryResponseEntry
    {
        [JsonPropertyName("hash")] public string Hash { get; set; } = string.Empty;
        [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
        [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
    }

    [HttpPost("query")]
    public IActionResult Query([FromBody] QueryRequest req)
    {
        if (_provider.IsExpired())
        {
            return BadRequest(new { code = "DISCOVERY_SALT_EXPIRED" });
        }

        var uid = User?.Claims?.FirstOrDefault(c => c.Type == MareSynchronosShared.Utils.MareClaimTypes.Uid)?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(uid) || req?.Hashes == null || req.Hashes.Length == 0)
            return Json(Array.Empty<QueryResponseEntry>());

        List<QueryResponseEntry> matches = new();
        foreach (var h in req.Hashes.Distinct(StringComparer.Ordinal))
        {
            var (found, token, displayName) = _presence.TryMatchAndIssueToken(uid, h);
            if (found)
            {
                matches.Add(new QueryResponseEntry { Hash = h, Token = token, DisplayName = displayName });
            }
        }

        return Json(matches);
    }

    public sealed class RequestDto
    {
        [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestPair([FromBody] RequestDto req)
    {
        if (string.IsNullOrEmpty(req.Token)) return BadRequest();
        if (_presence.ValidateToken(req.Token, out var targetUid))
        {
            // Phase 3 (minimal): notify target via mare-server internal controller
            try
            {
                var fromUid = User?.Claims?.FirstOrDefault(c => c.Type == MareSynchronosShared.Utils.MareClaimTypes.Uid)?.Value ?? string.Empty;
                var fromAlias = User?.Claims?.FirstOrDefault(c => c.Type == MareSynchronosShared.Utils.MareClaimTypes.Alias)?.Value ?? string.Empty;

                using var http = new HttpClient();
                // Use same host as public (goes through nginx)
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var url = new Uri(new Uri(baseUrl), "/main/discovery/notifyRequest");

                // Generate internal JWT
                var serverToken = HttpContext.RequestServices.GetRequiredService<MareSynchronosShared.Utils.ServerTokenGenerator>().Token;
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serverToken);
                var payload = System.Text.Json.JsonSerializer.Serialize(new { targetUid, fromUid, fromAlias });
                var resp = await http.PostAsync(url, new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
                // ignore response content, just return Accepted to caller
            }
            catch { /* ignore */ }

            return Accepted();
        }
        return BadRequest(new { code = "INVALID_TOKEN" });
    }

    public sealed class PublishRequest
    {
        [JsonPropertyName("hashes")] public string[] Hashes { get; set; } = Array.Empty<string>();
        [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
    }

    [HttpPost("publish")]
    public IActionResult Publish([FromBody] PublishRequest req)
    {
        if (_provider.IsExpired())
        {
            return BadRequest(new { code = "DISCOVERY_SALT_EXPIRED" });
        }
        var uid = User?.Claims?.FirstOrDefault(c => c.Type == MareSynchronosShared.Utils.MareClaimTypes.Uid)?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(uid) || req?.Hashes == null || req.Hashes.Length == 0)
            return Accepted();

        _presence.Publish(uid, req.Hashes, req.DisplayName);
        return Accepted();
    }
}
