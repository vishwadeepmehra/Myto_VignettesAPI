using System;
using System.Collections.Generic;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

public partial class Vehicle
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string RegistrationNumber { get; set; } = null!;

    public string VehicleCategory { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<VignettePurchase> VignettePurchases { get; set; } = new List<VignettePurchase>();
}
