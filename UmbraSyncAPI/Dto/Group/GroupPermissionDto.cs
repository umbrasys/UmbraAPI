using UmbraSync.API.Data;
using UmbraSync.API.Data.Enum;
using MessagePack;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupPermissionDto(GroupData Group, GroupPermissions Permissions) : GroupDto(Group);
