using System;
using System.Collections.Generic;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsOnboardingCompleted { get; set; }
        public int OnboardingStep { get; set; }

        public List<UserClientDto> Clients { get; set; } = new();
        public UserClientDto? ActiveClient { get; set; }
        public List<StoreResponseDto> Stores { get; set; } = new();
        public StoreResponseDto? ActiveStore { get; set; }
        public List<MenuResponseDto> Menus { get; set; } = new();

        public LoginUserInfoDto User { get; set; } = null!;
    }

    public class LoginUserInfoDto
    {
        public long UserId { get; set; }
        public Guid UserKey { get; set; }
        public long ClientId { get; set; }
        public Guid ClientKey { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsOnboardingCompleted { get; set; }
        public int OnboardingStep { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();

        public List<UserClientDto> Clients { get; set; } = new();
        public List<StoreResponseDto> Stores { get; set; } = new();
        public List<MenuResponseDto> Menus { get; set; } = new();
    }
}
