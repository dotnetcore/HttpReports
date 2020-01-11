using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpReports
{
    /// <summary>
    /// HttpReports构建器
    /// </summary>
    public interface IHttpReportsBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}