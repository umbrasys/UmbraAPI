using MareSynchronos.API.Data;

namespace MareSynchronos.API.Dto.User;

public record TypingStateDto(UserData User, bool IsTyping);
