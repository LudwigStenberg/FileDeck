

using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories.Interfaces;
public interface IFolderRepository
{
    Task<FolderEntity> CreateFolderAsync(FolderEntity folder);
    // Task<FolderEntity> GetById(FolderEntity folder);
    Task<bool> FolderExistsAsync(int parentFolderId, string userId);

}