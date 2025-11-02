using MareSynchronos.API.Data;
using MareSynchronos.API.Data.Enum;
using MessagePack;

namespace MareSynchronos.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record TypingStateDto(UserData User, bool IsTyping, TypingScope Scope)
{
    public UserData User { get; set; } = User;
    public bool IsTyping { get; set; } = IsTyping;
    public TypingScope Scope { get; set; } = Scope;
}
