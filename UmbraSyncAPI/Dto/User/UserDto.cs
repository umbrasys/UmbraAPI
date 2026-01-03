using MessagePack;
using UmbraSync.API.Data;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record UserDto(UserData User, string? CharacterName = null, uint? WorldId = null);