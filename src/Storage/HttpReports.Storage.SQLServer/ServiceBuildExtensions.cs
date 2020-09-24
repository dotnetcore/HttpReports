using HttpReports;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.SQLServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder UseSQLServerStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(builder.Configuration.GetSection("Storage"));

            return builder.UseSQLServerStorageService();
        }

        public static IHttpReportsBuilder UseSQLServerStorage(this IHttpReportsBuilder builder,Action<SQLServerStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(options);

            return builder.UseSQLServerStorageService();
        }

        private static IHttpReportsBuilder UseSQLServerStorageService(this IHttpReportsBuilder builder)
        { 
            builder.Services.AddTransient<IHttpReportsStorage, SQLServerStorage>();

            return builder;
        }

    }
}