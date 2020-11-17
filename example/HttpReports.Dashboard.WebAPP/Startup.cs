using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks; 
using HttpReports.Core.Diagnostics; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

namespace HttpReports.Dashboard.WebAPP
{
    public class Startup
    { 

        public void ConfigureServices(IServiceCollection services) 
        { 
            //services.AddHttpReports().UseHttpTransport();   

            services.AddHttpReportsDashboard().UseMySqlStorage();

            services.AddControllers(); 

        } 
      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 

            //app.UseHttpReports();

            app.UseHttpReportsDashboard();    

            //app.UseMiddleware<ErrorMiddleware>();  

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            }); 

        } 

    }
}
