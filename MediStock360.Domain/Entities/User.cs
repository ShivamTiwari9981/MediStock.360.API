using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class User
{
    public long UserId { get; set; }

    public long ClientId { get; set; }

    public Guid UserKey { get; set; }

    public long? EmployeeId { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserSalt { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public bool IsLocked { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<StoreUserMap> StoreUserMaps { get; set; } = new List<StoreUserMap>();

    public virtual ICollection<UserAppSetting> UserAppSettings { get; set; } = new List<UserAppSetting>();

    public virtual ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
