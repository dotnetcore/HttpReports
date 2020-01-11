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
        [Obsolete]
        public static IServiceCollection AddHttpReportsMiddleware(this IServiceCollection services, IConfiguration configuration, WebType webType, DBType dbType, string Node = "Default")
        {
            Action<HttpReportsOptions> options = (op) =>
            {
                op.DBType = dbType;
                op.WebType = webType;
                op.Node = string.IsNullOrEmpty(Node) ? "Default" : Node;
            };

            services.AddOptions();
            services.Configure(options);
            HttpReportsMiddleware.Configuration = configuration;
            return services.AddTransient<IHttpReports, DefaultHttpReports>();
        }

        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<HttpReportsOptions>(configuration);
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

        /// 使用 HttpReports, 必须放在UseMvc之前
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        [Obsolete]
        public static IApplicationBuilder UseHttpReportsMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpReportsMiddleware>();
        }

        /// <summary>
        /// 使用HttpReports, 必须放到请求管道的顶部
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>();
            storage?.InitAsync().Wait();

            return app.UseMiddleware<DefaultHttpReportsMiddleware>();
        }
    }
}