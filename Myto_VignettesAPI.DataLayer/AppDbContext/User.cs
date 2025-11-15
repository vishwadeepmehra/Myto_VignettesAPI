using System;
using System.Collections.Generic;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

public partial class User
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? PasswordHash { get; set; }

    public string? PreferredLanguage { get; set; }

    public bool? IsEmailVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Role { get; set; } = null!;

    public bool? IsInvited { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
