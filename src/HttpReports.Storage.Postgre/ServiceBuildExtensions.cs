using HttpReports;
using HttpReports.Storage.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder UsePostgreSQLStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<PostgreStorageOptions>(builder.Configuration.GetSection("Storage"));
            builder.Services.AddSingleton<IHttpReportsStorage,PostgreSQLStorage>();
            builder.Services.AddSingleton<PostgreConnectionFactory>();

            return builder;
        }
    }
}
