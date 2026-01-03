using MessagePack;

namespace UmbraSync.API.Dto.Slot;

[MessagePackObject(keyAsPropertyName: true)]
public record SlotSyncshellDto
{
    public string Gid { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}