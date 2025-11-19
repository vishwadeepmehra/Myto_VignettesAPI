using System;
using System.Collections.Generic;

namespace Myto_VignettesAPI.DataLayer.AppDbContext;

public partial class VignettePurchase
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long VehicleId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string RegistrationNumber { get; set; } = null!;

    public string VehicleCategory { get; set; } = null!;

    public string VignetteType { get; set; } = null!;

    public DateTime ValidityStart { get; set; }

    public DateTime ValidityEnd { get; set; }

    public decimal BasePrice { get; set; }

    public decimal Commission { get; set; }

    public decimal FinalPrice { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentLink { get; set; }

    public string? PaymentTransactionId { get; set; }

    public DateTime? PaymentTimestamp { get; set; }

    public string? ReceiptPdfUrl { get; set; }

    public bool? IsEmailSent { get; set; }

    public string? GovernmentReferenceId { get; set; }

    public string? GovernmentValidationStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
