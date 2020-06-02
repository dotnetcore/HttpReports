using HttpReports.Storage.FilterOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpReports()
                    .UseMySqlStorage()
                    .UseGrpc()
                    .AddHttpReportsGrpcCollector();     //Add Grpc Collector

            services.AddHttpReportsDashboard().UseMySqlStorage();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Task.Run(() =>
            {

                for (int i = 0; i < 100000; i++)
                {
                    Task.Run(() =>
                    {

                        var str = new HttpClient().GetStringAsync("http://www.baidu.com").Result;

                    });
                }

            });


            app.UseHttpReports();

            app.UseHttpReportsDashboard();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHttpReportsGrpcCollector();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}