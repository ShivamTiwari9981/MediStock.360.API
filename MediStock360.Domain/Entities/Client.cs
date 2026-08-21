using System;
using System.Collections.Generic;

namespace MediStock360.Domain.Entities;

public partial class Client
{
    public long ClientId { get; set; }

    public Guid ClientKey { get; set; }

    public string ClientCode { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public int BusinessTypeId { get; set; }

    public string? OwnerName { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Gstnumber { get; set; }

    public string? DrugLicenseNumber { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public int? StateId { get; set; }

    public int? CountryId { get; set; }

    public string? PostalCode { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual BusinessType BusinessType { get; set; } = null!;

    public virtual City? City { get; set; }

    public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();

    public virtual Country? Country { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual State? State { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
