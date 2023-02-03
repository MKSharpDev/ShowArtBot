using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShowArtBot.Hermitage;

public partial class ArtGalleryContext : DbContext
{
    public ArtGalleryContext()
    {
    }

    public ArtGalleryContext(DbContextOptions<ArtGalleryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArtObject> ArtObjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=G:\\HermitageBot\\ArtGallery.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArtObject>(entity =>
        {
            entity.Property(e => e.ArtObjectId).HasColumnName("ArtObjectID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
