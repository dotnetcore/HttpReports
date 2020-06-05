using HttpReports.Core.Interface;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.SQLite
{
    public class SQLiteStorage: IHttpReportsStorage
    {
        public SQLiteStorageOptions Options { get; }

        public SQLiteConnectionFactory ConnectionFactory { get; }

        public ILogger<SQLiteStorage> Logger { get; }

        private readonly AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail> _deferFlushCollection = null;

        public SQLiteStorage(IOptions<SQLiteStorageOptions> options, SQLiteConnectionFactory connectionFactory, ILogger<SQLiteStorage> logger)
        {
            Options = options.Value;
            ConnectionFactory = connectionFactory;
            Logger = logger;
            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferSecond);
            }
        }

        public Task InitAsync()
        {
            throw new NotImplementedException();
        }

        public Task AddRequestInfoAsync(Dictionary<IRequestInfo, IRequestDetail> list, CancellationToken token)
        {
            throw new NotImplementedException();

        }

        public Task AddRequestInfoAsync(IRequestInfo request, IRequestDetail requestDetail)
        {
            throw new NotImplementedException();
        }

        public Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddMonitorJob(IMonitorJob job)
        {
            throw new NotImplementedException();
        }

        public Task<List<IPerformance>> GetPerformances(PerformanceFilterIOption option)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMonitorJob(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<IMonitorJob> GetMonitorJob(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<IMonitorJob>> GetMonitorJobs()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            throw new NotImplementedException();
        }

        public Task<SysUser> CheckLogin(string Username, string Password)
        {
            throw new NotImplementedException();
        }

        public Task<SysUser> GetSysUser(string UserName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateLoginUser(SysUser model)
        {
            throw new NotImplementedException();
        }

        public Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<IRequestInfo> GetRequestInfo(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            throw new NotImplementedException();
        }

        public Task ClearData(string StartTime)
        {
            throw new NotImplementedException();
        }

        public Task SetLanguage(string Language)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSysConfig(string Key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPerformanceAsync(IPerformance performance)
        {
            throw new NotImplementedException();
        }
    }
}
