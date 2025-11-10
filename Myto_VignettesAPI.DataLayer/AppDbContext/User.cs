using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

[Table("users")]
[Index("Email", Name = "email", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string? Name { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("mobile")]
    [StringLength(30)]
    public string? Mobile { get; set; }

    [Column("password_hash")]
    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [Column("preferred_language")]
    [StringLength(5)]
    public string? PreferredLanguage { get; set; }

    [Column("is_email_verified")]
    public bool? IsEmailVerified { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
