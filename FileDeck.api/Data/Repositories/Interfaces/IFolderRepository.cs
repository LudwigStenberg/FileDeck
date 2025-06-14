using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<FolderEntity> CreateFolderAsync(FolderEntity folder);
    Task<FolderEntity?> GetFolderByIdAsync(int folderId, string userId);
    Task<IEnumerable<FolderEntity>> GetAllFoldersAsync(string userId);
    Task<IEnumerable<FolderEntity>> GetSubfoldersAsync(int folderId, string userId);
    Task<IEnumerable<FolderEntity>> GetRootFoldersAsync(string userId);
    Task<bool> FolderExistsAsync(int parentFolderId, string userId);
    Task RenameFolderAsync(int folderId, string newName, string userId);
    Task DeleteFolderAsync(int folderId, string userId);
    Task<DeletionResult> HardDeleteOldFoldersAsync(DateTime cutOffDate);
}