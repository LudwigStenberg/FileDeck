using System;

namespace FileDeck.api.DTOs;

public class FolderResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentFolderId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}