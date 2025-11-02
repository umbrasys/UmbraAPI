using System;
using System.Collections.Generic;
using MessagePack;

namespace MareSynchronos.API.Dto.McdfShare;

[MessagePackObject]
public class McdfShareUploadRequestDto
{
    [Key(0)] public Guid ShareId { get; set; }
    [Key(1)] public string Description { get; set; } = string.Empty;
    [Key(2)] public byte[] CipherData { get; set; } = Array.Empty<byte>();
    [Key(3)] public byte[] Nonce { get; set; } = Array.Empty<byte>();
    [Key(4)] public byte[] Salt { get; set; } = Array.Empty<byte>();
    [Key(5)] public byte[] Tag { get; set; } = Array.Empty<byte>();
    [Key(6)] public DateTime? ExpiresAtUtc { get; set; }
    [Key(7)] public List<string> AllowedIndividuals { get; set; } = [];
    [Key(8)] public List<string> AllowedSyncshells { get; set; } = [];
}
