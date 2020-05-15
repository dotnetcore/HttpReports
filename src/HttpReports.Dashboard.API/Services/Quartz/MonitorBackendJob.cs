using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class MonitorBackendJob : IJob
    {
        private IHttpReportsStorage _storage;

        private MonitorService _monitorService;

        private IAlarmService _alarmService;

        private ILogger<MonitorBackendJob> _logger;

        private readonly LocalizeService _localizeService;
        private Localize Localize => _localizeService.Current;

        public MonitorBackendJob(IHttpReportsStorage storage,
                                 IAlarmService alarmService,
                                 MonitorService monitorService,
                                 ILogger<MonitorBackendJob> logger,
                                 LocalizeService localizeService)
        {
            _storage = storage;
            _alarmService = alarmService;
            _monitorService = monitorService;
            _logger = logger;
            _localizeService = localizeService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            IMonitorJob job = context.JobDetail.JobDataMap.Get("job") as IMonitorJob;

            MonitorJobPayload payload = JsonConvert.DeserializeObject<MonitorJobPayload>(job.Payload);

            //开始调用任务
            var response = GetCheckResponse(new List<Func<IMonitorJob, MonitorJobPayload, Task<AlarmOption>>> {
                CheckResponseTimeOutMonitor,
                CheckErrorResponseMonitor,
                CheckIPMonitor,
                CheckRequestCountMonitor
           }, job, payload);

            await AlarmAsync(response.Select(x => x.Result).ToList(), job);
        }

        private IEnumerable<Task<AlarmOption>> GetCheckResponse(List<Func<IMonitorJob, MonitorJobPayload, Task<AlarmOption>>> funcs, IMonitorJob job, MonitorJobPayload payload)
        {
            List<Task<AlarmOption>> alarmOptions = new List<Task<AlarmOption>>();

            foreach (var func in funcs)
            {
                alarmOptions.Add(Task.Run(() => func.Invoke(job, payload)));
            }

            return alarmOptions;
        }

        private async Task AlarmAsync(IList<AlarmOption> alarmOption, IMonitorJob job)
        {
            foreach (var item in alarmOption)
            {
                if (item != null)
                {
                    item.Emails = job.Emails?.Split(',').AsEnumerable();
                    item.Phones = job.Mobiles?.Split(',').AsEnumerable();
                    item.WebHook = job.WebHook;

                    await _alarmService.AlarmAsync(item);
                }
            }
        }

        /// <summary>
        /// 检查超时监控
        /// </summary>
        /// <returns></returns>
        private async Task<AlarmOption> CheckResponseTimeOutMonitor(IMonitorJob job, MonitorJobPayload payload)
        {
            if (payload.ResponseTimeOutMonitor == null)
            {
                return null;
            }

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            _logger.LogInformation("CheckResponseTimeOutMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var timeoutCount = await _storage.GetTimeoutResponeCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }, payload.ResponseTimeOutMonitor.TimeOutMs);

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            });

            if (count == 0)
            {
                return null;
            }

            var percent = timeoutCount * 100.0 / count;

            _logger.LogInformation("CheckResponseTimeOutMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  Result {(percent > payload.ResponseTimeOutMonitor.Percentage ? "预警" : "正常")}");

            if (percent > payload.ResponseTimeOutMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{Localize.Monitor_Type_Timeout}】 </b>

                          <p> {Localize.Warning_Threshold}：{payload.ResponseTimeOutMonitor.Percentage.ToString("F2")}%  {Localize.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{Localize.Monitor_Title}：{job.Title}</p>

                          <p>{Localize.Monitor_ServiceNode}：{job.Nodes}</p>

                          <p>{Localize.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{Localize.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }

            return null;
        }

        /// <summary>
        /// 检查请求错误监控
        /// </summary>
        /// <param name="job"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<AlarmOption> CheckErrorResponseMonitor(IMonitorJob job, MonitorJobPayload payload)
        {
            if (payload.ErrorResponseMonitor == null)
            {
                return null;
            }

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            _logger.LogInformation("CheckErrorResponseMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var errorCount = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
                StatusCodes = payload.ErrorResponseMonitor.HttpCodeStatus.Split(',').Select(x => x.ToInt()).ToArray()
            });

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            });

            if (count == 0)
            {
                return null;
            }
            var percent = errorCount * 100.0 / count;

            _logger.LogInformation("CheckErrorResponseMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  Result {(percent > payload.ErrorResponseMonitor.Percentage ? "预警" : "正常")}");

            if (percent > payload.ErrorResponseMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{Localize.Monitor_Type_RequestError}】 </b>

                          <p>{Localize.Warning_Threshold}：{payload.ErrorResponseMonitor.Percentage.ToString("F2")}%  {Localize.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{Localize.Warning_Title}：{job.Title}</p>

                          <p>{Localize.Monitor_ServiceNode}：{job.Nodes}</p>

                          <p>{Localize.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{Localize.Monitor_HttpStatusCode}：{payload.ErrorResponseMonitor.HttpCodeStatus}</p>

                          <p>{Localize.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        private async Task<AlarmOption> CheckIPMonitor(IMonitorJob job, MonitorJobPayload payload)
        {
            if (payload.IPMonitor == null)
            {
                return null;
            }

            _logger.LogInformation("CheckIPMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            var (max, count) = await _storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
                InList = false,
                List = payload.IPMonitor.WhiteList.Split(',').ToArray(),
            });

            if (count == 0)
            {
                return null;
            }
            var percent = max * 100.0 / count;

            _logger.LogInformation("CheckIPMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  Result {(percent > payload.IPMonitor.Percentage ? "预警" : "正常")}");

            if (percent > payload.IPMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{Localize.Monitor_Type_IP}】 </b>

                          <p>{Localize.Warning_Threshold}：{payload.IPMonitor.Percentage.ToString("F2")}% {Localize.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{Localize.Warning_Title}：{job.Title}</p>

                          <p>{Localize.Monitor_ServiceNode}：{job.Nodes}</p>

                          <p>{Localize.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{Localize.Monitor_IPWhiteList}：{payload.IPMonitor.WhiteList}</p>

                          <p>{Localize.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        private async Task<AlarmOption> CheckRequestCountMonitor(IMonitorJob job, MonitorJobPayload payload)
        {
            if (payload.RequestCountMonitor == null)
            {
                return null;
            }

            _logger.LogInformation("CheckRequestCountMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));
            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            });

            _logger.LogInformation("CheckRequestCountMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  Result {(count > payload.RequestCountMonitor.Max ? "预警" : "正常")}");

            if (count > payload.RequestCountMonitor.Max)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{Localize.Monitor_Type_RequestCount}】 </b>

                          <p>{Localize.Warning_Threshold}：{payload.RequestCountMonitor.Max}  {Localize.Warning_Current}：{count} </p>

                          <p>{Localize.Warning_Title}：{job.Title}</p>

                          <p>{Localize.Monitor_ServiceNode}：{job.Nodes}</p>

                          <p>{Localize.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{Localize.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }

            return null;
        }

        /// <summary>
        /// 获取基于监控间隔的时间
        /// </summary>
        /// <returns></returns>
        protected (DateTime now, DateTime start, DateTime end) GetNowTimes(int minute)
        {
            var now = DateTime.Now;
            return (now, now.AddMinutes(-minute), now);
        }
    }
}