
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Repositories;
using FileDeck.api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FileDeck.api.Services;
public class FileService : IFileService
{
    private readonly IFolderRepository folderRepository;
    private readonly IFileRepository fileRepository;
    private readonly ILogger logger;

    public FileService(
        IFolderRepository folderRepository,
        IFileRepository fileRepository,
        ILogger logger)
    {
        this.folderRepository = folderRepository;
        this.fileRepository = fileRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Uploads a file to the database for the provided user.
    /// </summary>
    /// <param name="fileUpload">The DTO that contains the information on the new file.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponseDto which contains information on the newly uploaded file.</returns>
    /// <exception cref="ArgumentException">The exceptions thrown when the arguments do not fulfill either one of: Name.Length, no invalid characters or if a folder associated with the file, doesn't exist.</exception>
    public async Task<FileResponseDto> UploadFileAsync(FileUploadDto fileUpload, string userId)
    {
        logger.LogInformation("File upload initiated for user {UserId}: {FileName}, {FileSize} bytes",
            userId, fileUpload.Name, fileUpload.Content.Length);

        if (fileUpload.Name.Length > 50)
        {
            logger.LogWarning("File upload rejected: name too long ({NameLength} chars) for user {UserId}",
                fileUpload.Name.Length, userId);
            throw new ArgumentException("File name cannot be longer than 50 characters");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (fileUpload.Name.Any(invalidChars.Contains))
        {
            logger.LogWarning("File upload rejected: invalid characters in filename for user {UserId}", userId);
            throw new ArgumentException("Folder name contains invalid characters");
        }

        if (fileUpload.FolderId.HasValue)
        {
            logger.LogDebug("Checking if folder {FolderId} exists for user {UserId}", fileUpload.FolderId.Value, userId);
            bool folderExists = await folderRepository.FolderExistsAsync(fileUpload.FolderId.Value, userId);
            if (!folderExists)
            {
                logger.LogWarning("File upload rejected: specified folder {FolderId} does not exist for user {UserId}",
                    fileUpload.FolderId.Value, userId);
                throw new ArgumentException("The specified folder does not exist or you don't have access to it");
            }
        }

        var newFile = new FileEntity
        {
            Name = fileUpload.Name,
            ContentType = fileUpload.ContentType,
            Content = fileUpload.Content,
            Size = fileUpload.Content.Length,
            UserId = userId,
            FolderId = fileUpload.FolderId,
            UploadDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        logger.LogDebug("Saving new file to database: {FileName}, {ContentType}, {FileSize} bytes",
            fileUpload.Name, fileUpload.ContentType, fileUpload.Content.Length);
        var savedFile = await fileRepository.CreateFileAsync(newFile);

        logger.LogInformation("File successfully uploaded: ID={FileId}, {FileName} by user {UserId}",
            savedFile.Id, savedFile.Name, userId);

        return new FileResponseDto
        {
            Id = savedFile.Id,
            Name = savedFile.Name,
            ContentType = savedFile.ContentType,
            Size = savedFile.Size,
            UploadDate = savedFile.UploadDate,
            LastModifiedDate = savedFile.LastModifiedDate,
            FolderId = savedFile.FolderId
        };
    }


    /// <summary>
    /// Retrieves a file based on the file ID provided.
    /// </summary>
    /// <param name="fileId">The ID of the file to be retrieved.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponseDto containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    public async Task<FileResponseDto?> GetFileByIdAsync(int fileId, string userId)
    {
        logger.LogInformation("File retrieval initiated for user {UserId}. File: {FileId}", userId, fileId);
        var fileEntity = await fileRepository.GetFileByIdAsync(fileId, userId);

        if (fileEntity == null)
        {
            logger.LogWarning("File with ID {FileId} for user {UserId} could not be found.", fileId, userId);
            return null;
        }

        logger.LogInformation("File {FileId} successfully retrieved for user {UserId} - {FileName}",
            fileId, userId, fileEntity.Name);
        return new FileResponseDto
        {
            Id = fileEntity.Id,
            Name = fileEntity.Name,
            ContentType = fileEntity.ContentType,
            Size = fileEntity.Size,
            UploadDate = fileEntity.UploadDate,
            LastModifiedDate = fileEntity.LastModifiedDate,
            FolderId = fileEntity.FolderId
        };
    }

    /// <summary>
    /// Retrieves and downloads a file from the database.
    /// </summary>
    /// <param name="fileId">The ID of the file to be downloaded.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileDownloadDto containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    public async Task<FileDownloadDto?> DownloadFileAsync(int fileId, string userId)
    {
        var fileEntity = await fileRepository.GetFileByIdAsync(fileId, userId);

        if (fileEntity == null)
        {
            return null;
        }

        return new FileDownloadDto
        {
            Id = fileEntity.Id,
            Name = fileEntity.Name,
            ContentType = fileEntity.ContentType,
            Content = fileEntity.Content
        };
    }

    /// <summary>
    /// Retrieves information on the files within a specific folder.
    /// </summary>
    /// <param name="folderId">The folder ID of the folder containing the files.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A list of FileResponseDto objects if the request is successful; otherwise it returns an empty enumerable. </returns>
    public async Task<IEnumerable<FileResponseDto>> GetFilesInFolderAsync(int folderId, string userId)
    {
        bool folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            return Enumerable.Empty<FileResponseDto>();
        }

        var files = await fileRepository.GetFilesInFolderAsync(folderId, userId);

        return files.Select(file => new FileResponseDto
        {
            Id = file.Id,
            Name = file.Name,
            ContentType = file.ContentType,
            Size = file.Size,
            UploadDate = file.UploadDate,
            LastModifiedDate = file.LastModifiedDate,
            FolderId = file.FolderId
        }).ToList();
    }

    /// <summary>
    /// Deletes a specific file by means of a soft-delete.
    /// </summary>
    /// <param name="fileId">The ID of the file to be deleted.</param>
    /// <param name="userId">The ID of the user requesting to delete the file and who should have access to it.</param>
    /// <returns>A boolean value to indicate whether the operation was a success or a failure.</returns>
    public async Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        bool fileExists = await fileRepository.FileExistsAsync(fileId, userId);
        if (!fileExists)
        {
            return false;
        }

        return await fileRepository.DeleteFileAsync(fileId, userId);
    }
}