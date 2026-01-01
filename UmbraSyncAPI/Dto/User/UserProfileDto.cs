using UmbraSync.API.Data;
using MessagePack;

namespace UmbraSync.API.Dto.User;

[MessagePackObject(keyAsPropertyName: true)]
public record UserProfileDto(UserData User, bool Disabled, bool? IsNSFW, string? ProfilePictureBase64, string? Description) : UserDto(User)
{
    public string? RpProfilePictureBase64 { get; set; }
    public string? RpDescription { get; set; }
    public bool? IsRpNSFW { get; set; }
    public string? RpFirstName { get; set; }
    public string? RpLastName { get; set; }
    public string? RpTitle { get; set; }
    public string? RpAge { get; set; }
    public string? RpHeight { get; set; }
    public string? RpBuild { get; set; }
    public string? RpOccupation { get; set; }
    public string? RpAffiliation { get; set; }
    public string? RpAlignment { get; set; }
    public string? RpAdditionalInfo { get; set; }
}
public record UserProfileDto(UserData User, bool Disabled, bool? IsNSFW, string? ProfilePictureBase64, string? Description, 
    string? RpProfilePictureBase64 = null, string? RpDescription = null, bool? IsRpNSFW = null,
    string? RpFirstName = null, string? RpLastName = null, string? RpTitle = null, string? RpAge = null,
    string? RpHeight = null, string? RpBuild = null, string? RpOccupation = null, string? RpAffiliation = null,
    string? RpAlignment = null, string? RpAdditionalInfo = null,
    string? CharacterName = null, uint? WorldId = null) : UserDto(User, CharacterName, WorldId);
