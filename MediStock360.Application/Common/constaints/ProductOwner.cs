using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Common.constaints
{
    public static class ProductOwner
    {
        public static class InternalClientSeed
        {
            public const int ClientId = 00000000 - 0000 - 0000 - 0000 - 000000000000;
            public const string ClientCode = "INTERNAL";
            public const string ClientName = "MediStock360 Internal Admin";
            public const string CompanyName = "MediStock 360 Internal";
            public const string OwnerName = "Shivam";
             public const string Email = "marcus.po@medistock360.com";
            public const string Phone = "+919876543210";
            public const string Address = "Headquarters, Main Campus";
            public const string PostalCode = "10001";
        }


        public static class ProductOwnerUserSeed
        {
            public const string UserName= "productowner";
            public const string Email = "shiv7398kumar@gmail.com";
            public const string Password = "Product#Owner2026";
            public const string FirstName = "Shivam";
            public const string LastName = "Tiwari";
            public const string PhoneNumber = "+919389773800";
  
        }

        public static class ProductOwnerRole
        {
            public const string RoleCode = "PRODUCT_OWNER";
            public const string RoleName = "Product Owner";
            public const string Description = "Product owner with global system administration and client management permissions";

        }

    }
}
