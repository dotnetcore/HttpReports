using HttpReports.Core.Models; 
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models; 
using System;
using System.Collections.Generic; 
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


        Task<RequestInfoSearchResult> GetSearchRequestInfoAsync(QueryDetailFilter filter);

        Task<bool> AddMonitorJob(MonitorJob job);

        Task<bool> AddMonitorAlarm(MonitorAlarm alarm);

        Task<List<MonitorAlarm>> GetMonitorAlarms(BasicFilter filter);

        Task<bool> UpdateMonitorJob(MonitorJob job);

        Task<bool> DeleteMonitorJob(string Id);

        Task<MonitorJob> GetMonitorJob(string Id);

        Task<List<MonitorJob>> GetMonitorJobs();


        Task<(int timeout, int total)> GetTimeoutResponeCountAsync(ResponseTimeTaskFilter filter);

        Task<(int error, int total)> GetErrorResponeCountAsync(ResponseErrorTaskFilter filter);

        Task<int> GetCallCountAsync(CallCountTaskFilter filter);


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
