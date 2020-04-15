using System;

using HttpReports;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.Services.Quartz;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {

        public static IHttpReportsBuilder AddHttpReportsDashborad(this IServiceCollection services)
        {
            ServiceContainer.provider = services.BuildServiceProvider();

            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReportsDashboard"); 

            services.AddOptions();
            services.Configure<DashboardOptions>(configuration);

            return services.UseHttpReportsDashboradService(configuration);
        }


        public static IHttpReportsBuilder AddHttpReportsDashborad(this IServiceCollection services,Action<DashboardOptions> options)
        {
            ServiceContainer.provider = services.BuildServiceProvider();

            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReportsDashboard");

            services.AddOptions();
            services.Configure<DashboardOptions>(options);

            return services.UseHttpReportsDashboradService(configuration);
        }


        private static IHttpReportsBuilder UseHttpReportsDashboradService(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddSingleton<IModelCreator, DefaultModelCreator>();

            services.AddSingleton<IAlarmService, AlarmService>();

            services.AddSingleton<MonitorService>();

            services.AddSingleton<ScheduleService>(); 

            services.AddMvcCore(x => {
                x.Filters.Add<GlobalAuthorizeFilter>();
            });

            return new HttpReportsBuilder(services, configuration);
        }  

        public static IApplicationBuilder UseHttpReportsDashboard(this IApplicationBuilder app)
        {
            ServiceContainer.provider = app.ApplicationServices as ServiceProvider; 

            ConfigRoute(app);

            ConfigStaticFiles(app);

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage.InitAsync().Wait();

            app.ApplicationServices.GetService<ScheduleService>().InitAsync().Wait();
             
            return app;
        }


        /// <summary>
        /// Add access to static resources on debug mode
        /// </summary>
        /// <param name="app"></param>
        private static void ConfigStaticFiles(IApplicationBuilder app)
        {
            if (System.IO.Directory.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, @"wwwroot\HttpReportsStaticFiles")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(AppContext.BaseDirectory, @"wwwroot\HttpReportsStaticFiles")),
                    RequestPath = new Microsoft.AspNetCore.Http.PathString("/HttpReportsStaticFiles")

                });
            } 
        } 


        /// <summary>
        /// Configure user routing  
        /// </summary>
        /// <param name="UseHome"></param>
        /// <param name="app"></param>
        private static void ConfigRoute(IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<DashboardOptions>>().Value;

            app.Use(async (context, next) =>
            {  
                if (context.Request.Path.HasValue)
                { 
                    if (context.Request.Path.Value.ToLower() == (options.UseHome ? "/" : "dashboard"))
                    {
                        context.Request.Path = "/HttpReports";
                    }    
                }

                await next(); 

            }); 
         
        } 


    }
}