
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            return false; // File not found (or already deleted)
        }

        file.IsDeleted = true;
        file.LastModifiedDate = DateTime.UtcNow;

        int affectedRows = await context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public Task<bool> FileExistsAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }
}