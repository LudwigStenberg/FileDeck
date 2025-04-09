
using System;

namespace FileDeck.api.DTOs;
public class FileDownloadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required string ContentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
}