using System;
using System.Linq;
using System.Reflection;
using HttpReports;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Handle;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Route;
using HttpReports.Dashboard.Services; 
using HttpReports.Dashboard.Services.Quartz;
using HttpReports.Dashboard.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    { 
        public static IHttpReportsBuilder AddHttpReportsDashboard(this IServiceCollection services)
        {  
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReportsDashboard"); 

            services.AddOptions();
            services.Configure<DashboardOptions>(configuration);

            return services.UseHttpReportsDashboardService(configuration);
        } 



        public static IHttpReportsBuilder AddHttpReportsDashboard(this IServiceCollection services,Action<DashboardOptions> options)
        { 

            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReportsDashboard");

            services.AddOptions();
            services.Configure<DashboardOptions>(options);

            return services.UseHttpReportsDashboardService(configuration);
        }


        private static IHttpReportsBuilder UseHttpReportsDashboardService(this IServiceCollection services, IConfiguration configuration)
        {  
            services.AddSingleton<IModelCreator, DefaultModelCreator>();

            services.AddSingleton<IAlarmService, AlarmService>();

            services.AddSingleton<MonitorService>();

            services.AddSingleton<ScheduleService>(); 

            services.AddSingleton<LocalizeService>(); 

            services.AddHandleService().AddViewsService();   

            return new HttpReportsBuilder(services, configuration);
        }  

        public static IApplicationBuilder UseHttpReportsDashboard(this IApplicationBuilder app)
        { 
            ServiceContainer.provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Faild"); 

            ConfigRoute(app); 

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Not Found");
            storage.InitAsync().Wait();

            app.ApplicationServices.GetService<ScheduleService>().InitAsync().Wait(); 

            var localizeService = app.ApplicationServices.GetRequiredService<LocalizeService>() ?? throw new ArgumentNullException("localizeService Not Found");

            localizeService.InitAsync().Wait(); 

            app.UseMiddleware<DashboardMiddleware>(); 

            return app;
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
                        context.Request.Path = "/HttpReports/Index";
                    }    
                }

                await next(); 

            }); 
         
        }


        private static IServiceCollection AddHandleService(this IServiceCollection services)
        {
            var handles = Assembly.GetAssembly(typeof(DashboardRoute)).GetTypes().Where(x => typeof(DashboardHandleBase).IsAssignableFrom(x) && x != typeof(DashboardHandleBase));

            foreach (var handle in handles)
            {
                services.AddTransient(handle);
            }

            return services;

        }

        private static IServiceCollection AddViewsService(this IServiceCollection services)
        {
            var views = Assembly.GetAssembly(typeof(DashboardRoute)).GetTypes().Where(x => typeof(RazorPage).IsAssignableFrom(x) && x != typeof(RazorPage));

            foreach (var view in views)
            {
                services.AddTransient(view);
            }

            return services;
        } 

    }
}