using Microsoft.AspNetCore.Builder;

namespace HttpReports
{
    /// <summary>
    /// HttpReports初始化器（其实是个存放构建时上下文的）
    /// </summary>
    public class HttpReportsInitializer : IHttpReportsInitializer
    {
        public IApplicationBuilder ApplicationBuilder { get; }

        public HttpReportsInitializer(IApplicationBuilder builder)
        {
            ApplicationBuilder = builder;
        }
    }
}