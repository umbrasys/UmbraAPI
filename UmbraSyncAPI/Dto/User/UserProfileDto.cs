using UmbraSync.API.Data;
using MessagePack;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record UserProfileDto(UserData User, bool Disabled, bool? IsNSFW, string? ProfilePictureBase64, string? Description, 
    string? RpProfilePictureBase64 = null, string? RpDescription = null, bool? IsRpNSFW = null,
    string? RpFirstName = null, string? RpLastName = null, string? RpTitle = null, string? RpAge = null,
    string? RpHeight = null, string? RpBuild = null, string? RpOccupation = null, string? RpAffiliation = null,
    string? RpAlignment = null, string? RpAdditionalInfo = null,
    string? CharacterName = null, uint? WorldId = null) : UserDto(User, CharacterName, WorldId);