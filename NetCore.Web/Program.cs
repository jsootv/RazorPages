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
        // ���� �޼��带 �񵿱� �޼���� ����
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            // Seed data
            // try~catch~finally ����
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
                // scope ���ҽ��� ����
                scope.Dispose();
            }
            */

            // IDisposable : �������� �ʴ� ���ҽ��� �����ϴ� ��Ŀ������ ����
            // IDisposable�� ��ӹ޾ƾ� using �������� ���
            // void Dispose() �޼��� : �������� �ʴ� ���ҽ� ����
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IDbInitializer initializer = scope.ServiceProvider.GetService<IDbInitializer>();
                await initializer.SeedData();

                // using �������� �ڵ����� ���ҽ��� ����
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
