using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class UserOtp
{
    public long UserOtpId { get; set; }

    public long UserId { get; set; }

    public byte OtpType { get; set; }

    public string OtpHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public int AttemptCount { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public long? ClientId { get; set; }

    public virtual User User { get; set; } = null!;
}
