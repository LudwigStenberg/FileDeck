using System.Threading.Tasks;
using FileDeck.api.DTOs;

namespace FileDeck.api.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponse> CreateFolderAsync(CreateFolderRequest folderDto, string userId);
    Task<FolderResponse?> GetFolderByIdAsync(int folderId, string userId);
    Task<bool> RenameFolderAsync(int folderId, RenameFolderRequest request, string userId);
    Task<bool> DeleteFolderAsync(int folderId, string userId);

}