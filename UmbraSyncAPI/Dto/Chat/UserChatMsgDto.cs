using UmbraSync.API.Data;
using UmbraSync.API.Dto.User;
using MessagePack;

namespace UmbraSync.API.Dto.Chat;

[MessagePackObject(keyAsPropertyName: true)]
public record UserChatMsgDto(SignedChatMessage Message)
{
    public SignedChatMessage Message = Message;
}