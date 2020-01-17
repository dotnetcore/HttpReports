namespace HttpReports.Monitor
{
    /// <summary>
    /// 响应超时监控接口
    /// </summary>
    public class ResponseTimeOutMonitor : IResponseTimeOutMonitor
    {
        public int Id { get; set; }
        public int RuleId { get; set; }

        public int TimeoutThreshold { get; set; }

        public float WarningPercentage { get; set; }

        public MonitorType Type => MonitorType.ResponseTimeOut;

        public string Description { get; set; }

        public string CronExpression { get; set; }
    }
}