using System;

namespace FileDeck.api.DTOs;
public class FileMetadataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}