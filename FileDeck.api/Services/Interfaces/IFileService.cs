using FileDeck.api.DTOs;

namespace FileDeck.api.Services;

public interface IFileService
{
    Task<FileResponse> UploadFileAsync(FileUploadRequest request, string userId);

    Task<FileResponse> GetFileByIdAsync(int fileId, string userId);

    Task<IEnumerable<FileResponse>> GetRootFilesAsync(string userId);
    Task<FileDownloadResponse> DownloadFileAsync(int fileId, string userId);

    Task<IEnumerable<FileResponse>> GetFilesInFolderAsync(int folderId, string userId);

    Task<bool> DeleteFileAsync(int fileId, string userId); // Soft delete
}

