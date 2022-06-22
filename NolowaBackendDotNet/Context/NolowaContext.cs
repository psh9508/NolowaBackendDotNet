﻿using System;
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

        public virtual DbSet<DirectMessage> DirectMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=ConnectionStrings:NolowaContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Korean_Wansung_CI_AS");

            modelBuilder.Entity<DirectMessage>(entity =>
            {
                entity.ToTable("DirectMessage");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.InsertTime)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("MESSAGE");

                entity.Property(e => e.ReceiverId).HasColumnName("RECEIVER_ID");

                entity.Property(e => e.SenderId).HasColumnName("SENDER_ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
