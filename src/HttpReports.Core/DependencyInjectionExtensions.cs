using System;

using HttpReports;

using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsInitializer InitHttpReports(this IApplicationBuilder app)
        {
            return new HttpReportsInitializer(app);
        }

        public static IHttpReportsInitializer InitStorage(this IHttpReportsInitializer initializer)
        {
            var storage = initializer.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Service Not Found");
            storage.InitAsync().Wait();

            return initializer;
        }
    }
}