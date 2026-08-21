using System;
using System.Collections.Generic;

namespace MediStock360.Domain.Entities;

public partial class User
{
    public long UserId { get; set; }

    public long ClientId { get; set; }

    public long? StoreId { get; set; }

    public Guid UserKey { get; set; }

    public string? EmployeeCode { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Store? Store { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
