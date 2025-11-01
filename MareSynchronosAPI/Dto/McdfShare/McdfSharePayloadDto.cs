using System;

namespace MareSynchronos.API.Dto.McdfShare;

public class McdfSharePayloadDto
{
    public Guid ShareId { get; set; }
    public string Description { get; set; } = string.Empty;
    public byte[] CipherData { get; set; } = Array.Empty<byte>();
    public byte[] Nonce { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public byte[] Tag { get; set; } = Array.Empty<byte>();
    public DateTime CreatedUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}
