using MareSynchronos.API.Data.Enum;

namespace MareSynchronosShared.Utils;

public readonly record struct ClientMessage(
    MessageSeverity Severity,
    string Message,
    string Title = "",
    int TimeMs = 5000,
    string UID = ""
);