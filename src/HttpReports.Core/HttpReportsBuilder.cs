using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpReports
{
    /// <summary>
    /// HttpReports构建器（其实是个存放构建时上下文的）
    /// </summary>
    public class HttpReportsBuilder : IHttpReportsBuilder
    {
        public IServiceCollection Services { get; private set; }

        public IConfiguration Configuration { get; private set; }

        public HttpReportsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }
    }
}