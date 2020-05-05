using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpReports.Dashboard.Docker
{
    public class Startup
    {  
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        public IConfiguration Configuration { get; }

        private ILogger<Startup> _logger { get; set; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            BuildHttpRereports(services);

            services.AddControllersWithViews();
        } 

   
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpReportsDashboard();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void BuildHttpRereports(IServiceCollection services)
        {
            _logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();

            string ConnectionString = Configuration["HttpReportsDashboard:Storage:ConnectionString"];

            string Type = Configuration["HttpReportsDashboard:Type"];

            int ExpireDay = Configuration["HttpReportsDashboard:ExpireDay"].ToInt();

            _logger.LogInformation($"Init Start...");
            _logger.LogInformation($"HttpReportsDashboard:Storage:ConnectionString:{ConnectionString}");
            _logger.LogInformation($"HttpReportsDashboard:Type:{Type}");
            _logger.LogInformation($"HttpReportsDashboard:ExpireDay:{ExpireDay}");

            services.AddHttpReportsDashboard(x => {
                x.ExpireDay = ExpireDay > 0 ? ExpireDay : BasicConfig.ExpireDay;
            }).UseAutoStorage(new StorageOptions
            {
                StorageType = Type ?? string.Empty,
                ConnectionString = ConnectionString ?? string.Empty
            }); 

        }  
    }
}
