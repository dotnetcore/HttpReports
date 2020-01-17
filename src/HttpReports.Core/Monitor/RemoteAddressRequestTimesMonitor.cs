using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 单个远程地址请求量监控
    /// </summary>
    public class RemoteAddressRequestTimesMonitor : IRemoteAddressRequestTimesMonitor
    {
        public int Id { get; set; }

        public IList<string> WhileList { get; set; }

        public float WarningPercentage { get; set; }

        public int RuleId { get; set; }

        public MonitorType Type => MonitorType.ToManyRequestBySingleRemoteAddress;

        public string Description { get; set; }

        public string CronExpression { get; set; }
    }
}