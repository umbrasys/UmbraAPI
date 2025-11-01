using System;
using System.Collections.Generic;

namespace MareSynchronos.API.Dto.McdfShare;

public class McdfShareEntryDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public bool IsOwner { get; set; }
    public string OwnerUid { get; set; } = string.Empty;
    public string OwnerAlias { get; set; } = string.Empty;
    public int DownloadCount { get; set; }
    public List<string> AllowedIndividuals { get; set; } = [];
    public List<string> AllowedSyncshells { get; set; } = [];
}
