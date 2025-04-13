using System.Threading.Tasks;
using FileDeck.api.DTOs;

namespace FileDeck.api.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto folderDto, string userId);
    Task<FolderResponseDto?> GetFolderByIdAsync(int folderId, string userId);
    Task<bool> RenameFolderAsync(int folderId, string newName, string userId);
    Task<bool> DeleteFolderAsync(int folderId, string userId);

}