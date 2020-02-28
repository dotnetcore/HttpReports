using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 响应错误状态监控
    /// </summary>
    public class ErrorResponseMonitor : IErrorResponseMonitor
    {
        public IList<int> StatusCodes { get; set; }
        public float WarningPercentage { get; set; }
        public int Id { get; set; }
        public int RuleId { get; set; }

        public MonitorType Type => MonitorType.ErrorResponse;

        public string Description { get; set; }

        public string CronExpression { get; set; }
    }
}