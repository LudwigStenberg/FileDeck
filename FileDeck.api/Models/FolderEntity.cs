using System;
using System.Collections.Generic;

namespace FileDeck.api.Models;
public class FolderEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false; // Soft Delete


    // Relationship to other Folders (self-referencing)
    public int? ParentFolderId { get; set; } // FK, null means that it is a "root" folder
    public FolderEntity? ParentFolder { get; set; } // NavProp to parent
    public ICollection<FolderEntity> SubFolders { get; set; } = new List<FolderEntity>(); // NavProp to children

    // Relationship to Files (One-to-Many)
    public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();

    // Relationship to User (many-to-one, many folders - one user)
    public string UserId { get; set; } = null!; // FK
    public UserEntity User { get; set; } = null!; // NavProp

    public FolderEntity() { }
}