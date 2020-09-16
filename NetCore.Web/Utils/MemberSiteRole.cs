using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Utils
{
    public static class MemberSiteRole
    {
        /// <summary>
        /// 준사용자
        /// </summary>
        public const string _associateUser = "AssociateUser";

        /// <summary>
        /// 일반사용자
        /// </summary>
        public const string _generalUser = "GeneralUser";

        /// <summary>
        /// 향상된 사용자
        /// </summary>
        public const string _superUser = "SuperUser";

        /// <summary>
        /// 시스템사용자
        /// </summary>
        public const string _systemUser = "SystemUser";
    }
}
