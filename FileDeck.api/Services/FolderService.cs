
using System;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Services.Interfaces;

public class FolderService : IFolderService
{
    public async Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto folderDto, string userId)
    {
        // Validate folder name and format
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

        // ParentFolderId? Verify it's correct and that it belongs to this user

        // Build the FolderEntity that will be saved/persisted to the database
        // Call the FolderRepository.CreateFolderAsync 
        // Map the result to a FolderResponseDto
        // Return success?? with the dto

        return folderResponseDto;
    }
}