using System;

using HttpReports;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpReportsMiddlewareExtensions
    {
        /// <summary>
        /// 添加HttpReports中间件
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="webType">Type of the web.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="Node">The node.</param>
        /// <returns></returns>
        [Obsolete("有新的替代方案", true)]
        public static IServiceCollection AddHttpReportsMiddleware(this IServiceCollection services, IConfiguration configuration, WebType webType, DBType dbType, string Node = "Default")
        {
            return services;
        }

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
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<HttpReportsOptions>(configuration);
            services.AddSingleton<IModelCreator, DefaultModelCreator>();
            services.AddSingleton<IHttpInvokeProcesser, DefaultHttpInvokeProcesser>();

            return new HttpReportsBuilder(services, configuration);
        }

        /// <summary>
        /// 使用webapi的模式
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpReportsBuilder UseWebApiReports(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IRequestInfoBuilder, ApiRequestInfoBuilder>();
            return builder;
        }

        /// <summary>
        /// 使用MVC的模式
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpReportsBuilder UseMVCReports(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IRequestInfoBuilder, MVCRequestInfoBuilder>();
            return builder;
        }

        /// <summary>
        /// 使用 HttpReports, 必须放在UseMvc之前
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        [Obsolete("有新的替代方案", true)]
        public static IApplicationBuilder UseHttpReportsMiddleware(this IApplicationBuilder app)
        {
            return app;
        }

        /// <summary>
        /// 使用HttpReports, 必须放到请求管道的顶部
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage?.InitAsync().Wait();

            return app.UseMiddleware<DefaultHttpReportsMiddleware>();
        }
    }
}