using Microsoft.AspNetCore.Identity;
using NetCore.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly string _adminEmail = "admin@netcore.com";

        public DbInitializer(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SeedData()
        {
            bool ExistAdmin = _userManager.Users.Where(u => u.NormalizedUserName == _adminEmail.ToUpper()).Any();

            if (!ExistAdmin)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = _adminEmail
                    , Email = _adminEmail
                    , EmailConfirmed = true
                    , GivenName = "Netcore"
                    , Surname = "Admin"
                    , ContactName = "Admin's Address"
                    , Description = MemberDescription._seedData
                };

                // 관리자 계정 가입
                await _userManager.CreateAsync(user, "Netcore3#");

                // 관리자 권한
                await _userManager.AddToRoleAsync(user, MemberSiteRole._systemUser);

                // 관리자 클레임
                await _userManager.AddClaimsAsync(user, new[] {
                    new Claim(ClaimTypes.GivenName, user.GivenName)
                    , new Claim(ClaimTypes.Surname, user.Surname)
                    , new Claim(ClaimTypes.StreetAddress, user.ContactName)
                });
            }
        }
    }
}
