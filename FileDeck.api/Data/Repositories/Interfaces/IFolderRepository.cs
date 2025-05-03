using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories.Interfaces;
public interface IFolderRepository
{
    Task<FolderEntity> CreateFolderAsync(FolderEntity folder);
    Task<FolderEntity?> GetFolderByIdAsync(int folderId, string userId);
    Task<IEnumerable<FolderEntity>> GetSubfoldersAsync(int folderId, string userId);
    Task<IEnumerable<FolderEntity>> GetRootFoldersAsync(string userId);
    Task<IEnumerable<FolderEntity>> GetFolderPathAsync(int folderId, string userId);
    Task<bool> FolderExistsAsync(int parentFolderId, string userId);
    Task<bool> RenameFolderAsync(int folderId, string newName, string userId);
    Task<bool> DeleteFolderAsync(int folderId, string userId);

}