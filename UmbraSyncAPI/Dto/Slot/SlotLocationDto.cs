using MessagePack;

namespace UmbraSync.API.Dto.Slot;

[MessagePackObject(keyAsPropertyName: true)]
public record SlotLocationDto
{
    public uint ServerId { get; init; }
    public uint TerritoryId { get; init; }
    public uint WardId { get; init; }
    public uint PlotId { get; init; }
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public float Radius { get; init; } = 10f;
}
