using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; 
using Microsoft.OpenApi.Models; 

namespace HttpReports.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } 

      
        public void ConfigureServices(IServiceCollection services)
        {  
            services.AddHttpReports().AddHttpTransport();   

            services.AddHttpReportsDashboard().AddMySqlStorage(); 

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HttpReports.Demo", Version = "v1" }); 
            });

        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {    
            app.UseHttpReports();
            //app.UseMiddleware<ErrorMiddleware>();

            app.UseHttpReportsDashboard(); 

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcCollector();
                endpoints.MapControllers();

            });
        }
    }
}
