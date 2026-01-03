using MessagePack;
using UmbraSync.API.Data;
using UmbraSync.API.Data.Enum;

namespace UmbraSync.API.Dto.Group;

[MessagePackObject(keyAsPropertyName: true)]
public record GroupPairUserPermissionDto(GroupData Group, UserData User, GroupUserPermissions GroupPairPermissions) : GroupPairDto(Group, User);