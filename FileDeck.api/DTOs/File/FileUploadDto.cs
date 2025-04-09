using System;

namespace FileDeck.api.DTOs;
public class FileUploadDto
{
    public required string Name { get; set; }
    public required string ContentType { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public int? FolderId { get; set; } // Needed if file should be within a folder

}