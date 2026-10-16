namespace MediStock360.Application.Common
{
    public class ProductOwnerUserSeedSettings
    {
        public string UserName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string PhoneNumber { get; set; } 
    }

    public class InternalClientSeedSettings
    {
        public int ClientId { get; set; }
        public string ClientCode { get; set; } 
        public string ClientName { get; set; } 
        public string CompanyName { get; set; }
        public string? OwnerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; } 
        public string? PostalCode { get; set; }
    }
}

