using HttpReports;
using HttpReports.Storage.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuildExtensions
    {
        public static IHttpReportsBuilder UseElasticSearchStorage(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.Configure<ElasticSearchStorageOptions>(builder.Configuration.GetSection("Storage"));
            builder.Services.AddSingleton<IHttpReportsStorage, ElasticSearchStorage>();
            builder.Services.AddSingleton<ElasticSearchConnectionFactory>();

            return builder;
        }
    }
}