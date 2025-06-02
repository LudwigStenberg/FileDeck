using FileDeck.api.DTOs;
using FileDeck.api.Repositories;
using FileDeck.api.Repositories.Interfaces;

namespace FileDeck.api.Services;

public class FileService : IFileService
{
    private readonly IFolderRepository folderRepository;
    private readonly IFileRepository fileRepository;
    private readonly ILogger<FileService> logger;

    public FileService(
        IFolderRepository folderRepository,
        IFileRepository fileRepository,
        ILogger<FileService> logger)
    {
        this.folderRepository = folderRepository;
        this.fileRepository = fileRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Uploads a file to the database for the provided user.
    /// </summary>
    /// <param name="request">The DTO that contains the information on the new file.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponse which contains information on the newly uploaded file.</returns>
    /// <exception cref="ArgumentException">The exceptions thrown when the arguments do not fulfill either one of: Name.Length, no invalid characters or if a folder associated with the file, doesn't exist.</exception>
    public async Task<FileResponse> UploadFileAsync(FileUploadRequest request, string userId)
    {
        logger.LogInformation("File upload initiated for user {UserId}: {FileName}, {FileSize} bytes",
            userId, request.Name, request.Content.Length);

        await ValidateFileUploadRequest(request, userId);

        var newFile = FileMapper.ToEntity(request, userId);

        logger.LogDebug("Saving new file to database: {FileName}, {ContentType}, {FileSize} bytes",
            request.Name, request.ContentType, request.Content.Length);

        var savedFile = await fileRepository.CreateFileAsync(newFile);

        logger.LogInformation("File successfully uploaded: ID={FileId}, {FileName} by user {UserId}",
            savedFile.Id, savedFile.Name, userId);

        return FileMapper.ToResponse(savedFile);
    }

    /// <summary>
    /// Retrieves a file based on the file ID provided.
    /// </summary>
    /// <param name="fileId">The ID of the file to be retrieved.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponse containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    public async Task<FileResponse> GetFileByIdAsync(int fileId, string userId)
    {
        logger.LogInformation("File retrieval initiated for user {UserId}. File: {FileId}", userId, fileId);
        var fileEntity = await fileRepository.GetFileByIdAsync(fileId, userId);

        if (fileEntity == null)
        {
            logger.LogWarning("File {FileId} for user {UserId} could not be found.", fileId, userId);
            throw new FileNotFoundException(fileId);
        }

        logger.LogInformation("File {FileId} successfully retrieved for user {UserId} - {FileName}",
            fileId, userId, fileEntity.Name);

        return FileMapper.ToResponse(fileEntity);
    }

    /// <summary>
    /// Retrieves all the files located in the root.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the files and who has access to it.</param>
    /// <returns>A list containing FileResponse DTOs. It can be empty.</returns>
    public async Task<IEnumerable<FileResponse>> GetRootFilesAsync(string userId)
    {
        logger.LogInformation("Retrieval of root files initiated for user {UserId}.", userId);

        var rootFiles = await fileRepository.GetRootFilesAsync(userId);
        var rootFileList = rootFiles.ToList();

        logger.LogInformation("Retrieval of root files successful for user {UserId}. Found {FileCount} files.", userId, rootFileList.Count);

        return rootFileList.Select(FileMapper.ToResponse);
    }

    /// <summary>
    /// Retrieves and downloads a file from the database.
    /// </summary>
    /// <param name="fileId">The ID of the file to be downloaded.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileDownloadResponse containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    public async Task<FileDownloadResponse> DownloadFileAsync(int fileId, string userId)
    {
        logger.LogInformation("File download initiated for user: {UserId}. File: {FileId}", userId, fileId);

        var fileEntity = await fileRepository.GetFileByIdAsync(fileId, userId);

        if (fileEntity == null)
        {
            logger.LogWarning("File download for user {UserId} failed. File: {FileId} could not be found.", userId, fileId);
            throw new FileNotFoundException(fileId);
        }

        logger.LogInformation("File {FileId} successfully downloaded for user {UserId} - {FileName}", fileId, userId, fileEntity.Name);

        return FileMapper.ToDownloadResponse(fileEntity);
    }

    /// <summary>
    /// Retrieves information on the files within a specific folder.
    /// </summary>
    /// <param name="folderId">The folder ID of the folder containing the files.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A list of FileResponse objects if the request is successful; otherwise it returns an empty enumerable. </returns>
    public async Task<IEnumerable<FileResponse>> GetFilesInFolderAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of files in folder {FolderId} initiated by user {UserId}", folderId, userId);

        bool folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("File retrieval failed: the folder {FolderId} does not exist for user {UserId}", folderId, userId);
            return Enumerable.Empty<FileResponse>();
        }

        logger.LogDebug("Folder {FolderId} was found. Retrieving the files within for user {UserId}", folderId, userId);

        var files = await fileRepository.GetFilesInFolderAsync(folderId, userId);

        var filesList = files.ToList();

        logger.LogInformation("Successfully retrieved {FileCount} files from folder {FolderId} for user {UserId}",
            filesList.Count, folderId, userId);

        return filesList.Select(FileMapper.ToResponse);
    }

    /// <summary>
    /// Deletes a specific file by means of a soft-delete.
    /// </summary>
    /// <param name="fileId">The ID of the file to be deleted.</param>
    /// <param name="userId">The ID of the user requesting to delete the file and who should have access to it.</param>
    /// <returns>A boolean value to indicate whether the operation was a success or a failure.</returns>
    public async Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        logger.LogInformation("Deletion of file {FileId} initiated for user {UserId}", fileId, userId);

        bool fileExists = await fileRepository.FileExistsAsync(fileId, userId);
        if (!fileExists)
        {
            logger.LogWarning("File deletion failed. File {FileId} could not be found for user {UserId}", fileId, userId);
            return false;
        }

        bool result = await fileRepository.DeleteFileAsync(fileId, userId);

        if (result)
        {
            logger.LogInformation("File {FileId} successfully removed for user {UserId}", fileId, userId);
        }
        else
        {
            logger.LogWarning("File {FileId} deletion failed for user {UserId} during database operation", fileId, userId);
        }

        return result;
    }

    #region Helper Methods

    private async Task ValidateFileUploadRequest(FileUploadRequest request, string userId)
    {
        if (request.Name.Length > 50)
        {
            logger.LogWarning("File upload rejected: name too long ({NameLength} chars) for user {UserId}",
                request.Name.Length, userId);
            throw new ArgumentException("File name cannot be longer than 50 characters");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (request.Name.Any(invalidChars.Contains))
        {
            logger.LogWarning("File upload rejected: invalid characters in filename for user {UserId}", userId);
            throw new ArgumentException("Folder name contains invalid characters");
        }

        if (request.FolderId.HasValue)
        {
            logger.LogDebug("Checking if folder {FolderId} exists for user {UserId}", request.FolderId.Value, userId);
            bool folderExists = await folderRepository.FolderExistsAsync(request.FolderId.Value, userId);
            if (!folderExists)
            {
                logger.LogWarning("File upload rejected: specified folder {FolderId} does not exist for user {UserId}",
                    request.FolderId.Value, userId);
                throw new ArgumentException("The specified folder does not exist or you don't have access to it");
            }
        }
    }

    #endregion
}