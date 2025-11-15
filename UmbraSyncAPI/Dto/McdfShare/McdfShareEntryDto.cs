using System;
using System.Collections.Generic;
using MessagePack;

namespace UmbraSync.API.Dto.McdfShare;

[MessagePackObject]
public class McdfShareEntryDto
{
    [Key(0)] public Guid Id { get; set; }
    [Key(1)] public string Description { get; set; } = string.Empty;
    [Key(2)] public DateTime CreatedUtc { get; set; }
    [Key(3)] public DateTime? UpdatedUtc { get; set; }
    [Key(4)] public DateTime? ExpiresAtUtc { get; set; }
    [Key(5)] public bool IsOwner { get; set; }
    [Key(6)] public string OwnerUid { get; set; } = string.Empty;
    [Key(7)] public string OwnerAlias { get; set; } = string.Empty;
    [Key(8)] public int DownloadCount { get; set; }
    [Key(9)] public IReadOnlyList<string> AllowedIndividuals { get; set; } = Array.Empty<string>();
    [Key(10)] public IReadOnlyList<string> AllowedSyncshells { get; set; } = Array.Empty<string>();
}
