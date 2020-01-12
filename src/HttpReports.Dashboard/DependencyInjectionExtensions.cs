using System;

using HttpReports;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

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

            return new HttpReportsBuilder(services, configuration);
        }

        /// <summary>
        /// 使用HttpReports, 必须放到请求管道的顶部
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static IApplicationBuilder ConfigHttpReports(this IApplicationBuilder app)
        {
            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("未正确配置存储方式");
            storage?.InitAsync().Wait();

            return app;
        }
    }
}