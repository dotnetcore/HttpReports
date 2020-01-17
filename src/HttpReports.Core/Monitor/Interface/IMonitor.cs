namespace HttpReports.Monitor
{
    /// <summary>
    /// 监控接口
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 所属监控规则Id
        /// </summary>
        int RuleId { get; set; }

        /// <summary>
        /// 监控类型
        /// </summary>
        MonitorType Type { get; }

        /// <summary>
        /// 监控描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        string CronExpression { get; set; }
    }
}