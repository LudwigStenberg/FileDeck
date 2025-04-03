using System.Threading.Tasks;
using FileDeck.api.Data;
using FileDeck.api.Models;
using FileDeck.api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> FolderExistsAsync(int parentFolderId, string userId)
    {
        return await context.Folders
            .AnyAsync(f => f.Id == parentFolderId && f.UserId == userId && !f.IsDeleted);
    }

    public async Task<FolderEntity?> GetFolderById(int folderId)
    {
        return await context.Folders.SingleOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
    }
}