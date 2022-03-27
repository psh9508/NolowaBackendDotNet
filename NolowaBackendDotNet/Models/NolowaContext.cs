using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class NolowaContext : DbContext
    {
        private IConfiguration _configuration;

        public NolowaContext()
        {
        }

        public NolowaContext(DbContextOptions<NolowaContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Follower> Followers { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<ProfileImage> ProfileImages { get; set; }
        public virtual DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var test = _configuration.GetConnectionString("NolowaContext");
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-4P0C2JG;Initial Catalog=Nolowa;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Korean_Wansung_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "EMAIL")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UNIQUE_EMAIL")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("ACCOUNT_ID");

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("ACCOUNT_NAME");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasColumnName("INSERT_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Password)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.ProfileImageId).HasColumnName("PROFILE_IMAGE_ID");

                entity.HasOne(d => d.ProfileImage)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ProfileImageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PROFILE_IMAGE_ID");
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.ToTable("Follower");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DestinationAccountId).HasColumnName("DESTINATION_ACCOUNT_ID");

                entity.Property(e => e.SourceAccountId).HasColumnName("SOURCE_ACCOUNT_ID");

                entity.HasOne(d => d.DestinationAccount)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(d => d.DestinationAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FOLLOWER__SOURCE__412EB0B6");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasColumnName("INSERT_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("MESSAGE");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__POST__ACCOUNT_ID__398D8EEE");
            });

            modelBuilder.Entity<ProfileImage>(entity =>
            {
                entity.ToTable("Profile_Image");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FileHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("FILE_HASH");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<SearchHistory>(entity =>
            {
                entity.ToTable("Search_History");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasColumnName("INSERT_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Keyword)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("KEYWORD");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.SearchHistories)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__SEARCH_HI__ACCOU__3D5E1FD2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
