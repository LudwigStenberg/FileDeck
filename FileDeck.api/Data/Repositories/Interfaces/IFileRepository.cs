using FileDeck.api.Models;

namespace FileDeck.api.Repositories;

public interface IFileRepository
{
    Task<FileEntity> CreateFileAsync(FileEntity file);
    Task<FileEntity?> GetFileByIdAsync(int fileId, string userId);
    Task<IEnumerable<FileEntity>> GetRootFilesAsync(string userId);
    Task<IEnumerable<FileEntity>> GetFilesInFolderAsync(int folderId, string userId);
    Task<bool> DeleteFileAsync(int fileId, string userId); // Soft delete!
    Task<DeletionResult> HardDeleteOldFilesAsync(DateTime cutOffDate);
    Task<bool> FileExistsAsync(int fileId, string userId);

}