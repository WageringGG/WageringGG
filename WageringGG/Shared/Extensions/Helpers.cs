﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WageringGG.Shared.Constants;

namespace WageringGG
{
    public static class Helpers
    {
        public static bool IsUnique<T>(this IEnumerable<T> list)
        {
            var hs = new HashSet<T>();
            foreach (var item in list)
                if (!hs.Add(item))
                    return false;
            return true;
        }

        public static string GetKey(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(x => x.Type == Claims.PublicKey)?.Value;
        }

        public static string GetName(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(x => x.Type == Claims.DisplayName)?.Value;
        }

        public static Claim NameClaim(this IList<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == Claims.DisplayName);
        }

        public static Claim VerifiedClaim(this IList<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == Claims.IsVerified);
        }

        public static Claim KeyClaim(this IList<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == Claims.PublicKey);
        }
    }
}
