namespace FileDeck.api.DTOs;

public class CreateFolderRequest
{
    public required string Name { get; set; }
    public int? ParentFolderId { get; set; }
}