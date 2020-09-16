using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NetCore.Web.Data
{
    // IdentityDbContext<IdentityUser, IdentityRole, string>
    // IdentityDbContext<User type, Role type, Key type>
    // IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    // IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //FluentAPI Chain 메서드
            #region ① 컬럼 기본값 지정 - 1개(1번)
            // 1.
            builder.Entity<ApplicationRole>()
                .Property(c => c.RolePriority)
                .HasDefaultValue(1);
            #endregion
            #region ② 컬럼 NOT NULL 지정 - 1개(2번)
            // 2.
            builder.Entity<ApplicationUser>()
                .Property(c => c.UserName)
                .IsRequired(true);
            #endregion
            #region ③ DB 테이블이름 매핑 및 변경 - 7개(3-9번)
            // 3.
            builder.Entity<ApplicationUser>()
                .ToTable("Member");
            // 4.
            builder.Entity<ApplicationRole>()
                .ToTable("SiteRole");
            // 5.
            builder.Entity<IdentityUserRole<string>>()
                .ToTable("MemberSiteRole");
            // 6.
            builder.Entity<IdentityUserClaim<string>>()
                .ToTable("MemberClaim");
            // 7.
            builder.Entity<IdentityUserLogin<string>>()
                .ToTable("MemberLogin");
            // 8.
            builder.Entity<IdentityUserToken<string>>()
                .ToTable("MemberToken");
            // 9.
            builder.Entity<IdentityRoleClaim<string>>()
                .ToTable("SiteRoleClaim");
            #endregion
            #region ④ 컬럼명 및 컬럼 Data Type 지정 - 7그룹(10-16번)
            // 10.
            builder.Entity<ApplicationUser>(e =>
            {
                // 10-1.
                e.Property(c => c.Id).HasColumnName("UserId")
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 10-2.
                e.Property(c => c.PasswordHash)
                .HasColumnType("varchar(100)").HasMaxLength(100);
                // 10-3.
                e.Property(c => c.SecurityStamp)
                .HasColumnType("varchar(100)").HasMaxLength(100);
                // 10-4.
                e.Property(c => c.ConcurrencyStamp)
                .HasColumnType("varchar(100)").HasMaxLength(100);
                // 10-5.
                e.Property(c => c.PhoneNumber)
                .HasColumnType("varchar(50)").HasMaxLength(50);
            });
            // 11.
            builder.Entity<ApplicationRole>(e =>
            {
                // 11-1.
                e.Property(c => c.Id).HasColumnName("RoleId")
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 11-2.
                e.Property(c => c.ConcurrencyStamp)
                .HasColumnType("varchar(100)").HasMaxLength(100);
            });
            // 12.
            builder.Entity<IdentityUserRole<string>>(e =>
            {
                // 12-1.
                e.Property(c => c.UserId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 12-2.
                e.Property(c => c.RoleId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
            });
            // 13.
            builder.Entity<IdentityUserClaim<string>>(e =>
            {
                // 13-1.
                e.Property(c => c.Id).HasColumnName("UserSeq");
                // 13-2.
                e.Property(c => c.UserId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 13-3.
                e.Property(c => c.ClaimType)
                .HasColumnType("varchar(100)").HasMaxLength(100);
                // 13-4.
                e.Property(c => c.ClaimValue)
                .HasColumnType("nvarchar(100)").HasMaxLength(100);
            });
            // 14.
            builder.Entity<IdentityUserLogin<string>>(e =>
            {
                // 14-1.
                e.Property(c => c.LoginProvider)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 14-2.
                e.Property(c => c.ProviderKey)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 14-3.
                e.Property(c => c.ProviderDisplayName)
                .HasColumnType("nvarchar(100)").HasMaxLength(100);
                // 14-4.
                e.Property(c => c.UserId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
            });
            // 15.
            builder.Entity<IdentityUserToken<string>>(e =>
            {
                // 15-1.
                e.Property(c => c.UserId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 15-2.
                e.Property(c => c.LoginProvider)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 15-3.
                e.Property(c => c.Name)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 15-4.
                e.Property(c => c.Value)
                .HasColumnType("nvarchar(100)").HasMaxLength(100);
            });
            // 16.
            builder.Entity<IdentityRoleClaim<string>>(e =>
            {
                // 16-1.
                e.Property(c => c.Id).HasColumnName("RoleSeq");
                // 16-2.
                e.Property(c => c.RoleId)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 16-3.
                e.Property(c => c.ClaimType)
                .HasColumnType("varchar(50)").HasMaxLength(50);
                // 16-4.
                e.Property(c => c.ClaimValue)
                .HasColumnType("nvarchar(100)").HasMaxLength(100);
            });
            #endregion
            #region ⑤ 관계(ForeignKey:외래키) 지정 - 2그룹(17-18번)
            // 17.
            builder.Entity<ApplicationUser>(e =>
            {
                // 17-1.
                //ApplicationUser가 IdentityUserRole<string> 컬렉션을 가진다.
                e.HasMany<IdentityUserRole<string>>()
                //일대다 관계                           // FK Column NOT NULL
                .WithOne().HasForeignKey(c => c.UserId).IsRequired();
                // 17-2.
                e.HasMany<IdentityUserClaim<string>>()
                .WithOne().HasForeignKey(c => c.UserId).IsRequired();
                // 17-3.
                e.HasMany<IdentityUserToken<string>>()
                .WithOne().HasForeignKey(c => c.UserId).IsRequired();
                // 17-4.
                e.HasMany<IdentityUserLogin<string>>()
                .WithOne().HasForeignKey(c => c.UserId).IsRequired();
            });
            // 18.
            builder.Entity<ApplicationRole>(e =>
            {
                //18-1.
                e.HasMany<IdentityUserRole<string>>()
                .WithOne().HasForeignKey(c => c.RoleId).IsRequired();
                //18-2.
                e.HasMany<IdentityRoleClaim<string>>()
                .WithOne().HasForeignKey(c => c.RoleId).IsRequired();
            });
            #endregion
            #region ⑥ 복합키(CompositeKey) 지정 - 3개(19-21번)
            // 19.
            builder.Entity<IdentityUserRole<string>>()
                .HasKey(c => new { c.UserId, c.RoleId });
            // 20.
            builder.Entity<IdentityUserToken<string>>()
                .HasKey(c => new { c.UserId, c.LoginProvider, c.Name });
            // 21.
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(c => new { c.LoginProvider, c.ProviderKey });
            #endregion
            #region ⑦ 효율적 검색을 위한 인덱스 - 2그룹(22-23번)
            // 22.
            // Database Table Column Value VS 입력된 UserName, Email, RoleName 값(대문자 처리된)
            builder.Entity<ApplicationUser>(e =>
            {
                // 22-1.
                e.HasIndex(c => c.NormalizedUserName)
                .HasName("UserNameIndex")
                .IsUnique();// 중복허용 안함
                // 22-2.
                e.HasIndex(c => c.NormalizedEmail)
                .HasName("EmailIndex");
            });
            // 23.
            builder.Entity<ApplicationRole>()
                .HasIndex(c => c.NormalizedName)
                .HasName("RoleNameIndex");
            #endregion
        }
    }
}
