using MessagePack;

namespace UmbraSync.API.Dto;

[MessagePackObject(keyAsPropertyName: true)]
public record SystemInfoDto
{
    public int OnlineUsers { get; set; }
    public bool SupportsTypingState { get; set; }
}
