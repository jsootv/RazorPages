using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Data
{
    // Table Class
    public class ApplicationRole : IdentityRole<string>
    {
        // Constructors
        public ApplicationRole() : this(null)
        {
        }

        public ApplicationRole(string roleName)
        {
            Id = Guid.NewGuid().ToString();
            Name = roleName;
        }

        // Column Variables
        /// <summary>
        /// 권한 설명
        /// </summary>
        [Required, StringLength(100), Column(TypeName="nvarchar(100)")]
        public string Description { get; set; }

        /// <summary>
        /// 권한 우선 순위
        /// </summary>
        [Required]
        public byte RolePriority { get; set; }
    }
}
