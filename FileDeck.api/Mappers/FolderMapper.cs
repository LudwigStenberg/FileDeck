
using FileDeck.api.DTOs;
using FileDeck.api.Models;

public static class FolderMapper
{
    public static FolderEntity ToEntity(CreateFolderRequest request, string userId)
    {
        return new FolderEntity
        {
            Name = request.Name,
            ParentFolderId = request.ParentFolderId,
            UserId = userId

        };
    }

    public static FolderResponse ToResponse(FolderEntity folder)
    {
        return new FolderResponse
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            CreatedDate = folder.CreatedDate
        };
    }
}