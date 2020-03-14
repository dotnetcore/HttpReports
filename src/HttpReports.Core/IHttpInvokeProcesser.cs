using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace HttpReports
{
    /// <summary>
    /// http调用信息处理器
    /// </summary>
    public interface IHttpInvokeProcesser
    {
        /// <summary>
        /// 处理调用信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stopwatch">调用计时器</param>
        void Process(HttpContext context, Stopwatch stopwatch);
    }
}