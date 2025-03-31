using System;
using System.Collections.Generic;

public class FolderEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<FileEntity> Files { get; set; }
    public DateTime CreatedDate { get; set; }


    // Relationship to other Folders
    public int? ParentFolderId { get; set; }
    public FolderEntity ParentFolder { get; set; }
    public ICollection<FolderEntity> SubFolders { get; set; }

    // Relationship to Files (One-to-Many)
    public ICollection<FileEntity> Files { get; set; }

    // Relationship to User (many-to-one, many folders - one user)
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}