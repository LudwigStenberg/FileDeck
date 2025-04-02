

using System.Threading.Tasks;
using FileDeck.api.Models;

namespace FileDeck.api.Repositories.Interfaces;
public interface IFolderRepository
{
    Task<FolderEntity> CreateFolder(FolderEntity folder);
    // Task<FolderEntity> GetById(FolderEntity folder);


}