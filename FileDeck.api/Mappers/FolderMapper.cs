
using FileDeck.api.DTOs;
using FileDeck.api.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

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
}