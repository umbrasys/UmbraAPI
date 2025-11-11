using System;
using UmbraSync.API.Data;
using MessagePack;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupPasswordDto(GroupData Group, string Password) : GroupDto(Group)
{
    public bool IsTemporary { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
