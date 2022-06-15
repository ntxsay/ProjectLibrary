using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibApi.Models.Local.SQLite
{
    public partial class LibrarySqLiteDbContext : DbContext
    {
        public LibrarySqLiteDbContext()
        {
            Database.EnsureCreated();
        }

        public LibrarySqLiteDbContext(DbContextOptions<LibrarySqLiteDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tbook> Tbooks { get; set; } = null!;
        public virtual DbSet<TbookClassification> TbookClassifications { get; set; } = null!;
        public virtual DbSet<TbookCollection> TbookCollections { get; set; } = null!;
        public virtual DbSet<TbookContactRoleConnector> TbookContactRoleConnectors { get; set; } = null!;
        public virtual DbSet<TbookEtat> TbookEtats { get; set; } = null!;
        public virtual DbSet<TbookExemplary> TbookExemplaries { get; set; } = null!;
        public virtual DbSet<TbookFormat> TbookFormats { get; set; } = null!;
        public virtual DbSet<TbookIdentification> TbookIdentifications { get; set; } = null!;
        public virtual DbSet<TbookOtherTitle> TbookOtherTitles { get; set; } = null!;
        public virtual DbSet<TbookPret> TbookPrets { get; set; } = null!;
        public virtual DbSet<TbookReading> TbookReadings { get; set; } = null!;
        public virtual DbSet<Tcollection> Tcollections { get; set; } = null!;
        public virtual DbSet<Tcontact> Tcontacts { get; set; } = null!;
        public virtual DbSet<Tlibrary> Tlibraries { get; set; } = null!;
        public virtual DbSet<TlibraryCategorie> TlibraryCategories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=LibraryDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tbook>(entity =>
            {
                entity.ToTable("TBook");

                entity.HasIndex(e => e.Guid, "IX_TBook_Guid")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "IX_TBook_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdCategorieNavigation)
                    .WithMany(p => p.Tbooks)
                    .HasForeignKey(d => d.IdCategorie)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.Tbooks)
                    .HasForeignKey(d => d.IdLibrary);
            });

            modelBuilder.Entity<TbookClassification>(entity =>
            {
                entity.ToTable("TBookClassification");

                entity.HasIndex(e => e.Id, "IX_TBookClassification_Id")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AtelAge).HasColumnName("ATelAge");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookClassification)
                    .HasForeignKey<TbookClassification>(d => d.Id);
            });

            modelBuilder.Entity<TbookCollection>(entity =>
            {
                entity.ToTable("TBookCollections");

                entity.HasIndex(e => e.Id, "IX_TBookCollections_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookCollections)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdCollectionNavigation)
                    .WithMany(p => p.TbookCollections)
                    .HasForeignKey(d => d.IdCollection);
            });

            modelBuilder.Entity<TbookContactRoleConnector>(entity =>
            {
                entity.ToTable("TBookContactRoleConnector");

                entity.HasIndex(e => e.Id, "IX_TBookContactRoleConnector_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookContactRoleConnectors)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdContactNavigation)
                    .WithMany(p => p.TbookContactRoleConnectors)
                    .HasForeignKey(d => d.IdContact);
            });

            modelBuilder.Entity<TbookEtat>(entity =>
            {
                entity.ToTable("TBookEtat");

                entity.HasIndex(e => e.Id, "IX_TBookEtat_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdBookExemplaryNavigation)
                    .WithMany(p => p.TbookEtats)
                    .HasForeignKey(d => d.IdBookExemplary);
            });

            modelBuilder.Entity<TbookExemplary>(entity =>
            {
                entity.ToTable("TBookExemplary");

                entity.HasIndex(e => e.Id, "IX_TBookExemplary_Id")
                    .IsUnique();

                entity.Property(e => e.IsVisible).HasDefaultValueSql("1");

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookExemplaries)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdContactSourceNavigation)
                    .WithMany(p => p.TbookExemplaries)
                    .HasForeignKey(d => d.IdContactSource)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TbookFormat>(entity =>
            {
                entity.ToTable("TBookFormat");

                entity.HasIndex(e => e.Id, "IX_TBookFormat_Id")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookFormat)
                    .HasForeignKey<TbookFormat>(d => d.Id);
            });

            modelBuilder.Entity<TbookIdentification>(entity =>
            {
                entity.ToTable("TBookIdentification");

                entity.HasIndex(e => e.Id, "IX_TBookIdentification_Id")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Asin).HasColumnName("ASIN");

                entity.Property(e => e.Isbn).HasColumnName("ISBN");

                entity.Property(e => e.Isbn10).HasColumnName("ISBN10");

                entity.Property(e => e.Isbn13).HasColumnName("ISBN13");

                entity.Property(e => e.Issn).HasColumnName("ISSN");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookIdentification)
                    .HasForeignKey<TbookIdentification>(d => d.Id);
            });

            modelBuilder.Entity<TbookOtherTitle>(entity =>
            {
                entity.ToTable("TBookOtherTitle");

                entity.HasIndex(e => e.Id, "IX_TBookOtherTitle_Id")
                    .IsUnique();

                entity.HasIndex(e => e.Title, "IX_TBookOtherTitle_Title")
                    .IsUnique();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookOtherTitles)
                    .HasForeignKey(d => d.IdBook);
            });

            modelBuilder.Entity<TbookPret>(entity =>
            {
                entity.ToTable("TBookPret");

                entity.HasIndex(e => e.Id, "IX_TBookPret_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdBookExemplaryNavigation)
                    .WithMany(p => p.TbookPrets)
                    .HasForeignKey(d => d.IdBookExemplary);

                entity.HasOne(d => d.IdContactNavigation)
                    .WithMany(p => p.TbookPrets)
                    .HasForeignKey(d => d.IdContact);

                entity.HasOne(d => d.IdEtatAfterNavigation)
                    .WithMany(p => p.TbookPretIdEtatAfterNavigations)
                    .HasForeignKey(d => d.IdEtatAfter)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IdEtatBeforeNavigation)
                    .WithMany(p => p.TbookPretIdEtatBeforeNavigations)
                    .HasForeignKey(d => d.IdEtatBefore);
            });

            modelBuilder.Entity<TbookReading>(entity =>
            {
                entity.ToTable("TBookReading");

                entity.HasIndex(e => e.Id, "IX_TBookReading_Id")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookReading)
                    .HasForeignKey<TbookReading>(d => d.Id);
            });

            modelBuilder.Entity<Tcollection>(entity =>
            {
                entity.ToTable("TCollection");

                entity.HasIndex(e => e.Id, "IX_TCollection_Id")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "IX_TCollection_Name")
                    .IsUnique();

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.Tcollections)
                    .HasForeignKey(d => d.IdLibrary);
            });

            modelBuilder.Entity<Tcontact>(entity =>
            {
                entity.ToTable("TContact");

                entity.HasIndex(e => e.Guid, "IX_TContact_Guid")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "IX_TContact_Id")
                    .IsUnique();

#warning A ne pas oublier
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();


            });

            modelBuilder.Entity<Tlibrary>(entity =>
            {
                entity.ToTable("TLibrary");

                entity.HasIndex(e => e.Guid, "IX_TLibrary_Guid")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "IX_TLibrary_Id")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "IX_TLibrary_Name")
                    .IsUnique();

#warning A ne pas oublier
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<TlibraryCategorie>(entity =>
            {
                entity.ToTable("TLibraryCategorie");

                entity.HasIndex(e => e.Id, "IX_TLibraryCategorie_Id")
                    .IsUnique();

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.TlibraryCategories)
                    .HasForeignKey(d => d.IdLibrary);

                entity.HasOne(d => d.IdParentCategorieNavigation)
                    .WithMany(p => p.InverseIdParentCategorieNavigation)
                    .HasForeignKey(d => d.IdParentCategorie)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
