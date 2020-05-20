using HttpReports.Monitor;

namespace HttpReports
{
    /// <summary>
    /// model创建器，用于创建储存库特有的有特性标签的对象
    /// </summary>
    public interface IModelCreator
    {
        /// <summary>
        /// 创建请求信息
        /// </summary>
        /// <returns></returns>
        IRequestInfo CreateRequestInfo();

        /// <summary>
        /// 创建请求详情
        /// </summary>
        /// <returns></returns>
        IRequestDetail CreateRequestDetail();

        /// <summary>
        /// 创建监控
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IMonitor CreateMonitor(MonitorType type);

        /// <summary>
        /// 创建监控规则
        /// </summary>
        /// <returns></returns>
        IMonitorRule CreateMonitorRule();
    }
}