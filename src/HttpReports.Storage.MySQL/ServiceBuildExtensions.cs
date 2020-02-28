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
            builder.Services.AddSingleton<IHttpReportsStorage, MySqlStorage>();
            builder.Services.AddSingleton<MySqlConnectionFactory>();

            return builder;
        }
    }
}