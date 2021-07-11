using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Core.StorageFilters;
using HttpReports.Core.ViewModels; 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.Abstractions
{
    public interface IHttpReportsStorage
    {
        Task InitAsync();

        Task PrintSQLAsync();

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

        Task<bool> DeleteMonitorJob(long Id);

        Task<MonitorJob> GetMonitorJob(long Id);

        Task<List<string>> GetEndpoints(BasicFilter filter);

        Task<List<MonitorJob>> GetMonitorJobs(); 

        Task<(int timeout, int total)> GetTimeoutResponeCountAsync(ResponseTimeTaskFilter filter);

        Task<(int error, int total)> GetErrorResponeCountAsync(ResponseErrorTaskFilter filter);

        Task<int> GetCallCountAsync(CallCountTaskFilter filter);


        Task<SysUser> CheckLogin(string Username, string Password);

        Task<SysUser> GetSysUser(string UserName);

        Task<bool> UpdateLoginUser(SysUser model);

        Task<(RequestInfo, RequestDetail)> GetRequestInfoDetail(long Id);

        Task<RequestInfo> GetRequestInfo(long Id);

        Task<List<RequestInfo>> GetRequestInfoByParentId(long ParentId);

        Task ClearData(string StartTime);

        Task SetLanguage(string Language);

        Task<string> GetSysConfig(string Key);

        Task<bool> AddPerformanceAsync(Performance performance);

        Task<bool> GetPerformanceAsync(DateTime start,DateTime end,string Service,string Instance);  


    }
}
