

using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories.Interfaces;
public interface IFolderRepository
{
    Task<FolderEntity> CreateFolderAsync(FolderEntity folder);
    Task<FolderEntity?> GetFolderByIdAsync(int folderId, string userId);
    Task<bool> FolderExistsAsync(int parentFolderId, string userId);

}