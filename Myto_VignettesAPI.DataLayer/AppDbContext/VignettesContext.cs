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

    public virtual DbSet<VignettePurchase> VignettePurchases { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=mytovignettes;user=vignettes_user;password=myto@123;treattinyasboolean=true", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IsEmailVerified)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_email_verified");
            entity.Property(e => e.IsInvited)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_invited");
            entity.Property(e => e.Mobile)
                .HasMaxLength(30)
                .HasColumnName("mobile");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PreferredLanguage)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("preferred_language");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'Customer'")
                .HasColumnType("enum('Admin','Customer')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("vehicles");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .HasColumnName("country_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(50)
                .HasColumnName("registration_number");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleCategory)
                .HasMaxLength(50)
                .HasColumnName("vehicle_category");

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vehicles_ibfk_1");
        });

        modelBuilder.Entity<VignettePurchase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("vignette_purchases");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.VehicleId, "vehicle_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.Commission)
                .HasPrecision(10, 2)
                .HasColumnName("commission");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .HasColumnName("country_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.FinalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("final_price");
            entity.Property(e => e.GovernmentReferenceId)
                .HasMaxLength(255)
                .HasColumnName("government_reference_id");
            entity.Property(e => e.GovernmentValidationStatus)
                .HasDefaultValueSql("'PENDING'")
                .HasColumnType("enum('PENDING','SUCCESS','FAILED')")
                .HasColumnName("government_validation_status");
            entity.Property(e => e.IsEmailSent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_email_sent");
            entity.Property(e => e.PaymentLink)
                .HasMaxLength(1000)
                .HasColumnName("payment_link");
            entity.Property(e => e.PaymentStatus)
                .HasDefaultValueSql("'PENDING'")
                .HasColumnType("enum('PENDING','SUCCESS','FAILED')")
                .HasColumnName("payment_status");
            entity.Property(e => e.PaymentTimestamp)
                .HasColumnType("datetime")
                .HasColumnName("payment_timestamp");
            entity.Property(e => e.PaymentTransactionId)
                .HasMaxLength(255)
                .HasColumnName("payment_transaction_id");
            entity.Property(e => e.ReceiptPdfUrl)
                .HasMaxLength(1000)
                .HasColumnName("receipt_pdf_url");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(50)
                .HasColumnName("registration_number");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ValidityEnd)
                .HasColumnType("datetime")
                .HasColumnName("validity_end");
            entity.Property(e => e.ValidityStart)
                .HasColumnType("datetime")
                .HasColumnName("validity_start");
            entity.Property(e => e.VehicleCategory)
                .HasMaxLength(50)
                .HasColumnName("vehicle_category");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.VignetteType)
                .HasColumnType("enum('1DAY','10DAY','30DAY','ANNUAL')")
                .HasColumnName("vignette_type");

            entity.HasOne(d => d.User).WithMany(p => p.VignettePurchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vignette_purchases_ibfk_1");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VignettePurchases)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vignette_purchases_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
