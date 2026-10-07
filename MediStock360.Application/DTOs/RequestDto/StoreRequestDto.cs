

namespace MediStock360.Application.DTOs.RequestDto
{
    public class StoreRequestDto
    {
        public long StoreId { get; set; }

        public long ClientId { get; set; }

        public Guid StoreKey { get; set; }

        public string StoreCode { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public byte StoreType { get; set; }

        public string? StoreEmail { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AlternatePhoneNumber { get; set; }

        public string? DrugLicenseNumber { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public int? CityId { get; set; }

        public int? CountryId { get; set; }

        public int? StateId { get; set; }

        public string? PostalCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool IsOnBording { get; set; }

        public bool IsActive { get; set; }
        public long UserId { get; set; }
    }
}
