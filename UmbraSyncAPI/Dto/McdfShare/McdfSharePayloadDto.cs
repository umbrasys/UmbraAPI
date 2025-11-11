using System;
using MessagePack;

namespace UmbraSync.API.Dto.McdfShare;

[MessagePackObject]
public class McdfSharePayloadDto
{
    [Key(0)] public Guid ShareId { get; set; }
    [Key(1)] public string Description { get; set; } = string.Empty;
    [Key(2)] public byte[] CipherData { get; set; } = Array.Empty<byte>();
    [Key(3)] public byte[] Nonce { get; set; } = Array.Empty<byte>();
    [Key(4)] public byte[] Salt { get; set; } = Array.Empty<byte>();
    [Key(5)] public byte[] Tag { get; set; } = Array.Empty<byte>();
    [Key(6)] public DateTime CreatedUtc { get; set; }
    [Key(7)] public DateTime? ExpiresAtUtc { get; set; }
}
