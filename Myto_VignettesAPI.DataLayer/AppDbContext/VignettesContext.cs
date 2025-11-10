using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

public partial class VignettesContext : DbContext
{
    public VignettesContext()
    {
    }

    public VignettesContext(DbContextOptions<VignettesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsEmailVerified).HasDefaultValueSql("'0'");
            entity.Property(e => e.PreferredLanguage).IsFixedLength();
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vehicles_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
