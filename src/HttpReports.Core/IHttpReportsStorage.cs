using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpReports.Core.Models;
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
        Task AddRequestInfoAsync(IRequestInfo request,IRequestDetail requestDetail);

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
         
        Task<bool> AddMonitorJob(IMonitorJob job);

        Task<bool> UpdateMonitorJob(IMonitorJob job);

        Task<bool> DeleteMonitorJob(string Id);

        Task<IMonitorJob> GetMonitorJob(string Id);

        Task<List<IMonitorJob>> GetMonitorJobs();  


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

        Task<SysUser> CheckLogin(string Username, string Password);

        Task<SysUser> GetSysUser(string UserName); 

        Task<bool> UpdateLoginUser(SysUser model);

        Task<(IRequestInfo,IRequestDetail)> GetRequestInfoDetail(string Id); 

        Task<IRequestInfo> GetRequestInfo(string Id);

        Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId);

        Task ClearData(string StartTime);

        Task SetLanguage(string Language);

        Task<string> GetSysConfig(string Key); 

    }
}