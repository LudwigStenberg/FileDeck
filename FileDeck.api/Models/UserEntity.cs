
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace FileDeck.api.Models;
public class UserEntity : IdentityUser
{
    // Virtual is for lazy-loading in Entity Framework Core
    public virtual ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
    public virtual ICollection<FolderEntity> Folders { get; set; } = new List<FolderEntity>();

    private UserEntity() { }

}