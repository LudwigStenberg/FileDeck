using FileDeck.api.Data;
using FileDeck.api.Models;
using Microsoft.EntityFrameworkCore;

namespace FileDeck.api.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileDeckDbContext context;
    public FileRepository(FileDeckDbContext context)
    {
        this.context = context;
    }
    public async Task<FileEntity> CreateFileAsync(FileEntity file)
    {
        context.Files.Add(file);
        await context.SaveChangesAsync();
        return file;
    }

    public async Task<FileEntity?> GetFileByIdAsync(int fileId, string userId)
    {
        return await context.Files
            .AsNoTracking()
            .SingleOrDefaultAsync(f => f.Id == fileId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<IEnumerable<FileEntity>> GetRootFilesAsync(string userId)
    {
        return await context.Files
            .Where(f => f.FolderId == null && f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<FileEntity>> GetFilesInFolderAsync(int folderId, string userId)
    {
        return await context.Files
            .Where(f => f.FolderId == folderId && f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        var file = await context.Files
            .SingleOrDefaultAsync(f => f.Id == fileId && f.UserId == userId && !f.IsDeleted);

        if (file == null)
        {
            return false;
        }

        file.IsDeleted = true;
        file.LastModifiedDate = DateTime.UtcNow;

        int affectedRows = await context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<DeletionResult> HardDeleteOldFilesAsync(DateTime cutOffDate)
    {
        var deletionResult = new DeletionResult();

        var fileIds = await context.Files
                    .Where(f => f.IsDeleted && f.LastModifiedDate < cutOffDate)
                    .Select(f => f.Id)
                    .ToListAsync();

        var deletedFiles = await context.Files
            .Where(f => f.IsDeleted && f.LastModifiedDate < cutOffDate)
            .ExecuteDeleteAsync();

        deletionResult.Ids.AddRange(fileIds);
        deletionResult.Count = deletedFiles;

        return deletionResult;

    }

    public async Task<bool> FileExistsAsync(int fileId, string userId)
    {
        return await context.Files.AnyAsync(f => f.Id == fileId && f.UserId == userId && !f.IsDeleted);
    }
}