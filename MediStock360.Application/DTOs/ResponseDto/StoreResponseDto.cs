using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class StoreResponseDto
    {
        public long StoreId { get; set; }
        public Guid StoreKey { get; set; }
        public long ClientId { get; set; }
        public string StoreCode { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public byte StoreType { get; set; }
        public string? OwnerName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        public string? GSTNumber { get; set; }
        public string? DrugLicenseNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int? CityId { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; }
    }
}

