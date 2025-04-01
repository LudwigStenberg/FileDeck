using Microsoft.EntityFrameworkCore;

namespace FileDeck.api.Data;
public class FileDeckDbContext : DbContext
{
    // DbSet properties - represents the tables in the database
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<FolderEntity> Folders { get; set; }

    // Constructor that accepts DbContextOptions
    // How EF Core passes configuration from Program.cs to the context
    public FileDeckDbContext(DbContextOptions<FileDeckDbContext> options)
        : base(options)
    {

    }

    // This method is called when the model for a derived context is being created
    // The place where we configure relationships, constraints, etc.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration: File Entity
        modelBuilder.Entity<FileEntity>(entity =>
        {
            entity.ToTable("Files");
            entity.HasKey(file => file.Id);

            // Configure the relationship with Folder:
            entity.HasOne(file => file.Folder)          // Each file can belong to one folder
                .WithMany(folder => folder.Files)       // Each folder can have many files
                .HasForeignKey(file => file.FolderId)   // FolderId is the foreign key
                .OnDelete(DeleteBehavior.SetNull);      // When a folder is deleted: set FolderId to null
        });

        modelBuilder.Entity<FolderEntity>(entity =>
        {
            entity.ToTable("Folders");
            entity.HasKey(folder => folder.Id);

            // Configure the self-referencing relationship (folders can contain folders)
            entity.HasOne(folder => folder.ParentFolder)            // Each folder can have one parent folder 
                .WithMany(parentFolder => parentFolder.SubFolders)  // Each parentfolder can have many subfolders
                .HasForeignKey(folder => folder.ParentFolderId)     // ParentFolderId is the foreign key
                .OnDelete(DeleteBehavior.Restrict);                 // Prevent deleting a folder if it contains subfolders
        });
    }
}