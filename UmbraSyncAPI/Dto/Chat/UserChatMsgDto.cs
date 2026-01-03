using MessagePack;
using UmbraSync.API.Data;

namespace UmbraSync.API.Dto.Chat;

[MessagePackObject(keyAsPropertyName: true)]
public record UserChatMsgDto(SignedChatMessage Message)
{
    public SignedChatMessage Message = Message;
}