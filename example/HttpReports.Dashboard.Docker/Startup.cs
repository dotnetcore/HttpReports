using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        public IConfiguration Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            _logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger<Startup>(); 
         

            Test();

            services.AddHttpReportsDashboard().UseAutoStorage(new StorageOptions
            { 
                StorageType = "MySql",
                ConnectionString = "DataBase=HttpReports;Data Source=127.0.0.1;User Id=root;Password=123456;"

            }) ;

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


        public void Test()
        {   
            _logger.LogInformation("----------------------------------------------------------");
            _logger.LogInformation($"HttpReports:Storage:ConnectionString:{Configuration["HttpReports:Storage:ConnectionString"]}");
            _logger.LogInformation($"HttpReports:ExpireDay:{Configuration["HttpReports:ExpireDay"]}"); 

        } 

    }
}
