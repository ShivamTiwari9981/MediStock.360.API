using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MediStock360.Application.Common
{
   

    public static class ClaimsHelper
    {
        public static string? GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            return user?.FindFirst(claimType)?.Value;
        }
    }
}
