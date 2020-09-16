using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace NetCore.Web.Extensions
{
    public static class IdentityExtension
    {
        /// <summary>
        /// Given name
        /// </summary>
        /// <param name="identity">Identity 인터페이스 자기 자신</param>
        /// <returns></returns>
        //Given name
        public static string GetGivenName(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.GivenName);
            return claim != null ? claim.Value : string.Empty;
        }

        /// <summary>
        /// Surname
        /// </summary>
        /// <param name="identity">Identity 인터페이스 자기 자신</param>
        /// <returns></returns>
        //Surname
        public static string GetSurname(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Surname);
            return claim != null ? claim.Value : string.Empty;
        }

        /// <summary>
        /// Full name
        /// </summary>
        /// <param name="identity">Identity 인터페이스 자기 자신</param>
        /// <returns></returns>
        //Full name
        public static string GetFullName(this IIdentity identity)
        {
            return GetGivenName(identity) + ", " + GetSurname(identity);
        }
    }
}
