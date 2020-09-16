using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Data
{
    interface IDbInitializer
    {
        /// <summary>
        /// 데이터 심기
        /// </summary>
        /// <returns></returns>
        Task SeedData();
    }
}
