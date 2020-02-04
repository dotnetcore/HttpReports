using System;

using HttpReports;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.Services.Quartz;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
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
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services)
        { 
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");

            return services.AddHttpReports(configuration);
        }

        
        /// <summary>
        /// 添加HttpReports
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">HttpReports的配置节点</param>
        /// <returns></returns>
        private static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration)
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
        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            app.Use(async (context, next) => {

                if (context.Request.Path.Value.ToLower() == "/dashboard")
                {
                    context.Request.Path = "/HttpReportsHome";
                }

                await next();

            });

            ServiceContainer.provider = app.ApplicationServices as ServiceProvider; 

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage.InitAsync().Wait();

            app.ApplicationServices.GetService<ScheduleService>().InitAsync().Wait();  

            // 静态资源文件引用
            if (System.IO.Directory.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, @"wwwroot\HttpReportsStaticFiles")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(AppContext.BaseDirectory, @"wwwroot\HttpReportsStaticFiles")),
                    RequestPath = new Microsoft.AspNetCore.Http.PathString("/HttpReportsStaticFiles") 
                });
            } 

            return app;
        } 
        
    }
}