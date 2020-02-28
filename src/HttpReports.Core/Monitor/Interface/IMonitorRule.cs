using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 监控规则
    /// </summary>
    public interface IMonitorRule
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 规则标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 规则已应用的节点列表
        /// </summary>
        IList<string> Nodes { get; set; }

        /// <summary>
        /// 规则的所有监控
        /// </summary>
        IList<IMonitor> Monitors { get; set; }

        /// <summary>
        /// 通知的邮箱列表
        /// </summary>
        IList<string> NotificationEmails { get; set; }

        /// <summary>
        /// 通知的电话列表
        /// </summary>
        IList<string> NotificationPhoneNumbers { get; set; }
    }
}