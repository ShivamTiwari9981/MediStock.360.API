using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class PaymentConfiguration
{
    public long PaymentConfigurationId { get; set; }

    public int PaymentMethodId { get; set; }

    public string ConfigurationCode { get; set; } = null!;

    public string ConfigurationName { get; set; } = null!;

    public string? UpiId { get; set; }

    public string? UpiNumber { get; set; }

    public string? QrcodeUrl { get; set; }

    public string? AccountHolderName { get; set; }

    public string? AccountNumber { get; set; }

    public string? Ifsccode { get; set; }

    public string? BankName { get; set; }

    public string? PaymentModeLabel { get; set; }

    public string? BadgeText { get; set; }

    public string? CollectionInstructions { get; set; }

    public string? ContactNumber { get; set; }

    public string? ContactLabel { get; set; }

    public string? Instructions { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
