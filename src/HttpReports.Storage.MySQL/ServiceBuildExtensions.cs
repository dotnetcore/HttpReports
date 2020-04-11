using System;
using System.Runtime.CompilerServices;

using HttpReports;
using HttpReports.Storage.MySql;

[assembly: InternalsVisibleTo("HttpReports.Test")]

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder UseMySqlStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(builder.Configuration.GetSection("Storage"));
            return builder.UseMySqlStorageService();
        }

        public static IHttpReportsBuilder UseMySqlStorage(this IHttpReportsBuilder builder,Action<MySqlStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(options); 

            return builder.UseMySqlStorageService();
        }

        public static IHttpReportsBuilder UseMySqlStorageService(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IHttpReportsStorage, MySqlStorage>();
            builder.Services.AddSingleton<MySqlConnectionFactory>();

            return builder;

        }  
    }
}