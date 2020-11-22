using System;
using System.Runtime.CompilerServices;

using HttpReports;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.MySql;

[assembly: InternalsVisibleTo("HttpReports.Test")]

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder AddMySqlStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(builder.Configuration.GetSection("Storage"));
            return builder.AddMySqlStorageService();
        }

        public static IHttpReportsBuilder AddMySqlStorage(this IHttpReportsBuilder builder,Action<MySqlStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(options); 

            return builder.AddMySqlStorageService();
        }

        internal static IHttpReportsBuilder AddMySqlStorageService(this IHttpReportsBuilder builder)
        {  
            builder.Services.AddSingleton<IHttpReportsStorage, MySqlStorage>();

            return builder;

        }

        [Obsolete("Use AddMySqlStorage instead")]
        public static IHttpReportsBuilder UseMySqlStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(builder.Configuration.GetSection("Storage"));
            return builder.AddMySqlStorageService();
        }


        [Obsolete("Use AddMySqlStorage instead")]
        public static IHttpReportsBuilder UseMySqlStorage(this IHttpReportsBuilder builder, Action<MySqlStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<MySqlStorageOptions>(options);

            return builder.AddMySqlStorageService();
        }



    }
}