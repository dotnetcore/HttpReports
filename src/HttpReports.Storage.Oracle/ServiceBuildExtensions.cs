using HttpReports;
using HttpReports.Storage.Oracle;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OracleBuildExtensions
    {
        public static IHttpReportsBuilder UseOracleStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<OracleStorageOptions>(builder.Configuration.GetSection("Storage"));
            return builder.UseOracleStorageService();
        }

        public static IHttpReportsBuilder UseOracleStorage(this IHttpReportsBuilder builder,Action<OracleStorageOptions> options)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<OracleStorageOptions>(options);
            
            return builder.UseOracleStorageService();
        }

        public static IHttpReportsBuilder UseOracleStorageService(this IHttpReportsBuilder builder)
        { 
            builder.Services.AddTransient<IHttpReportsStorage, OracleStorage>();
            builder.Services.AddSingleton<OracleConnectionFactory>();
            builder.Services.AddSingleton<IModelCreator, ModelCreator>();
            return builder;
        }

    }
}