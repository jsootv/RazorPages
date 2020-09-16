using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using NetCore.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using NetCore.Web.Utils;

namespace NetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            //services.AddDefaultIdentity<IdentityUser>(options =>
            //권한 추가
            //services.AddIdentity<IdentityUser, IdentityRole>(options =>
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // 로그인할 때 계정을 확인할 것인지 결정(기본값:false)
                options.SignIn.RequireConfirmedAccount = true;

                // 신규사용자를 잠글 수 있는지 (기본값:true)
                options.Lockout.AllowedForNewUsers = true;

                //잠금 발생때 사용자가 잠기는 시간(기본:5분)
                options.Lockout.DefaultLockoutTimeSpan
                = TimeSpan.FromMinutes(15);

                // 잠금기능 활성화되어 있을 때
                // 사용자가 잠기기 전에 로그인 실패 허용횟수(기본 5번)
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // layout 추가할 때 자동 생성
                .AddDefaultUI()
                // 비밀번호 재설정, 이메일 또는 전화번호 변경
                // 2단계 인증
                // 토큰 생성에 사용되는 기본토큰 공급자
                .AddDefaultTokenProviders();

            // Email Sender
            // AddTransient : 서비스가 요청될 때마다 생성
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<SmtpOptions>(Configuration);

            // Seed data
            // AddScoped : 클라이언트 연결시에 한 번 생성
            services.AddScoped<IDbInitializer,DbInitializer>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
