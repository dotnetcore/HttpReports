namespace HttpReports.Monitor
{
    /// <summary>
    /// 请求次数监控
    /// </summary>
    public class RequestTimesMonitor : IRequestTimesMonitor
    {
        public int Id { get; set; }
        public int RuleId { get; set; }

        public MonitorType Type => MonitorType.ToManyRequestWithAddress;

        public string Description { get; set; }

        public string CronExpression { get; set; }

        /// <summary>
        /// 警告阈值
        /// </summary>
        public int Max { get; set; }
        public int WarningThreshold { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}