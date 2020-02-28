using System.Diagnostics;

using Microsoft.AspNetCore.Http;

namespace HttpReports
{
    /// <summary>
    /// 请求信息构建器
    /// </summary>
    public interface IRequestInfoBuilder
    {
        /// <summary>
        /// 构建请求信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        IRequestInfo Build(HttpContext context, Stopwatch stopwatch);
    }
}