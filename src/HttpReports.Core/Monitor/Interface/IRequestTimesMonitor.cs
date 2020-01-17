namespace HttpReports.Monitor
{
    /// <summary>
    /// 请求次数监控
    /// </summary>
    public interface IRequestTimesMonitor : IMonitor
    {
        /// <summary>
        /// 警告阈值
        /// </summary>
        int WarningThreshold { get; set; }
    }
}