using System;
using System.Collections.Generic;

namespace MareSynchronos.API.Dto.McdfShare;

public class McdfShareUploadRequestDto
{
    public Guid ShareId { get; set; }
    public string Description { get; set; } = string.Empty;
    public byte[] CipherData { get; set; } = Array.Empty<byte>();
    public byte[] Nonce { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public byte[] Tag { get; set; } = Array.Empty<byte>();
    public DateTime? ExpiresAtUtc { get; set; }
    public List<string> AllowedIndividuals { get; set; } = [];
    public List<string> AllowedSyncshells { get; set; } = [];
}
