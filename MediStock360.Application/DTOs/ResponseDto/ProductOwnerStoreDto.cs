using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ProductOwnerStoreDto
    {
        public long StoreId { get; set; }
        public Guid StoreKey { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string StoreCode { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public byte StoreType { get; set; }
        public string StoreTypeText { get; set; } = string.Empty;
        public string? StoreEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DrugLicenseNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

