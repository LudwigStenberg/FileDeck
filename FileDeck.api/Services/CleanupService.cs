
using FileDeck.api.Repositories;
using FileDeck.api.Repositories.Interfaces;
using Microsoft.Extensions.Options;

public class CleanupService : ICleanupService
{
    private readonly IFolderRepository folderRepository;
    private readonly IFileRepository fileRepository;
    private readonly ILogger<CleanupService> logger;
    private readonly IOptions<CleanupSettings> cleanupSettings;

    public CleanupService(IFolderRepository folderRepository, IFileRepository fileRepository, ILogger<CleanupService> logger, IOptions<CleanupSettings> cleanupSettings)
    {
        this.folderRepository = folderRepository;
        this.fileRepository = fileRepository;
        this.logger = logger;
        this.cleanupSettings = cleanupSettings;

    }
    public async Task CleanupSoftDeletedItemsAsync()
    {
        var retentionDays = cleanupSettings.Value.RetentionDays;
        var cutOffDate = DateTime.UtcNow.AddDays(-retentionDays);

        logger.LogInformation("Starting hard deletion of folders at {Time}.", DateTime.UtcNow);
        var deletedFolders = await folderRepository.HardDeleteOldFoldersAsync(cutOffDate);

        string folderIds = string.Join(", ", deletedFolders.Ids);
        logger.LogDebug("Deleted {FolderCount} folders with IDs: {FolderIds}", deletedFolders.Count, folderIds);

        logger.LogInformation("Starting hard deletion of files at {Time}.", DateTime.UtcNow);
        var deletedFiles = await fileRepository.HardDeleteOldFilesAsync(cutOffDate);

        var fileIds = string.Join(", ", deletedFiles.Ids);
        logger.LogDebug("Deleted {FileCount} files with IDs: {FileIds}.", deletedFiles.Count, fileIds);

        logger.LogInformation("Cleanup completed: deleted {TotalFolders} folders and {TotalFiles} files.", deletedFolders.Count, deletedFiles.Count);
    }
}