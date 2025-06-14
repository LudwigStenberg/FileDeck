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
    /// Asynchronously uploads a file to the database for the provided user.
    /// </summary>
    /// <param name="file">The uploaded file containing content and metadata.</param>
    /// <param name="folderId">The ID of the folder to store the file in, or null for root.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponse which contains information on the newly uploaded file.</returns>
    /// <exception cref="EmptyNameException">Thrown when the file name is empty or whitespace.</exception>
    /// <exception cref="NameTooLongException">Thrown when the file name exceeds the maximum allowed length.</exception>
    /// <exception cref="InvalidCharactersException">Thrown when the file name contains invalid characters.</exception>
    /// <exception cref="FolderNotFoundException">Thrown when the specified folder cannot be found.</exception>
    public async Task<FileResponse> UploadFileAsync(IFormFile file, int? folderId, string userId)
    {

        logger.LogInformation("File upload initiated for user with ID: '{UserId}': {FileName}, {FileSize} bytes",
            userId, file.FileName, file.Length);

        string fileName = file.FileName;
        string contentType = file.ContentType;

        byte[] content;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            content = memoryStream.ToArray();
        }

        await ValidateFileUploadRequest(file, content, folderId, userId);

        var newFile = FileMapper.ToEntity(file, content, folderId, userId);

        logger.LogDebug("Saving new file to database: {FileName}, {ContentType}, {FileSize} bytes",
            fileName, contentType, content.Length);

        var savedFile = await fileRepository.CreateFileAsync(newFile);

        logger.LogInformation("File successfully uploaded: ID={FileId}, {FileName} by user {UserId}",
            savedFile.Id, savedFile.Name, userId);

        return FileMapper.ToResponse(savedFile);
    }

    /// <summary>
    /// Asynchronously retrieves a file based on the file ID provided.
    /// </summary>
    /// <param name="fileId">The ID of the file to be retrieved.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileResponse containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file ID cannot be found for the provided user ID.</exception>
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
    ///  Asynchronously retrieves all the files located in the root.
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
    /// Asynchronously retrieves and downloads a file from the database.
    /// </summary>
    /// <param name="fileId">The ID of the file to be downloaded.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A FileDownloadResponse containing information about the file if found and if the user has access to it; otherwise returns null.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file ID cannot be found for the provided user ID.</exception>
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
    /// Asynchronously retrieves information on the files within a specific folder.
    /// </summary>
    /// <param name="folderId">The folder ID of the folder containing the files.</param>
    /// <param name="userId">The ID of the user requesting the file and who should have access to it.</param>
    /// <returns>A list of FileResponse objects if the request is successful; otherwise it returns an empty enumerable. </returns>
    /// <exception cref="FolderNotFoundException">Thrown when the folder ID cannot be found for the provided user ID.</exception>
    public async Task<IEnumerable<FileResponse>> GetFilesInFolderAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of files in folder {FolderId} initiated by user {UserId}", folderId, userId);

        bool folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("File retrieval failed: the folder {FolderId} does not exist for user {UserId}", folderId, userId);
            throw new FolderNotFoundException(folderId);
        }

        logger.LogDebug("Folder {FolderId} was found. Retrieving the files within for user {UserId}", folderId, userId);

        var files = await fileRepository.GetFilesInFolderAsync(folderId, userId);

        var filesList = files.ToList();

        logger.LogInformation("Successfully retrieved {FileCount} files from folder {FolderId} for user {UserId}",
            filesList.Count, folderId, userId);

        return filesList.Select(FileMapper.ToResponse);
    }

    /// <summary>
    /// Asynchronously deletes a specific file by means of a soft-delete.
    /// </summary>
    /// <param name="fileId">The ID of the file to be deleted.</param>
    /// <param name="userId">The ID of the user requesting to delete the file and who should have access to it.</param>
    /// <exception cref="FileNotFoundException">Thrown when the file ID cannot be found for the provided user ID.</exception>
    public async Task DeleteFileAsync(int fileId, string userId)
    {
        logger.LogInformation("Deletion of file {FileId} initiated for user {UserId}", fileId, userId);

        bool fileExists = await fileRepository.FileExistsAsync(fileId, userId);
        if (!fileExists)
        {
            logger.LogWarning("File deletion failed. File {FileId} could not be found for user {UserId}", fileId, userId);
            throw new FileNotFoundException(fileId);
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
    }

    #region Helper Methods

    private async Task ValidateFileUploadRequest(IFormFile file, byte[] content, int? folderId, string userId)
    {

        if (file.Length > ValidationConstants.MaxFileSizeBytes)
        {
            throw new FileTooLargeException();
        }

        if (string.IsNullOrWhiteSpace(file.FileName))
        {
            logger.LogWarning("File creation failed for user {UserId}. Name is null or has whitespace.", userId);
            throw new EmptyNameException("file");
        }

        if (file.FileName.Length > ValidationConstants.MaxFileNameLength)
        {
            logger.LogWarning("File upload rejected: name too long ({NameLength} chars) for user {UserId}",
                file.FileName.Length, userId);
            throw new NameTooLongException("file", ValidationConstants.MaxFileNameLength);
        }

        if (file.FileName.Any(ValidationConstants.InvalidNameCharacters.Contains))
        {
            logger.LogWarning("File upload rejected: invalid characters in filename for user {UserId}", userId);
            throw new InvalidCharactersException("file");
        }

        if (folderId.HasValue)
        {
            logger.LogDebug("Checking if folder {FolderId} exists for user {UserId}", folderId.Value, userId);

            bool folderExists = await folderRepository.FolderExistsAsync(folderId.Value, userId);
            if (!folderExists)
            {
                logger.LogWarning("File upload rejected: specified folder {FolderId} does not exist for user {UserId}",
                    folderId.Value, userId);
                throw new FolderNotFoundException(folderId.Value);
            }
        }
    }

    #endregion
}