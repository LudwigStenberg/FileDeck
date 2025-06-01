using FileDeck.api.DTOs;
using FileDeck.api.Models;

public static class FileMapper
{
    public static FileEntity ToEntity(FileUploadRequest request, string userId)
    {
        return new FileEntity
        {
            Name = request.Name,
            ContentType = request.ContentType,
            Content = request.Content,
            Size = request.Content.Length,
            FolderId = request.FolderId,
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