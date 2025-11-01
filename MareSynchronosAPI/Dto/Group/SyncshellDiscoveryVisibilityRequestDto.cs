using MessagePack;

namespace MareSynchronos.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public sealed record SyncshellDiscoveryVisibilityRequestDto
{
    public string GID { get; init; } = string.Empty;
    public bool AutoDetectVisible { get; init; }
    
    public int? DisplayDurationHours { get; init; }
    public int[]? ActiveWeekdays { get; init; }
    public string? TimeStartLocal { get; init; }
    public string? TimeEndLocal { get; init; }
    public string? TimeZone { get; init; }
}
