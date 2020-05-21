namespace HttpReports.Dashboard.ViewModels
{
    public class MonitorJobRequest
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Nodes { get; set; }

        public string Emails { get; set; }

        public string WebHook { get; set; }

        public string Mobiles { get; set; }

        public int Interval { get; set; }

        public int Status { get; set; }

        public ResponseTimeOutMonitorViewModel ResponseTimeOutMonitor { get; set; }

        public ErrorResponseMonitorViewModel ErrorResponseMonitor { get; set; }

        public IPMonitorViewModel IPMonitor { get; set; }

        public RequestCountMonitorViewModel RequestCountMonitor { get; set; }
    }

    /// <summary>
    /// 响应超时监控
    /// </summary>
    public class ResponseTimeOutMonitorViewModel
    {
        public int Status { get; set; }

        public string TimeOutMs { get; set; }

        public string Percentage { get; set; }
    }

    /// <summary>
    /// 请求错误监控
    /// </summary>
    public class ErrorResponseMonitorViewModel
    {
        public int Status { get; set; }

        public string HttpCodeStatus { get; set; }

        public string Percentage { get; set; }
    }

    /// <summary>
    /// IP监控
    /// </summary>
    public class IPMonitorViewModel
    {
        public int Status { get; set; }

        public string WhiteList { get; set; }

        public string Percentage { get; set; }
    }

    /// <summary>
    /// 请求量过多监控
    /// </summary>
    public class RequestCountMonitorViewModel
    {
        public int Status { get; set; }

        public string Max { get; set; }
    }
}