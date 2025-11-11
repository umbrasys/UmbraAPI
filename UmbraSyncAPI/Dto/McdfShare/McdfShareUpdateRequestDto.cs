using System;
using System.Collections.Generic;
using MessagePack;

namespace UmbraSync.API.Dto.McdfShare;

[MessagePackObject]
public class McdfShareUpdateRequestDto
{
    [Key(0)] public Guid ShareId { get; set; }
    [Key(1)] public string Description { get; set; } = string.Empty;
    [Key(2)] public DateTime? ExpiresAtUtc { get; set; }
    [Key(3)] public List<string> AllowedIndividuals { get; set; } = [];
    [Key(4)] public List<string> AllowedSyncshells { get; set; } = [];
}
