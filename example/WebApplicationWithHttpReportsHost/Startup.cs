using HttpReports.Dashboard;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace WebApplicationWithHttpReportsHost
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
            services.AddControllersWithViews();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebApi", Version = "v1" });

                options.DocInclusionPredicate((docName, description) => true);
            });

            // !!!Important for API
            services.AddHttpReportsDashboardAPI(new DashboardAPIRouteOptions("HttpReports/api", "HttpReports.Api.Cors"), Configuration.GetSection("HttpReports"))
                    .AddHttpReportsGrpcCollector()
                    .UseMySqlStorage();

            // !!!Important for UI
            services.AddHttpReportsDashboardUI(Configuration.GetSection("HttpReportsUI"));

            // API CORS.....
            services.AddCors(option => option.AddPolicy("HttpReports.Api.Cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // !!!Important for API
            app.InitHttpReportsDashboardAPI();

            // !!!Important for UI
            app.UseHttpReportsDashboardUI();

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

            // API CORS.....
            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // !!!Important for GrpcCollector
                endpoints.MapHttpReportsGrpcCollector();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}