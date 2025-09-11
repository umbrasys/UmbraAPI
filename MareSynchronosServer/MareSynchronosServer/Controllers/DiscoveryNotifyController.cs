using MareSynchronos.API.SignalR;
using MareSynchronosShared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace MareSynchronosServer.Controllers;

[Route("/main/discovery")] 
[Authorize(Policy = "Internal")]
public class DiscoveryNotifyController : Controller
{
    private readonly ILogger<DiscoveryNotifyController> _logger;
    private readonly IHubContext<Hubs.MareHub, IMareHub> _hub;

    public DiscoveryNotifyController(ILogger<DiscoveryNotifyController> logger, IHubContext<Hubs.MareHub, IMareHub> hub)
    {
        _logger = logger;
        _hub = hub;
    }

    public sealed class NotifyRequestDto
    {
        [JsonPropertyName("targetUid")] public string TargetUid { get; set; } = string.Empty;
        [JsonPropertyName("fromUid")] public string FromUid { get; set; } = string.Empty;
        [JsonPropertyName("fromAlias")] public string? FromAlias { get; set; }
    }

    [HttpPost("notifyRequest")]
    public async Task<IActionResult> NotifyRequest([FromBody] NotifyRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.TargetUid)) return BadRequest();
        var name = string.IsNullOrEmpty(dto.FromAlias) ? dto.FromUid : dto.FromAlias;
        var msg = $"{name} wants to pair with you (Nearby)";
        _logger.LogInformation("Discovery notify request to {target} from {from}", dto.TargetUid, name);
        await _hub.Clients.User(dto.TargetUid).Client_ReceiveServerMessage(MareSynchronos.API.Data.Enum.MessageSeverity.Information, msg);
        return Accepted();
    }
}
