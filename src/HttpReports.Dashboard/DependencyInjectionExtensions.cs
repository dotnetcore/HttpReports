using System;
using System.Linq;
using System.Reflection;
using HttpReports;
using HttpReports.Core;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Handles;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Routes;
using HttpReports.Dashboard.Services; 
using HttpReports.Storage.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Quartz.Spi;
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

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
            services.PostConfigure<DashboardOptions>(x => {

                if (x.Check == null)
                {
                    x.Check = new HealthCheckOptions();
                }

                if (string.IsNullOrEmpty(x.Check.Endpoint))
                {
                    x.Check.Endpoint = BasicConfig.HttpReportsDefaultHealth;
                }

                if (string.IsNullOrEmpty(x.Check.Range))
                {
                    x.Check.Range = "500,2000";
                }

            }); 

            services.AddHttpClient(BasicConfig.HttpReportsHttpClient);

            services.AddSingleton<IAlarmService, AlarmService>(); 
            services.AddSingleton<IAuthService, AuthService>();  
            services.AddSingleton<IScheduleService, ScheduleService>();  
            services.AddSingleton<ILocalizeService,LocalizeService>();
            services.AddSingleton<IHealthCheckService, HealthCheckService>();

            services.AddHandleService(); 
            services.AddHttpReportsHttpCollector(); 

            return new HttpReportsBuilder(services, configuration);
        }   

        public static IApplicationBuilder UseHttpReportsDashboard(this IApplicationBuilder app)
        { 
            ServiceContainer.provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Failed");   

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Not Found");

            storage.InitAsync().Wait();

            app.ApplicationServices.GetService<IScheduleService>().InitAsync().Wait(); 

            var localizeService = app.ApplicationServices.GetRequiredService<ILocalizeService>() ?? throw new ArgumentNullException("localizeService Not Found");

            localizeService.InitAsync().Wait();

            app.UseHttpReportsHttpCollector();

            app.UseMiddleware<DashboardMiddleware>(); 

            return app;
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
       

    }
}