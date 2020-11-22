using HttpReports;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder AddPostgreSQLStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<PostgreStorageOptions>(builder.Configuration.GetSection("Storage"));

            return builder.AddPostgreSQLStorageService();
        }

        public static IHttpReportsBuilder AddPostgreSQLStorage(this IHttpReportsBuilder builder,Action<PostgreStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<PostgreStorageOptions>(options); 

            return builder.AddPostgreSQLStorageService();
        }

        internal static IHttpReportsBuilder AddPostgreSQLStorageService(this IHttpReportsBuilder builder)
        { 
            builder.Services.AddSingleton<IHttpReportsStorage, PostgreSQLStorage>();

            return builder;
        } 


        [Obsolete("Use AddPostgreSQLStorage instead")]
        public static IHttpReportsBuilder UsePostgreSQLStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<PostgreStorageOptions>(builder.Configuration.GetSection("Storage"));

            return builder.AddPostgreSQLStorageService();
        }


        [Obsolete("Use AddPostgreSQLStorage instead")]
        public static IHttpReportsBuilder UsePostgreSQLStorage(this IHttpReportsBuilder builder, Action<PostgreStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<PostgreStorageOptions>(options);

            return builder.AddPostgreSQLStorageService();
        } 

    }
}
