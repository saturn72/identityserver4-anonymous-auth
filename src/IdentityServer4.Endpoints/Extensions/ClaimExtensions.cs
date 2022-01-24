using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer4
{
    public static class ClaimExtensions
    {
        public static Claim GetFirstOrDefault(this IEnumerable<Claim> claims, string claimType, Claim defaultValue = default)
        {
            if (claims.IsNullOrEmpty() || !claimType.HasValue()) return defaultValue;
            return claims.FirstOrDefault(c => c.Type == claimType) ?? defaultValue;
        }
        public static string GetFirstValueOrDefault(this IEnumerable<Claim> claims, string claimType, string defaultValue = default)
        {
            if (claims.IsNullOrEmpty() || !claimType.HasValue()) return defaultValue;
            return claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? defaultValue;
        }
    }
}
