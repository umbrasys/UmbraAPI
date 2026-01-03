using MessagePack;
using UmbraSync.API.Data.Enum;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public class TypingStateExDto
{
    public bool IsTyping { get; set; }
    public TypingScope Scope { get; set; }
    public string? TargetUid { get; set; }
    public string? ChannelId { get; set; }
}