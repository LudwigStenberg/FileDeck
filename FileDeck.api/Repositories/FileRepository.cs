
using System;
using System.Collections.Generic;
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

    public Task<IEnumerable<FileEntity>> GetFilesInFolderAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> FileExistsAsync(int fileId, string userId)
    {
        throw new System.NotImplementedException();
    }
}