using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Models;

namespace FileDeck.api.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto folderDto, string userId);
}