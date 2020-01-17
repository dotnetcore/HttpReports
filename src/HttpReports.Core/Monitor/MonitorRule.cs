using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 监控规则
    /// </summary>
    public class MonitorRule : IMonitorRule
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 规则标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 规则已应用的节点列表
        /// </summary>
        public IList<string> Nodes { get; set; } = new List<string>();

        /// <summary>
        /// 规则的所有监控
        /// </summary>
        public IList<IMonitor> Monitors { get; set; } = new List<IMonitor>();

        public IList<string> NotificationEmails { get; set; } = new List<string>();

        public IList<string> NotificationPhoneNumbers { get; set; } = new List<string>();
    }
}