using System.Threading.Tasks;
using FileDeck.api.DTOs;

namespace FileDeck.api.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto folderDto, string userId);
    Task<FolderResponseDto?> GetFolderByIdAsync(int folderId);

}