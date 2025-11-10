using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

[Table("vehicles")]
[Index("UserId", Name = "user_id")]
public partial class Vehicle
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("country_code")]
    [StringLength(5)]
    public string CountryCode { get; set; } = null!;

    [Column("registration_number")]
    [StringLength(50)]
    public string RegistrationNumber { get; set; } = null!;

    [Column("vehicle_category")]
    [StringLength(50)]
    public string VehicleCategory { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Vehicles")]
    public virtual User User { get; set; } = null!;
}
