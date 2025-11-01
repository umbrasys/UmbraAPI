using System;
using MareSynchronos.API.Data;
using MareSynchronos.API.Data.Enum;
using MessagePack;

namespace MareSynchronos.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupInfoDto(GroupData Group, UserData Owner, GroupPermissions GroupPermissions) : GroupDto(Group)
{
    public GroupPermissions GroupPermissions { get; set; } = GroupPermissions;
    public UserData Owner { get; set; } = Owner;
    public bool AutoDetectVisible { get; set; }
    public bool PasswordTemporarilyDisabled { get; set; }

    public string OwnerUID => Owner.UID;
    public string? OwnerAlias => Owner.Alias;
    public string OwnerAliasOrUID => Owner.AliasOrUID;

    public bool IsTemporary { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
