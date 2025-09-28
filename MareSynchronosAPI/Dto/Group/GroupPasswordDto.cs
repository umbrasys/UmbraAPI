using System;
using MareSynchronos.API.Data;
using MessagePack;

namespace MareSynchronos.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupPasswordDto(GroupData Group, string Password) : GroupDto(Group)
{
    public bool IsTemporary { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
