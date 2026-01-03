using MessagePack;
using UmbraSync.API.Data;
using UmbraSync.API.Data.Enum;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record TypingStateDto(UserData User, bool IsTyping, TypingScope Scope)
{
    public UserData User { get; set; } = User;
    public bool IsTyping { get; set; } = IsTyping;
    public TypingScope Scope { get; set; } = Scope;
}