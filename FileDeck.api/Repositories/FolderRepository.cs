using System.Threading.Tasks;
using FileDeck.api.Data;
using FileDeck.api.Models;
using FileDeck.api.Repositories.Interfaces;

public class FolderRepository : IFolderRepository
{

    private readonly FileDeckDbContext context;

    public FolderRepository(FileDeckDbContext context)
    {
        this.context = context;
    }

    public async Task<FolderEntity> CreateFolder(FolderEntity folder)
    {
        context.Folders.Add(folder);
        await context.SaveChangesAsync();

        return folder;
    }
}