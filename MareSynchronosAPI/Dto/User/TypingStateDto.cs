using MareSynchronos.API.Data;
using MessagePack;

namespace MareSynchronos.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record TypingStateDto(UserData User, bool IsTyping)
{
    public UserData User { get; set; } = User;
    public bool IsTyping { get; set; } = IsTyping;
}
