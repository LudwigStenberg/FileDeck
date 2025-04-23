using System;
using System.ComponentModel.DataAnnotations;

namespace FileDeck.api.DTOs;

public class FileUploadRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Name { get; set; }
    [Required]
    public required string ContentType { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public int? FolderId { get; set; }

}