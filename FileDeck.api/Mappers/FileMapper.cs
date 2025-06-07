using FileDeck.api.DTOs;
using FileDeck.api.Models;

public static class FileMapper
{
    public static FileEntity ToEntity(IFormFile file, byte[] content, int? folderId, string userId)
    {
        return new FileEntity
        {
            Name = file.FileName,
            ContentType = file.ContentType,
            Content = content,
            Size = content.Length,
            FolderId = folderId,
            UserId = userId
        };
    }

    public static FileResponse ToResponse(FileEntity file)
    {
        return new FileResponse
        {
            Id = file.Id,
            Name = file.Name,
            ContentType = file.ContentType,
            Size = file.Size,
            UploadDate = file.UploadDate,
            LastModifiedDate = file.LastModifiedDate,
            FolderId = file.FolderId
        };
    }

    public static FileDownloadResponse ToDownloadResponse(FileEntity file)
    {
        return new FileDownloadResponse
        {
            Id = file.Id,
            Name = file.Name,
            ContentType = file.ContentType,
            Content = file.Content
        };
    }
}