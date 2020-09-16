using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace NetCore.Web.Data
{
    // Table Class
    public class ApplicationUser : IdentityUser<string>
    {
        // Constructors
        public ApplicationUser() : this(null)
        {
        }

        public ApplicationUser(string userName)
        {
            Id = Guid.NewGuid().ToString();
            UserName = userName;
        }

        // Column Variables
        /// <summary>
        /// 회원 설명
        /// </summary>
        [Required, StringLength(500), Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; }

        #region 회원 클레임 정보
        // 회원 클레임 정보
        //string claimInfo = ClaimTypes.StreetAddress;
        /// <summary>
        /// 회원 이름 First name
        /// </summary>
        [NotMapped]
        public string GivenName { get; set; }

        /// <summary>
        /// 회원 성 Last name
        /// </summary>
        [NotMapped]
        public string Surname { get; set; }

        /// <summary>
        /// 회원 주소
        /// </summary>
        [NotMapped]
        public string ContactName { get; set; }
        #endregion

        /// <summary>
        /// 회원의 보유권한 정보
        /// </summary>
        [NotMapped]
        public ApplicationRole MemberInRole { get; set; }

        /// <summary>
        /// 권한변경이 금지된 회원인가?(true:금지됨, false:금지안됨)
        /// </summary>
        [NotMapped]
        public bool IsTheMemberWhoCannotChangeRole { get; set; }
    }
}
