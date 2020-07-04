using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using HttpReports;
using HttpReports.Core.Config;
using HttpReports.Transport.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions; 

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddOptions().Configure<HttpTransportOptions>(configuration);
            return builder.UseHttpTransport();
        }

        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder, Action<HttpTransportOptions> options)
        {
            builder.Services.AddOptions().Configure(options);
            return builder.UseHttpTransport();
        }

        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder)
        {
            builder.Services.AddHttpClient(BasicConfig.HttpReportsHttpClient,client => {

                client.DefaultRequestHeaders.Clear(); 
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=utf-8");  

            });

            builder.Services.RemoveAll<IReportsTransport>();
            builder.Services.AddSingleton<IReportsTransport, HttpTransport>();
            builder.Services.RemoveAll<IModelCreator>();
            builder.Services.AddSingleton<IModelCreator, ModelCreator>();
            return builder;
        }
    }
}
