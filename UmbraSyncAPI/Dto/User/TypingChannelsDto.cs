using MessagePack;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public class TypingChannelsDto
{
    public string? PartyId { get; set; }
    public string? AllianceId { get; set; }
    public string? FreeCompanyId { get; set; }
    public string[]? CrossPartyIds { get; set; }
    public string[]? CustomGroupIds { get; set; }
    public bool ProximityEnabled { get; set; } = true;
}