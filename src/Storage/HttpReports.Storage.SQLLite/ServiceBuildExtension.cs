using HttpReports;
using HttpReports.Storage.SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtension
    {
        public static IHttpReportsBuilder UseSQLiteStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLiteStorageOptions>(builder.Configuration.GetSection("Storage"));
            return builder.UseSQLiteStorageService();
        }

        public static IHttpReportsBuilder UseSQLiteStorage(this IHttpReportsBuilder builder, Action<SQLiteStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLiteStorageOptions>(options);

            return builder.UseSQLiteStorageService();
        }

        internal static IHttpReportsBuilder UseSQLiteStorageService(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IHttpReportsStorage, SQLiteStorage>();
            builder.Services.AddSingleton<SQLiteConnectionFactory>();

            return builder.UseDirectlyReportsTransport();

        }

    }
}
