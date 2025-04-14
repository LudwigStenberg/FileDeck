using System;
using System.Linq;
using System.Threading.Tasks;
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
        Console.WriteLine($"Repository - Folder User ID before save: '{folder.UserId}'");
        context.Folders.Add(folder);
        await context.SaveChangesAsync();

        return folder;
    }

    public async Task<bool> FolderExistsAsync(int parentFolderId, string userId)
    {
        return await context.Folders
            .AnyAsync(f => f.Id == parentFolderId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<FolderEntity?> GetFolderByIdAsync(int folderId, string userId)
    {
        return await context.Folders
            .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<bool> RenameFolderAsync(int folderId, string newName, string userId)
    {
        var folder = await context.Folders
            .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);

        if (folder == null)
        {
            return false;
        }

        folder.Name = newName;
        folder.LastModifiedDate = DateTime.UtcNow;

        int affectedRows = await context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<bool> DeleteFolderAsync(int folderId, string userId)
    {
        var folder = await context.Folders
            .SingleOrDefaultAsync(f => f.Id == folderId && f.UserId == userId && !f.IsDeleted);

        if (folder == null)
        {
            return false;
        }

        folder.IsDeleted = true;
        folder.LastModifiedDate = DateTime.UtcNow;

        int affectedRows = await context.SaveChangesAsync();

        return affectedRows > 0;

    }
}