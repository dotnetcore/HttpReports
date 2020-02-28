using HttpReports;
using HttpReports.Storage.SQLServer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder UseSQLServerStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<SQLServerStorageOptions>(builder.Configuration.GetSection("Storage"));
            builder.Services.AddTransient<IHttpReportsStorage, SQLServerStorage>();
            builder.Services.AddSingleton<SQLServerConnectionFactory>();
            builder.Services.AddSingleton<IModelCreator, ModelCreator>();
            return builder;
        }
    }
}