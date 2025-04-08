
using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories;
public class FileRepository : IFileRepository
{
    public Task<FileEntity> CreateFileAsync(FileEntity file)
    {
        throw new System.NotImplementedException();
    }

    public Task<FileEntity?> GetFileByIdAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<FileEntity>> GetFilesInFolderAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> FileExistsAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }
}