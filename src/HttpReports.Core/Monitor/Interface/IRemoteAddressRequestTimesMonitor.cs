using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 单个远程地址请求量监控
    /// </summary>
    public interface IRemoteAddressRequestTimesMonitor : IMonitor
    {
        /// <summary>
        /// 白名单
        /// </summary>
        IList<string> WhiteList { get; set; }

        /// <summary>
        /// 警告百分比
        /// </summary>
        float WarningPercentage { get; set; }
    }
}