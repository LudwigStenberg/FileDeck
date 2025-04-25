using System;

namespace FileDeck.api.Models;
public class FileEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ContentType { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>(); // Should this be limited somehow, for performance?
    public long Size { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    // Relationship with FolderEntity (Many-to-One)
    // Foreign Key
    public int? FolderId { get; set; } // Can be null to support "root" files (not in any folder)
    // Navigation Property
    public FolderEntity? Folder { get; set; }

    // Relationship with User: Many-to-one (Many files, one User)
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}