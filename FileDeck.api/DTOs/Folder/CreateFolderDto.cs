namespace FileDeck.api.DTOs;

public class CreateFolderDto
{
    public required string Name { get; set; }
    public int? ParentFolderId { get; set; }
}