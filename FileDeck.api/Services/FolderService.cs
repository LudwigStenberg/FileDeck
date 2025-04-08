using System;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Models;
using FileDeck.api.Repositories.Interfaces;
using FileDeck.api.Services.Interfaces;

namespace FileDeck.api.Services;
public class FolderService : IFolderService
{
    private readonly IFolderRepository folderRepository;
    public FolderService(IFolderRepository folderRepository)
    {
        this.folderRepository = folderRepository;
    }

    public async Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto folderDto, string userId)
    {
        Console.WriteLine($"Service - User ID from token: '{userId}'");

        if (string.IsNullOrWhiteSpace(folderDto.Name))
        {
            throw new ArgumentException("Folder name cannot be empty.");
        }

        if (folderDto.Name.Length > 50)
        {
            throw new ArgumentException("Folder name cannot be longer than 50 characters.");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (folderDto.Name.Any(invalidChars.Contains))
        {
            throw new ArgumentException("Folder name contains invalid characters.");
        }


        if (folderDto.ParentFolderId != null)
        {
            bool parentFolderExists = await folderRepository.FolderExistsAsync(folderDto.ParentFolderId.Value, userId);

            if (!parentFolderExists)
            {
                throw new ArgumentException("Parent folder does not exist or you do not have access to it.");
            }
        }


        // Build the FolderEntity that will be saved/persisted to the database
        var newFolder = new FolderEntity
        {
            Name = folderDto.Name,
            ParentFolderId = folderDto.ParentFolderId,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        var savedFolder = await folderRepository.CreateFolderAsync(newFolder);

        // Use the saved folder to map our response which is to be sent to the controller
        var FolderResponseDto = new FolderResponseDto
        {
            Id = savedFolder.Id,
            Name = savedFolder.Name,
            ParentFolderId = savedFolder.ParentFolderId,
            CreatedDate = savedFolder.CreatedDate
        };

        return FolderResponseDto;
    }

    public async Task<FolderResponseDto?> GetFolderByIdAsync(int folderId, string userId)
    {
        var folder = await folderRepository.GetFolderByIdAsync(folderId, userId);
        if (folder == null)
        {
            return null;
        }

        var FolderResponseDto = new FolderResponseDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            CreatedDate = folder.CreatedDate
        };

        return FolderResponseDto;
    }
}