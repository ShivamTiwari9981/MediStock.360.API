using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class User
{
    public long UserId { get; set; }

    public long ClientId { get; set; }

    public Guid UserKey { get; set; }

    public long? EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserSalt { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool? IsSynced { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
