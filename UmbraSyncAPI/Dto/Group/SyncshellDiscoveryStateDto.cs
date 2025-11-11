using MessagePack;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public sealed record SyncshellDiscoveryStateDto
{
    public string GID { get; init; } = string.Empty;
    public bool AutoDetectVisible { get; init; }
    public bool PasswordTemporarilyDisabled { get; init; }
}
