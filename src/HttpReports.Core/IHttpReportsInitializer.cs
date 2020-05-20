using Microsoft.AspNetCore.Builder;

namespace HttpReports
{
    /// <summary>
    /// HttpReports初始化器
    /// </summary>
    public interface IHttpReportsInitializer
    {
        IApplicationBuilder ApplicationBuilder { get; }
    }
}