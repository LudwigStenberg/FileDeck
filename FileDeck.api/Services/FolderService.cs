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
    /// Creates a new folder based on the information provided in the input DTO.
    /// </summary>
    /// <param name="request">The DTO used to create the new folder. Contains the name and the ID of the parent folder, if there is one.</param>
    /// <param name="userId">The ID of the user requesting the folder to be created and who will have access to it.</param>
    /// <returns>A FolderResponse with additional data that was created during construction.</returns>
    /// <exception cref="ArgumentException">The exceptions thrown when the arguments do not fulfill either one of: Name.Length, no invalid characters or if the parent folder doesn't exist despite request.ParentFolderId being populated.</exception>
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
    /// Retrieves a specific folder and its information.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be retrieved.</param>
    /// <param name="userId">The ID of the user requesting the folder and who will have access to it.</param>
    /// <returns>A FolderResponse which contains the information on the folder that has been retrieved. If no folder is found, returns null.</returns>
    public async Task<FolderResponse?> GetFolderByIdAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of folder {FolderId} for user {UserId} initiated", folderId, userId);

        var folder = await folderRepository.GetFolderByIdAsync(folderId, userId);
        if (folder == null)
        {
            logger.LogWarning("Folder retrieval failed. The folder {FolderId} could not be found for user {UserId}", folderId, userId);
            return null;
        }

        logger.LogInformation("Folder retrieval successful for user {UserId}. Folder {FolderId}, {FolderName} was found.",
            userId, folderId, folder.Name);
        return FolderMapper.ToResponse(folder);
    }

    /// <summary>
    /// Retrieves all folders belonging to a specific user.
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
    /// Retrieves the subfolders within a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be searched within.</param>
    /// <param name="userId">The ID of the user requesting to see the subfolders and who has access to it.</param>
    /// <returns>A list containing FolderResponse DTOs. It can be empty.</returns>
    public async Task<IEnumerable<FolderResponse>> GetSubfoldersAsync(int folderId, string userId)
    {
        logger.LogInformation("Retrieval of subfolders initiated for user {UserId}. ID of the folder to be searched within: {FolderId}", userId, folderId);

        var subfolders = await folderRepository.GetSubfoldersAsync(folderId, userId);

        var subfolderList = subfolders.ToList();

        logger.LogInformation("Retrieval of subfolders successful for user {UserId}. Found {SubfolderCount}.", userId, subfolderList.Count);
        return subfolderList.Select(FolderMapper.ToResponse);
    }

    /// <summary>
    /// Retrieves the root folders for a specific user. 
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
    /// Renames a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be renamed.</param>
    /// <param name="request">The DTO containing the new name.</param>
    /// <param name="userId">The ID of the user requesting the renaming and who has access to it.</param>
    /// <returns>A boolean to indicate a successful or unsuccessful operation.</returns>
    /// <exception cref="ArgumentException">The exceptions thrown when one of the arguments do not meet the validation requirements.</exception>
    public async Task<bool> RenameFolderAsync(int folderId, RenameFolderRequest request, string userId)
    {
        logger.LogInformation("Folder renaming initiated for user {UserId}. ID of folder to be renamed: {FolderId}", userId, folderId);

        bool folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("Folder renaming failed. The folder {FolderId} could not be found for user {UserId}", userId, folderId);
            return false;
        }

        ValidateRenameFolderRequest(request, userId);

        var success = await folderRepository.RenameFolderAsync(folderId, request.NewName, userId);

        if (!success)
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. The folder {FolderId} could not be renamed.", userId, folderId);
            return false;
        }

        logger.LogInformation("Folder {FolderId} renaming successful for user {UserId}. Renamed to {FolderName}", folderId, userId, request.NewName);
        return true;
    }

    /// <summary>
    /// Deletes a specific folder in addition to its content (subfolders and files) using soft deletion.
    /// </summary>
    /// <param name="folderId">The ID of the folder to be removed.</param>
    /// <param name="userId">The ID of the user requesting the deletion and who has access to it.</param>
    /// <returns>A boolean to indicate a successful or unsuccessful operation.</returns>
    public async Task<bool> DeleteFolderAsync(int folderId, string userId)
    {
        logger.LogInformation("Folder deletion initiated for user {UserId}. ID of folder to be deleted: {FolderId}", userId, folderId);
        var folderExists = await folderRepository.FolderExistsAsync(folderId, userId);

        if (!folderExists)
        {
            logger.LogWarning("Folder deletion failed for user {UserId}. Folder with ID: {FolderId} could not be found", userId, folderId);
            return false;
        }

        bool success = await folderRepository.DeleteFolderAsync(folderId, userId);

        if (!success)
        {
            logger.LogWarning("Folder deletion failed for user {UserId}. Folder with ID {FolderId} could not be deleted.", userId, folderId);
            return false;
        }

        logger.LogInformation("Folder deletion for user {UserId} successful. Folder with ID {FolderId} removed", userId, folderId);
        return true;
    }

    #region Helper methods

    private async Task ValidateCreateFolderRequest(CreateFolderRequest request, string userId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Name is null or has whitespace.", userId);
            throw new ArgumentException("Folder name cannot be empty.");
        }

        if (request.Name.Length > 50)
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Name too long ({NameLength} chars)",
                userId, request.Name.Length);
            throw new ArgumentException("Folder name cannot be longer than 50 characters.");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (request.Name.Any(invalidChars.Contains))
        {
            logger.LogWarning("Folder creation failed for user {UserId}. Invalid characters in folder name: {FolderName}", userId, request.Name);
            throw new ArgumentException("Folder name contains invalid characters.");
        }


        if (request.ParentFolderId != null)
        {
            logger.LogDebug("Checking if parent folder {ParentFolderId} exists for user {UserId}", request.ParentFolderId, userId);

            bool parentFolderExists = await folderRepository.FolderExistsAsync(request.ParentFolderId.Value, userId);

            if (!parentFolderExists)
            {
                logger.LogWarning("Folder creation failed for user {UserId}. The parent folder {ParentFolderId} could not be found", userId, request.ParentFolderId);
                throw new ArgumentException("Parent folder does not exist or you do not have access to it.");
            }
        }
    }

    private void ValidateRenameFolderRequest(RenameFolderRequest request, string userId)
    {
        if (string.IsNullOrWhiteSpace(request.NewName))
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Empty folder name provided", userId);
            throw new ArgumentException("Folder name cannot be empty.");
        }

        if (request.NewName.Length > 50)
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Name too long ({NameLength} chars)",
                userId, request.NewName.Length);
            throw new ArgumentException("Folder name cannot be longer than 50 characters.");
        }

        string invalidChars = "\\/:*?\"<>|";
        if (request.NewName.Any(invalidChars.Contains))
        {
            logger.LogWarning("Folder renaming failed for user {UserId}. Invalid characters in folder name: {FolderName}", userId, request.NewName);
            throw new ArgumentException("Folder name contains invalid characters.");
        }
    }

    #endregion
}