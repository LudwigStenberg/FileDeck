using FileDeck.api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileDeck.api.Data;

public class FileDeckDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<FolderEntity> Folders { get; set; }

    public FileDeckDbContext(DbContextOptions<FileDeckDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<FileEntity>(entity =>
        {
            entity.ToTable("Files");
            entity.HasKey(file => file.Id);


            entity.HasOne(file => file.Folder)          // Each file can belong to one folder
                .WithMany(folder => folder.Files)       // Each folder can have many files
                .HasForeignKey(file => file.FolderId)   // FolderId is the foreign key
                .OnDelete(DeleteBehavior.SetNull);      // When a folder is deleted: set FolderId to null


            entity.HasOne(file => file.User)            // Each file can belong to one User
                .WithMany(user => user.Files)           // Each user can have many files
                .HasForeignKey(file => file.UserId)     // UserId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);      // When a user is deleted: delete all their files

        });

        modelBuilder.Entity<FolderEntity>(entity =>
        {
            entity.ToTable("Folders");
            entity.HasKey(folder => folder.Id);

            entity.HasOne(folder => folder.ParentFolder)            // Each folder can have one parent folder 
                .WithMany(parentFolder => parentFolder.SubFolders)  // Each parentfolder can have many subfolders
                .HasForeignKey(folder => folder.ParentFolderId)     // ParentFolderId is the foreign key
                .OnDelete(DeleteBehavior.Restrict);                 // Prevent deleting a folder if it contains subfolders


            entity.HasOne(folder => folder.User)                    // Each folder can have one User
                .WithMany(user => user.Folders)                     // Each user can have many folders
                .HasForeignKey(folder => folder.UserId)             // UserId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);                  // When a user is deleted: delete all their folders.

        });
    }
}