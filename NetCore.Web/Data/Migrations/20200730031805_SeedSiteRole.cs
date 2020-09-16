using Microsoft.EntityFrameworkCore.Migrations;
using NetCore.Web.Utils;

namespace NetCore.Web.Data.Migrations
{
    public partial class SeedSiteRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SetSiteRole(migrationBuilder, MemberSiteRole._associateUser, "준사용자", 1);
            SetSiteRole(migrationBuilder, MemberSiteRole._generalUser, "일반사용자", 2);
            SetSiteRole(migrationBuilder, MemberSiteRole._superUser, "향상된 사용자", 3);
            SetSiteRole(migrationBuilder, MemberSiteRole._systemUser, "시스템사용자", 4);
        }

        /// <summary>
        /// 사이트 권한 세팅하기
        /// </summary>
        /// <param name="builder">마이그레이션 빌더</param>
        /// <param name="roleName">권한 이름</param>
        /// <param name="roleDescription">권한 설명</param>
        /// <param name="rolePriority">권한 우선순위</param>
        private void SetSiteRole(MigrationBuilder builder
                                , string roleName
                                , string roleDescription
                                , byte rolePriority)
        {
            builder.Sql(@"INSERT INTO [dbo].[SiteRole]
                                       ([RoleId]
                                       ,[Name]
                                       ,[NormalizedName]
                                       ,[ConcurrencyStamp]
                                       ,[Description]
                                       ,[RolePriority])
                                 VALUES
                                       (NEWID() " +
                                       $" , '{roleName}'" +
                                       $" , '{roleName.ToUpper()}'" +
                                       " , NEWID()" +
                                       $" , '{roleDescription}'" +
                                       $" , {rolePriority})");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
