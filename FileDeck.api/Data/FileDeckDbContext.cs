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
            entity.HasKey(e => e.Id);

            // Configure the relationship with Folder:
            entity.HasOne(e => e.Folder)
                .WithMany(f => f.Files)
                .HasForeignKey(e => e.FolderId)
                .OnDelete(DeleteBehavior.SetNull); // When a folder is deleted: set FolderId to null
        });

        modelBuilder.Entity<FolderEntity>(entity =>
        {
            entity.ToTable("Folders");
            entity.HasKey(e => e.Id);

            // Configure the self-referencing relationship (folders can contain folders)
            entity.HasOne(e => e.ParentFolder)
                .WithMany(f => f.SubFolders)
                .HasForeignKey(e => e.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a folder if it contains subfolders
        });
    }



}