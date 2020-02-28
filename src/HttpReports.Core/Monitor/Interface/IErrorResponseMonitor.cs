using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 响应错误状态监控
    /// </summary>
    public interface IErrorResponseMonitor : IMonitor
    {
        /// <summary>
        /// 状态码列表
        /// </summary>
        IList<int> StatusCodes { get; set; }

        /// <summary>
        /// 警告百分比
        /// </summary>
        float WarningPercentage { get; set; }
    }
}