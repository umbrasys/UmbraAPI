using System.Text.Json.Serialization;
using UmbraSync.API.Data;
using UmbraSync.API.Dto.Group;
using MessagePack;

namespace UmbraSync.API.Dto.Chat;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupChatMsgDto
{
    [JsonConstructor]
    public GroupChatMsgDto(GroupDto group, SignedChatMessage message)
    {
        Group = group;
        Message = message;
    }

    [Key(nameof(Group))]
    public GroupDto Group { get; init; }

    [Key(nameof(Message))]
    public SignedChatMessage Message { get; init; }
}
