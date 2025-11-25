using MessagePack;
using UmbraSync.API.Data.Enum;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public sealed record SyncshellDiscoverySetPolicyRequestDto
{
    public string GID { get; init; } = string.Empty;
    public AutoDetectMode Mode { get; init; }

    public int? DisplayDurationHours { get; init; }
    public int[]? ActiveWeekdays { get; init; }
    public string? TimeStartLocal { get; init; }
    public string? TimeEndLocal { get; init; }
    public string? TimeZone { get; init; }
}
