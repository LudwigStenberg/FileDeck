
using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories;
public interface IFileRepository
{
    Task<FileEntity> CreateFileAsync(FileEntity file);
    Task<FileEntity?> GetFileByIdAsync(int fileId, string userId);
    Task<bool> DeleteFileAsync(int fileId, string userId); // Soft delete!
    Task<bool> FileExistsAsync(int fileId, string userId);
    Task<IEnumerable<FileEntity>> GetFilesInFolderAsync();

}