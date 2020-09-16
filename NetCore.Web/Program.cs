using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore.Web.Data;

namespace NetCore.Web
{
    public class Program
    {
        // 동기 메서드를 비동기 메서드로 변경
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            // Seed data
            // try~catch~finally 구문
            /*
            IServiceScope scope = host.Services.CreateScope();

            try
            {
                IDbInitializer initializer = scope.ServiceProvider.GetService<IDbInitializer>();
                await initializer.SeedData();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                // scope 리소스를 해제
                scope.Dispose();
            }
            */

            // IDisposable : 관리되지 않는 리소스를 해제하는 메커니즘을 제공
            // IDisposable을 상속받아야 using 구문으로 사용
            // void Dispose() 메서드 : 관리되지 않는 리소스 해제
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IDbInitializer initializer = scope.ServiceProvider.GetService<IDbInitializer>();
                await initializer.SeedData();

                // using 구문에서 자동으로 리소스를 해제
                //scope.Dispose();
            }

            host.Run();
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
