using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class Client
{
    public long ClientId { get; set; }

    public Guid ClientKey { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? ClientName { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? OwnerName { get; set; }

    public int? BusinessTypeId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Gstnumber { get; set; }

    public string? DrugLicenseNumber { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public int? StateId { get; set; }

    public int? CountryId { get; set; }

    public string? PostalCode { get; set; }

    public bool IsOnboardingCompleted { get; set; }

    public int OnboardingStep { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual BusinessType? BusinessType { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<ClientAppSetting> ClientAppSettings { get; set; } = new List<ClientAppSetting>();

    public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();

    public virtual Country? Country { get; set; }

    public virtual ICollection<MasterCodeGeneration> MasterCodeGenerations { get; set; } = new List<MasterCodeGeneration>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual State? State { get; set; }

    public virtual ICollection<StoreAppSetting> StoreAppSettings { get; set; } = new List<StoreAppSetting>();

    public virtual ICollection<StoreUserMap> StoreUserMaps { get; set; } = new List<StoreUserMap>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
