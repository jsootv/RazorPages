using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Utils
{
    public static class MemberDescription
    {
        /// <summary>
        /// 정식으로 회원가입한 사용자
        /// </summary>
        public const string _register = "Register";

        /// <summary>
        /// 데이터 심기(Seed data)를 통해 가입된 사용자
        /// </summary>
        public const string _seedData = "Seed Data";

        /// <summary>
        /// 데이터베이스에서 직접 입력해서 가입된 사용자
        /// </summary>
        public const string _enterDataDirectly = "Enter Data Directly";
    }
}
