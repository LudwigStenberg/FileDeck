using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.DTOs;

namespace FileDeck.api.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponse> CreateFolderAsync(CreateFolderRequest request, string userId);
    Task<FolderResponse> GetFolderByIdAsync(int folderId, string userId);
    Task<IEnumerable<FolderResponse>> GetAllFoldersAsync(string userId);
    Task<IEnumerable<FolderResponse>> GetSubfoldersAsync(int folderId, string userId);
    Task<IEnumerable<FolderResponse>> GetRootFoldersAsync(string userId);
    Task RenameFolderAsync(int folderId, RenameFolderRequest request, string userId);
    Task<bool> DeleteFolderAsync(int folderId, string userId);
}