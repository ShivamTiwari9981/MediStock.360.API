using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class PaymentConfigurationDto
    {
        public long PaymentConfigurationId { get; set; }

        public int PaymentMethodId { get; set; }
        public string ConfigurationCode { get; set; } = string.Empty;
        public string ConfigurationName { get; set; } = string.Empty;
        public string? UpiId { get; set; }
        public string? UpiNumber { get; set; }
        public string? QRCodeUrl { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BankName { get; set; }
        public string? PaymentModeLabel { get; set; }
        public string? BadgeText { get; set; }
        public string? CollectionInstructions { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactLabel { get; set; }
        public string? Instructions { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }
    }
}
