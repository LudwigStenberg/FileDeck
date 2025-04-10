
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using FileDeck.api.DTOs;

namespace FileDeck.api.Services;

public interface IFileService
{
    Task<FileResponseDto> UploadFileAsync(FileUploadDto fileUpload, string userId);

    Task<FileResponseDto?> GetFileByIdAsync(int fileId, string userId);

    Task<FileDownloadDto?> DownloadFileAsync(int fileId, string userId);

    Task<IEnumerable<FileResponseDto>> GetFilesInFolderAsync(int folderId, string userId);

    Task<bool> DeleteFileAsync(int fileId, string userId); // Soft delete

    Task<bool> FileExistsAsync(int fileId, string userId);
}

