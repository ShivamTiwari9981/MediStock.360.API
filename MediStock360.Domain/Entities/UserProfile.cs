using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class UserProfile
{
    public long UserProfileId { get; set; }

    public long ClientId { get; set; }

    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? DisplayName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? AlternatePhoneNumber { get; set; }

    public string? ProfileImageUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public long? CityId { get; set; }

    public long? StateId { get; set; }

    public long? CountryId { get; set; }

    public string? PostalCode { get; set; }

    public bool IsPhoneVerified { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
