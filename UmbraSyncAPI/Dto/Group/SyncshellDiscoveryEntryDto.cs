using MessagePack;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public sealed record SyncshellDiscoveryEntryDto
{
    public string GID { get; init; } = string.Empty;
    public string? Alias { get; init; }
    public string OwnerUID { get; init; } = string.Empty;
    public string? OwnerAlias { get; init; }
    public int MemberCount { get; init; }
    public bool AutoAcceptPairs { get; init; }
    public string? Description { get; init; }
    public int MaxUserCount { get; init; }
}
