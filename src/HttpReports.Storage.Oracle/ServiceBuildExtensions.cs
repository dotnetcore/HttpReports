using HttpReports;
using HttpReports.Storage.Oracle;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OracleBuildExtensions
    {
        public static IHttpReportsBuilder UseOracleStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<OracleStorageOptions>(builder.Configuration.GetSection("Storage"));
            builder.Services.AddTransient<IHttpReportsStorage, OracleStorage>();
            builder.Services.AddSingleton<OracleConnectionFactory>();
            builder.Services.AddSingleton<IModelCreator, ModelCreator>();
            return builder;
        }
    }
}