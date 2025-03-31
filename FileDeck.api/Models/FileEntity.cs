public class FileEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }

    // Relationship with FolderEntity (Many-to-One)
    public int FolderId { get; set; }
    public FolderEntity Folder { get; set; }

    // Relationship with User: Many-to-one (Many files, one User)
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

}