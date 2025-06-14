using FileDeck.api.Data;
using FileDeck.api.Models;
using FileDeck.api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileDeck.api.Repositories;

public class FolderRepository : IFolderRepository
{

    private readonly FileDeckDbContext context;

    public FolderRepository(FileDeckDbContext context)
    {
        this.context = context;
    }

    public async Task<FolderEntity> CreateFolderAsync(FolderEntity folder)
    {
        context.Folders.Add(folder);
        await context.SaveChangesAsync();

        return folder;
    }

    public async Task<bool> FolderExistsAsync(int folderId, string userId)
    {
        return await context.Folders
            .AnyAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<FolderEntity?> GetFolderByIdAsync(int folderId, string userId)
    {
        return await context.Folders
            .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<IEnumerable<FolderEntity>> GetAllFoldersAsync(string userId)
    {
        return await context.Folders
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<FolderEntity>> GetSubfoldersAsync(int folderId, string userId)
    {
        return await context.Folders
            .Where(f => f.ParentFolderId == folderId && f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<FolderEntity>> GetRootFoldersAsync(string userId)
    {
        return await context.Folders
            .Where(f => f.ParentFolderId == null && f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }

    public async Task RenameFolderAsync(int folderId, string newName, string userId)
    {
        var folder = await context.Folders
            .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);

        folder!.Name = newName;
        folder.LastModifiedDate = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteFolderAsync(int folderId, string userId)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var folder = await context.Folders
                .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);

            var childFolders = await GetAllChildFoldersAsync(folderId, userId);
            foreach (var childfolder in childFolders)
            {
                childfolder.IsDeleted = true;
                childfolder.LastModifiedDate = DateTime.UtcNow;
            }

            folder!.IsDeleted = true;
            folder.LastModifiedDate = DateTime.UtcNow;

            var folderIds = childFolders.Select(f => f.Id).ToList();
            folderIds.Add(folderId);

            var files = await context.Files
                .Where(f => f.FolderId.HasValue && folderIds.Contains(f.FolderId.Value) && f.UserId == userId && !f.IsDeleted)
                .ToListAsync();

            foreach (var file in files)
            {
                file.IsDeleted = true;
                file.LastModifiedDate = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<List<FolderEntity>> GetAllChildFoldersAsync(int parentFolderId, string userId)
    {
        var result = new List<FolderEntity>();

        var directChildren = await context.Folders
            .Where(f => f.ParentFolderId == parentFolderId && f.UserId == userId && !f.IsDeleted)
            .ToListAsync();

        result.AddRange(directChildren);

        foreach (FolderEntity child in directChildren)
        {
            var grandChildren = await GetAllChildFoldersAsync(child.Id, userId);
            result.AddRange(grandChildren);
        }

        return result;
    }

    public async Task<DeletionResult> HardDeleteOldFoldersAsync(DateTime cutOffDate)
    {
        var deletionResult = new DeletionResult();

        var folderIds = await context.Folders
            .Where(f => f.IsDeleted && f.LastModifiedDate < cutOffDate)
            .Select(f => f.Id)
            .ToListAsync();

        var deletedFolders = await context.Folders
            .Where(f => f.IsDeleted && f.LastModifiedDate < cutOffDate)
            .ExecuteDeleteAsync();

        deletionResult.Ids.AddRange(folderIds);
        deletionResult.Count = deletedFolders;

        return deletionResult;
    }
}