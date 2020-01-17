using System.Collections.Generic;
using System.Threading.Tasks;

using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

namespace HttpReports
{
    /// <summary>
    /// 储存库接口
    /// </summary>
    public interface IHttpReportsStorage
    {
        /// <summary>
        /// 初始化储存库
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        /// 添加一条请求记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddRequestInfoAsync(IRequestInfo request);

        /// <summary>
        /// 获取所有节点信息
        /// </summary>
        /// <returns></returns>
        Task<List<NodeInfo>> GetNodesAsync();

        #region Statistics

        /// <summary>
        /// 获取Url请求统计
        /// </summary>
        /// <returns></returns>
        Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption);

        /// <summary>
        /// 获取Url的平均响应时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption);

        /// <summary>
        /// 获取http状态码数量统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption);

        /// <summary>
        /// 获取Url的响应时间分组统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption filterOption);

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption);

        /// <summary>
        /// 搜索请求信息
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption);

        /// <summary>
        /// 获取请求次数统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption);

        /// <summary>
        /// 获取响应时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption filterOption);

        #endregion Statistics

        #region Monitor

        /// <summary>
        /// 添加监控规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        Task<bool> AddMonitorRuleAsync(IMonitorRule rule);

        /// <summary>
        /// 更新监控规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        Task<bool> UpdateMonitorRuleAsync(IMonitorRule rule);

        /// <summary>
        /// 删除监控规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteMonitorRuleAsync(int ruleId);

        /// <summary>
        /// 获取指定监控规则
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        Task<IMonitorRule> GetMonitorRuleAsync(int ruleId);

        /// <summary>
        /// 获取所有监控规则
        /// </summary>
        /// <returns></returns>
        Task<List<IMonitorRule>> GetAllMonitorRulesAsync();

        #region Query

        /// <summary>
        /// 获取请求总次数
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption);

        /// <summary>
        /// 依据白名单获取请求次数
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption);

        /// <summary>
        /// 获取超时响应统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <param name="timeoutThreshold"></param>
        /// <returns></returns>
        Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold);

        #endregion Query

        #endregion Monitor

        //TODO 定义所有数据存取接口
    }
}