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
            //���� �߰�
            //services.AddIdentity<IdentityUser, IdentityRole>(options =>
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // �α����� �� ������ Ȯ���� ������ ����(�⺻��:false)
                options.SignIn.RequireConfirmedAccount = true;

                // �űԻ���ڸ� ��� �� �ִ��� (�⺻��:true)
                options.Lockout.AllowedForNewUsers = true;

                //��� �߻��� ����ڰ� ���� �ð�(�⺻:5��)
                options.Lockout.DefaultLockoutTimeSpan
                = TimeSpan.FromMinutes(15);

                // ��ݱ�� Ȱ��ȭ�Ǿ� ���� ��
                // ����ڰ� ���� ���� �α��� ���� ���Ƚ��(�⺻ 5��)
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // layout �߰��� �� �ڵ� ����
                .AddDefaultUI()
                // ��й�ȣ �缳��, �̸��� �Ǵ� ��ȭ��ȣ ����
                // 2�ܰ� ����
                // ��ū ������ ���Ǵ� �⺻��ū ������
                .AddDefaultTokenProviders();

            // Email Sender
            // AddTransient : ���񽺰� ��û�� ������ ����
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<SmtpOptions>(Configuration);

            // Seed data
            // AddScoped : Ŭ���̾�Ʈ ����ÿ� �� �� ����
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
