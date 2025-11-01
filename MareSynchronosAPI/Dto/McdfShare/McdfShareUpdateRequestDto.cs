using System;
using System.Collections.Generic;

namespace MareSynchronos.API.Dto.McdfShare;

public class McdfShareUpdateRequestDto
{
    public Guid ShareId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? ExpiresAtUtc { get; set; }
    public List<string> AllowedIndividuals { get; set; } = [];
    public List<string> AllowedSyncshells { get; set; } = [];
}
