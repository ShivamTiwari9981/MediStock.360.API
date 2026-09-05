using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class Store
{
    public long StoreId { get; set; }

    public long ClientId { get; set; }

    public Guid StoreKey { get; set; }

    public string StoreCode { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public byte StoreType { get; set; }

    public string? OwnerName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AlternatePhoneNumber { get; set; }

    public string? Gstnumber { get; set; }

    public string? DrugLicenseNumber { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public int? CityId { get; set; }

    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual City? City { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<MasterCodeGeneration> MasterCodeGenerations { get; set; } = new List<MasterCodeGeneration>();

    public virtual ICollection<StoreAppSetting> StoreAppSettings { get; set; } = new List<StoreAppSetting>();

    public virtual ICollection<StoreUserMap> StoreUserMaps { get; set; } = new List<StoreUserMap>();
}
