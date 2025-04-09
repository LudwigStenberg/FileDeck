
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Models;
using FileDeck.api.Repositories;
using FileDeck.api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FileDeck.api.Services;
public class FileService : IFileService
{
    private readonly IFolderRepository folderRepository;
    private readonly IFileRepository fileRepository;

    public FileService(IFolderRepository folderRepository, IFileRepository fileRepository)
    {
        this.folderRepository = folderRepository;
        this.fileRepository = fileRepository;
    }

    public async Task<FileResponseDto> UploadFileAsync(FileUploadDto fileUpload, string userId)
    {
        if (fileUpload.Name.Length > 50)
        {
            throw new ArgumentException("File name cannot be longer than 50 characters");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (fileUpload.Name.Any(invalidChars.Contains))
        {
            throw new ArgumentException("Folder name contains invalid characters");
        }

        if (fileUpload.FolderId.HasValue)
        {
            bool folderExists = await folderRepository.FolderExistsAsync(fileUpload.FolderId.Value, userId);
            if (!folderExists)
            {
                throw new ArgumentException("The specified folder does not existo r you don't have access to it");
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

        var savedFile = await fileRepository.CreateFileAsync(newFile);

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

    public Task<FileDownloadDto> DownloadFileAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<FileResponseDto>> GetFilesInFolderAsync(int folderId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> FileExistsAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

}