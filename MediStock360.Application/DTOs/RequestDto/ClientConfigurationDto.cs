using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class ClientConfigurationDto
    {
        [Required]
        public long ClientId { get; set; }

        public string? CompanyName { get; set; }

        public int MaxStores { get; set; } = 1;

        public int MaxUsers { get; set; } = 5;

        public string InvoicePrefix { get; set; } = "INV-";

        public int ExpiryAlertDays { get; set; } = 60;

        public bool MultiBranchEnabled { get; set; } = false;

        public bool WhatsAppNotificationsEnabled { get; set; } = false;

        public bool CreditSalesEnabled { get; set; } = true;

        public bool AutoBackupEnabled { get; set; } = true;

        public Dictionary<string, string> CustomSettings { get; set; } = new Dictionary<string, string>();
    }
}

