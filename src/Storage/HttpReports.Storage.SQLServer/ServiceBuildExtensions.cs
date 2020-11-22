using HttpReports;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.SQLServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder AddSQLServerStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(builder.Configuration.GetSection("Storage"));

            return builder.AddSQLServerStorageService();
        }

        public static IHttpReportsBuilder AddSQLServerStorage(this IHttpReportsBuilder builder,Action<SQLServerStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(options);

            return builder.AddSQLServerStorageService();
        }

        private static IHttpReportsBuilder AddSQLServerStorageService(this IHttpReportsBuilder builder)
        { 
            builder.Services.AddSingleton<IHttpReportsStorage, SQLServerStorage>();

            return builder;
        }


        [Obsolete("Use AddSQLServerStorage instead")]
        public static IHttpReportsBuilder UseSQLServerStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(builder.Configuration.GetSection("Storage"));

            return builder.AddSQLServerStorageService();
        }


        [Obsolete("Use AddSQLServerStorage instead")]
        public static IHttpReportsBuilder UseSQLServerStorage(this IHttpReportsBuilder builder, Action<SQLServerStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(options);

            return builder.AddSQLServerStorageService();
        }



    }
}