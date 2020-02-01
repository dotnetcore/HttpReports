using System;

using HttpReports;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.Services.Quartz;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 添加HttpReports
        /// <para/>自动使用配置文件中的HttpReports节点
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IHttpReportsBuilder AddHttpReports(this IServiceCollection services)
        { 
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");

            return services.AddHttpReports(configuration);
        }

        /// <summary>
        /// 根据配置自动设置Storage
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IHttpReportsBuilder UseStorageAutomatically(this IHttpReportsBuilder builder)
        {
            var type = builder.Configuration.GetSection("StorageType").Value.ToUpperInvariant();

            switch (type)
            {
                case "MYSQL":
                    builder.UseMySqlStorage();
                    break;

                case "ORACLE":
                    builder.UseOracleStorage();
                    break;

                case "SQLSERVER":
                    builder.UseSQLServerStorage();
                    break;

                default:
                    throw new ArgumentException($"存储类型没有正确配置: {type}");
            }
            return builder;
        }

        /// <summary>
        /// 添加HttpReports
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">HttpReports的配置节点</param>
        /// <returns></returns>
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceContainer.provider = services.BuildServiceProvider();

            services.AddOptions();
            services.Configure<HttpReportsOptions>(configuration);
            services.Configure<MailOptions>(configuration.GetSection("Mail"));

            

            services.AddSingleton<IModelCreator, DefaultModelCreator>();

            services.AddSingleton<IAlarmService, AlarmService>();

            services.AddTransient<MonitorService>();

            services.AddSingleton<ScheduleService>(); 
           
            return new HttpReportsBuilder(services, configuration);
        } 
        

        /// <summary>
        /// 使用HttpReports, 必须放到请求管道的顶部
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            ServiceContainer.provider = app.ApplicationServices as ServiceProvider; 

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage.InitAsync().Wait();

            app.ApplicationServices.GetService<ScheduleService>().InitAsync().Wait(); 
             
            return app;
        } 
        
    }
}