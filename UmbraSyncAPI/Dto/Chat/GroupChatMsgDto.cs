using UmbraSync.API.Data;
using UmbraSync.API.Dto.Group;
using UmbraSync.API.Dto.User;
using MessagePack;

namespace UmbraSync.API.Dto.Chat;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupChatMsgDto(GroupDto Group, SignedChatMessage Message)
{
    public GroupDto Group = Group;
    public SignedChatMessage Message = Message;
}