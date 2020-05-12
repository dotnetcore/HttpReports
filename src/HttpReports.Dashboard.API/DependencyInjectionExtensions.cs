using System;

using HttpReports;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.Services.Language;
using HttpReports.Dashboard.Services.Quartz;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="routePrefix"></param>
        /// <returns></returns>
        public static IHttpReportsBuilder AddHttpReportsDashboardAPI(this IServiceCollection services, IConfiguration configuration, string routePrefix = "HttpReportsDashboard/Api")
        {
            services.AddOptions()
                    .Configure<DashboardAPIOptions>(configuration);

            services.AddMvcCore(options =>
            {
                options.Conventions.Add(new HttpReportsDashboardApplicationModelConvention(routePrefix));
            }).PartManager.FeatureProviders.Add(new HttpReportsDashboardControllerFeatureProvider());

            services.AddSingleton<IModelCreator, DefaultModelCreator>();

            services.AddSingleton<IAlarmService, AlarmService>();

            services.AddSingleton<MonitorService>();

            services.AddQuartz();

            services.AddSingleton<ChineseLanguage>();
            services.AddSingleton<EnglishLanguage>();
            services.AddSingleton<LanguageService>();

            return new HttpReportsBuilder(services, configuration);
        }

        /// <summary>
        /// 添加Quartz
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, AutoInjectionJobFactory>();
            services.AddSingleton<QuartzLogProvider>();
            services.AddSingleton<QuartzSchedulerService>();

            return services;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReportsDashboardAPI(this IApplicationBuilder app)
        {
            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage.InitAsync().Wait();

            app.ConfigQuartz();

            return app;
        }

        /// <summary>
        /// 配置Quartz
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder ConfigQuartz(this IApplicationBuilder app)
        {
            var schedulerService = app.ApplicationServices.GetRequiredService<QuartzSchedulerService>();
            schedulerService.InitAsync().Wait();

            return app;
        }
    }
}