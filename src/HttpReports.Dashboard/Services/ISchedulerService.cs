using System.Threading.Tasks;

using HttpReports.Monitor;

namespace HttpReports.Dashboard.Services
{
    /// <summary>
    /// 计划任务Service
    /// </summary>
    public interface ISchedulerService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        /// 添加监控规则到计划任务执行
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        Task AddMonitorRuleAsync(IMonitorRule rule);

        /// <summary>
        /// 更新计划任务的监控规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        Task UpdateMonitorRuleAsync(IMonitorRule rule);

        /// <summary>
        /// 删除监控规则的计划任务
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        Task DeleteMonitorRuleAsync(int ruleId);
    }
}