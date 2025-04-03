using System;

namespace FileDeck.api.DTOs;

public class FolderResponseDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentFolderId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}