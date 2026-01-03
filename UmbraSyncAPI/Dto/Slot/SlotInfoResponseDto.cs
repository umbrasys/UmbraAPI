using MessagePack;

namespace UmbraSync.API.Dto.Slot;

[MessagePackObject(keyAsPropertyName: true)]
public record SlotInfoResponseDto
{
    public Guid SlotId { get; init; }
    public string SlotName { get; init; } = string.Empty;
    public string? SlotDescription { get; init; }
    public SlotLocationDto? Location { get; init; }
    public SlotSyncshellDto? AssociatedSyncshell { get; init; }
}