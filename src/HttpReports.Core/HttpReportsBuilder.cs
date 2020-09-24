using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpReports
{ 

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