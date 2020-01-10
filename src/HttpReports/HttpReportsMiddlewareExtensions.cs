using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    public static class HttpReportsMiddlewareExtensions
    { 
        /// <summary>
        /// 添加HttpReports中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpReportsMiddleware(this IServiceCollection services, IConfiguration configuration, WebType webType, DBType dbType,string Node = "Default")
        {
            Action<HttpReportsOptions> options = (op) =>
            {
                op.DBType = dbType;
                op.WebType = webType;
                op.Node = string.IsNullOrEmpty(Node) ? "Default":Node; 
            };   

            services.AddOptions();
            services.Configure(options);
            HttpReportsMiddleware.Configuration = configuration;
            return services.AddTransient<IHttpReports, DefaultHttpReports>();
        }  

        /// <summary>
        /// 使用 HttpReports, 必须放在UseMvc之前
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReportsMiddleware(this IApplicationBuilder app)
        { 
            return app.UseMiddleware<HttpReportsMiddleware>();
        }
    }
}
