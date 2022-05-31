using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NolowaBackendDotNet.Models;

#nullable disable

namespace NolowaBackendDotNet.Context
{
    public partial class NolowaContext : DbContext
    {
        public NolowaContext()
        {
        }

        public NolowaContext(DbContextOptions<NolowaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Follower> Followers { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<ProfileImage> ProfileImages { get; set; }
        public virtual DbSet<ProfileInfo> ProfileInfos { get; set; }
        public virtual DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=connectionstrings:NolowaContext");
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

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("USER_ID");

                entity.HasOne(d => d.ProfileImage)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ProfileImageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PROFILE_IMAGE_ID");
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.ToTable("Follower");

                entity.HasIndex(e => new { e.SourceAccountId, e.DestinationAccountId }, "UQ__Follower__SOURCE,DEST")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DestinationAccountId).HasColumnName("DESTINATION_ACCOUNT_ID");

                entity.Property(e => e.SourceAccountId).HasColumnName("SOURCE_ACCOUNT_ID");

                entity.HasOne(d => d.DestinationAccount)
                    .WithMany(p => p.FollowerDestinationAccounts)
                    .HasForeignKey(d => d.DestinationAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DESTINATION_ACCOUNT");

                entity.HasOne(d => d.SourceAccount)
                    .WithMany(p => p.FollowerSourceAccounts)
                    .HasForeignKey(d => d.SourceAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SOURCE_ACCOUNT");
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
                entity.ToTable("ProfileImage");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FileHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("FILE_HASH");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<ProfileInfo>(entity =>
            {
                entity.ToTable("ProfileInfo");

                entity.Property(e => e.BackgroundImg).HasMaxLength(500);

                entity.Property(e => e.Message).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ProfileInfos)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfileInfo_Account");
            });

            modelBuilder.Entity<SearchHistory>(entity =>
            {
                entity.ToTable("SearchHistory");

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
