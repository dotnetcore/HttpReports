namespace HttpReports.Monitor
{
    /// <summary>
    /// 响应超时监控接口
    /// </summary>
    public interface IResponseTimeOutMonitor : IMonitor
    {
        /// <summary>
        /// 超时阈值
        /// </summary>
        int TimeoutThreshold { get; set; }

        /// <summary>
        /// 警告百分比
        /// </summary>
        float WarningPercentage { get; set; }
    }
}