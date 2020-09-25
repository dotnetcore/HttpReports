using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpReports
{ 
    public interface IHttpReportsBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}