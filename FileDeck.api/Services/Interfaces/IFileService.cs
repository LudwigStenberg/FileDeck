
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Models;

namespace FileDeck.api.Services;

public interface IFileService
{
    Task<FileResponseDto> UploadFileAsync(FileUploadDto fileUpload, string userId);

    Task<FileResponseDto> DownloadFileAsync(int fileId, string userId);

    Task<IEnumerable<FileMetadataDto>> GetFilesInFolderAsync(int folderId, string userId);

    Task<bool> DeleteFileAsync(int fileId, string userId); // Soft delete

    Task<bool> FileExistsAsaync(int fileId, string userId);
}

