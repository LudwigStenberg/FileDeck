using FileDeck.api.DTOs;
using FileDeck.api.Repositories.Interfaces;
using FileDeck.api.Services.Interfaces;

namespace FileDeck.api.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository folderRepository;
    private readonly ILogger<FolderService> logger;
    public FolderService(IFolderRepository folderRepository, ILogger<FolderService> logger)
    {
        this.folderRepository = folderRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Asynchronously creates a new folder based on the information provided in the input DTO.
    /// </summary>
    /// <param name="request">The DTO used to create the new folder. Contains the name and the ID of the parent folder, if there is one.</param>
    /// <param name="userId">The ID of the user requesting the folder to be created and who will have access to it.</param>
    /// <returns>A FolderResponse with additional data that was created during construction.</returns>
    /// <exception cref="EmptyNameException">Thrown when the folder name is empty or whitespace.</exception>
    /// <exception cref="NameTooLongException">Thrown when the folder name exceeds the maximum allowed length.</exception>
    /// <exception cref="InvalidCharactersException">Thrown when the folder name contains invalid characters.</exception>
    /// <exception cref="FolderNotFoundException">Thrown when the parent folder doesn't exist despite request.ParentFolderId being populated.</exception>
    public async Task<FolderResponse> CreateFolderAsync(CreateFolderRequest request, string userId)
    {
        logger.LogInformation("Initiated creation of folder with name {FolderName} by user {UserId}", request.Name, userId);

        await ValidateCreateFolderRequest(request, userId);

        var newFolder = FolderMapper.ToEntity(request, userId);

        logger.LogDebug("Attempting to create the new folder {FolderName} for user {UserId}", request.Name, userId);

        var savedFolder = await folderRepository.CreateFolderAsync(newFolder);

        logger.LogInformation("Folder {FolderName} (ID: {FolderId})for user {UserId} successfully created", request.Name, savedFolder.Id, userId);
        return FolderMapper.ToResponse(savedFolder);
    }

    /// <summary>
    /// Asynchronously retrieves a specific folder and its information.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be retrieved.</param>
    /// <param name="userId">The ID of the user requesting the folder and who will have access to it.</param>
    /// <returns>A FolderResponse which contains the information on the folder that has been retrieved. If no folder is found, returns null.</returns>
    /// <exception cref="FolderNotFoundException">Thrown when the folder associated with the file cannot be found.</exception>
    public async Task<FolderResponse> GetFolderByIdAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of folder {FolderId} for user {UserId} initiated", folderId, userId);

        var folder = await folderRepository.GetFolderByIdAsync(folderId, userId);
        if (folder == null)
        {
            logger.LogWarning("Folder retrieval failed. The folder {FolderId} could not be found for user {UserId}", folderId, userId);
            throw new FolderNotFoundException(folderId);
        }

        logger.LogInformation("Folder retrieval successful for user {UserId}. Folder {FolderId}, {FolderName} was found.",
            userId, folderId, folder.Name);

        return FolderMapper.ToResponse(folder);
    }

    /// <summary>
    /// Asynchronously retrieves all folders belonging to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user requesting to retrieve the folders, and who has access to them.</param>
    /// <returns>A list containing FolderResponse DTOs. It can be empty.</returns>
    public async Task<IEnumerable<FolderResponse>> GetAllFoldersAsync(string userId)
    {
        var folders = await folderRepository.GetAllFoldersAsync(userId);

        var folderList = folders.ToList();

        logger.LogInformation("Retrieval of all folders successful for user {UserId}. Found {FolderCount}.", userId, folderList.Count);

        return folderList.Select(FolderMapper.ToResponse);
    }

    /// <summary>
    /// Asynchronously retrieves the subfolders within a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be searched within.</param>
    /// <param name="userId">The ID of the user requesting to see the subfolders and who has access to it.</param>
    /// <returns>A list containing FolderResponse DTOs. It can be empty.</returns>
    /// <exception cref="FolderNotFoundException">Thrown when the folder cannot be found for the provided User ID.</exception>
    public async Task<IEnumerable<FolderResponse>> GetSubfoldersAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of subfolders initiated for user {UserId}. ID of the folder to be searched within: {FolderId}", userId, folderId);

        bool parentExists = await folderRepository.FolderExistsAsync(folderId, userId);
        if (!parentExists)
        {
            logger.LogWarning("Folder retrieval failed. The folder {FolderId} could not be found for user {UserId}", folderId, userId);
            throw new FolderNotFoundException(folderId);
        }

        var subfolders = await folderRepository.GetSubfoldersAsync(folderId, userId);

        var subfolderList = subfolders.ToList();

        logger.LogInformation("Retrieval of subfolders successful for user {UserId}. Found {SubfolderCount}.", userId, subfolderList.Count);
        return subfolderList.Select(FolderMapper.ToResponse);
    }

    /// <summary>
    /// Asynchronously retrieves the root folders for a specific user. 
    /// </summary>
    /// <param name="userId">The ID of the user requesting the retrieval.</param>
    /// <returns>A list of FolderResponse DTOs, can be empty.</returns>
    public async Task<IEnumerable<FolderResponse>> GetRootFoldersAsync(string userId)
    {
        logger.LogInformation("Retrieval of root folders initiated for user {UserId}.", userId);
        var rootFolders = await folderRepository.GetRootFoldersAsync(userId);

        var rootFolderList = rootFolders.ToList();

        logger.LogInformation("Retrieval of root folders successful for user {UserId}. Found {FolderCount} folders in root.", userId, rootFolderList.Count);
        return rootFolderList.Select(FolderMapper.ToResponse);
    }

    /// <summary>
    /// Asynchronously renames a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be renamed.</param>
    /// <param name="request">The DTO containing the new name.</param>
    /// <param name="userId">The ID of the user requesting the renaming and who has access to it.</param>
    /// <exception cref="FolderNotFoundException">Thrown when the folder to be renamed cannot be found.</exception>
    /// <exception cref="EmptyNameException">Thrown when the new folder name is empty or whitespace.</exception>
    /// <exception cref="NameTooLongException">Thrown when the new folder name exceeds the maximum allowed length.</exception>
    /// <exception cref="InvalidCharactersException">Thrown when the new folder name contains invalid characters.</exception>
    public async Task RenameFolderAsync(int folderId, RenameFolderRequest request, string userId)
    {
        logger.LogInformation("Folder renaming initiated for user {UserId}. ID of folder to be renamed: {FolderId}", userId, folderId);

        bool folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("Folder renaming failed. The folder {FolderId} could not be found for user {UserId}", userId, folderId);
            throw new FolderNotFoundException(folderId);
        }

        ValidateRenameFolderRequest(request, userId);

        await folderRepository.RenameFolderAsync(folderId, request.NewName, userId);

        logger.LogInformation("Folder {FolderId} renaming successful for user {UserId}. Renamed to {FolderName}",
            folderId, userId, request.NewName);
    }

    /// <summary>
    /// Asynchronously deletes a specific folder in addition to its content (subfolders and files) using soft deletion.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be removed.</param>
    /// <param name="userId">The ID of the user requesting the deletion and who has access to it.</param>
    /// <exception cref="FolderNotFoundException">Thrown when the folder associated with the file cannot be found.</exception>
    public async Task DeleteFolderAsync(int folderId, string userId)
    {
        logger.LogInformation("Folder deletion initiated for user {UserId}. ID of folder to be deleted: {FolderId}", userId, folderId);
        var folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("Folder deletion failed for user {UserId}. Folder with ID: {FolderId} could not be found", userId, folderId);
            throw new FolderNotFoundException(folderId);
        }

        await folderRepository.DeleteFolderAsync(folderId, userId);

        logger.LogInformation("Folder deletion for user {UserId} successful. Folder with ID {FolderId} removed", userId, folderId);
    }

    #region Helper methods

    private async Task ValidateCreateFolderRequest(CreateFolderRequest request, string userId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Name is null or has whitespace.", userId);
            throw new EmptyNameException("folder");
        }

        if (request.Name.Length > ValidationConstants.MaxFolderNameLength)
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Name too long ({NameLength} chars)",
                userId, request.Name.Length);
            throw new NameTooLongException("folder", ValidationConstants.MaxFolderNameLength);
        }

        if (request.Name.Any(ValidationConstants.InvalidNameCharacters.Contains))
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Invalid characters in folder name: {FolderName}", userId, request.Name);
            throw new InvalidCharactersException("folder");
        }

        if (request.ParentFolderId != null)
        {
            logger.LogDebug("Checking if parent folder {ParentFolderId} exists for user {UserId}", request.ParentFolderId, userId);

            bool parentFolderExists = await folderRepository.FolderExistsAsync(request.ParentFolderId.Value, userId);

            if (!parentFolderExists)
            {
                logger.LogWarning("Folder creation failed for user {UserId}. The parent folder {ParentFolderId} could not be found", userId, request.ParentFolderId);
                throw new FolderNotFoundException(request.ParentFolderId.Value);
            }
        }
    }

    private void ValidateRenameFolderRequest(RenameFolderRequest request, string userId)
    {
        if (string.IsNullOrWhiteSpace(request.NewName))
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Empty folder name provided", userId);
            throw new EmptyNameException("folder");
        }

        if (request.NewName.Length > ValidationConstants.MaxFolderNameLength)
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Name too long ({NameLength} chars)",
                userId, request.NewName.Length);
            throw new NameTooLongException("folder", ValidationConstants.MaxFolderNameLength);
        }

        if (request.NewName.Any(ValidationConstants.InvalidNameCharacters.Contains))
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Invalid characters in folder name: {FolderName}", userId, request.NewName);
            throw new InvalidCharactersException("folder");
        }
    }

    #endregion
}