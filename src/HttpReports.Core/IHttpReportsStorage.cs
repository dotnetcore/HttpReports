using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Storage.FilterOptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Core
{
    public interface IHttpReportsStorage
    {
        Task InitAsync();


        Task AddRequestInfoAsync(RequestBag bag);


        Task AddRequestInfoAsync(List<RequestBag> list, CancellationToken token);


        Task<IndexPageData> GetIndexBasicDataAsync(BasicFilter filter);

        Task<List<BaseTimeModel>> GetServiceTrend(BasicFilter filter, List<string> Time);

        Task<List<BaseTimeModel>> GetServiceHeatMap(BasicFilter filter, List<string> Time);


        Task<IEnumerable<string>> GetTopServiceLoad(BasicFilter filter);


        Task<List<List<TopServiceResponse>>> GetGroupData(BasicFilter filter, GroupType groupType);


        Task<List<BaseNode>> GetTopologyData(BasicFilter filter);



        Task<List<APPTimeModel>> GetAppStatus(BasicFilter filter, List<string> range); 
 

        Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime);

        #region Statistics


        Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption);


        Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption);

        Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption);


        Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption filterOption);


        Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption);

        
        Task<RequestInfoSearchResult> GetSearchRequestInfoAsync(QueryDetailFilter filter);

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

        Task<bool> AddMonitorJob(MonitorJob job); 

        Task<List<Performance>> GetPerformances(PerformanceFilterIOption option);

        Task<bool> UpdateMonitorJob(MonitorJob job);

        Task<bool> DeleteMonitorJob(string Id);

        Task<MonitorJob> GetMonitorJob(string Id);

        Task<List<MonitorJob>> GetMonitorJobs();


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

        Task<(RequestInfo, RequestDetail)> GetRequestInfoDetail(string Id);

        Task<RequestInfo> GetRequestInfo(string Id);

        Task<List<RequestInfo>> GetRequestInfoByParentId(string ParentId);

        Task ClearData(string StartTime);

        Task SetLanguage(string Language);

        Task<string> GetSysConfig(string Key);

        Task<bool> AddPerformanceAsync(Performance performance);

    }
}
