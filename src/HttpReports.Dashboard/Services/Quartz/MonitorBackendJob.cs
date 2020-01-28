using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class MonitorBackendJob : IJob
    {
        private IHttpReportsStorage _storage;

        private MonitorService _monitorService;

        private IAlarmService _alarmService;

        public MonitorBackendJob()
        { 

        }


        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _alarmService = _alarmService ?? ServiceContainer.provider.GetService(typeof(IAlarmService)) as IAlarmService;
            _monitorService = _monitorService ?? ServiceContainer.provider.GetService(typeof(MonitorService)) as MonitorService;


            IMonitorJob job = context.JobDetail.JobDataMap.Get("job") as IMonitorJob;

            MonitorJobPayload payload = JsonConvert.DeserializeObject<MonitorJobPayload>(job.Payload);

            if (payload.ResponseTimeOutMonitor != null)
            {
                 AlarmOption alarmOption = await CheckResponseTimeOutMonitor(job, payload);

                 await AlarmAsync(alarmOption,job);
            }

            if (payload.ErrorResponseMonitor != null)
            {
                AlarmOption alarmOption = await CheckErrorResponseMonitor(job, payload);

                await AlarmAsync(alarmOption, job);
            }

            if (payload.IPMonitor != null)
            {
                AlarmOption alarmOption = await CheckIPMonitor(job, payload);

                await AlarmAsync(alarmOption, job);
            }

            if (payload.RequestCountMonitor != null)
            {
                AlarmOption alarmOption = await CheckRequestCountMonitor(job, payload);

                await AlarmAsync(alarmOption, job);
            }  
        }

        private async Task AlarmAsync(AlarmOption alarmOption,IMonitorJob job)
        {
            if (alarmOption != null)
            {
                alarmOption.Emails = job.Emails.Split(',').AsEnumerable();
                alarmOption.Phones = job.Mobiles.Split(',').AsEnumerable();
                await _alarmService.AlarmAsync(alarmOption);
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

            var timeoutCount = await _storage.GetTimeoutResponeCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }, payload.ResponseTimeOutMonitor.TimeOutMs).ConfigureAwait(false);

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }

            var percent = timeoutCount * 100.0 / count;

            if (percent > payload.ResponseTimeOutMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【响应超时】触发预警 </b>

                          <p>超时率预警值：{payload.ResponseTimeOutMonitor.Percentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Nodes}</p>

                          <p>监控频率：{_monitorService.ParseJobCron(job.CronLike)} 分钟</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
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

            var errorCount = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
                StatusCodes = payload.ErrorResponseMonitor.HttpCodeStatus.Split(',').Select(x => x.ToInt()).ToArray()
            }).ConfigureAwait(false);

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }
            var percent = errorCount * 100.0 / count;

            if (percent > payload.ErrorResponseMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【请求错误】触发预警 </b>

                          <p>命中率预警值：{payload.ErrorResponseMonitor.Percentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Nodes}</p>

                          <p>监控频率：{_monitorService.ParseJobCron(job.CronLike)} 分钟</p>

                          <p>设定Http状态码：{payload.ErrorResponseMonitor.HttpCodeStatus}</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }


        /// <summary>
        /// 检查IP异常监控
        /// </summary>
        /// <param name="job"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<AlarmOption> CheckIPMonitor(IMonitorJob job, MonitorJobPayload payload)
        { 
            if (payload.IPMonitor == null)
            {
                return null;
            } 

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            var (max, count) = await _storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
                InList = false,
                List = payload.IPMonitor.WhileList.Split(',').ToArray(),
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }
            var percent = max * 100.0 / count;

            if (percent > payload.IPMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【IP异常】触发预警 </b>

                          <p>IP重复率预警值：{payload.IPMonitor.Percentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Nodes}</p>

                          <p>监控频率：{_monitorService.ParseJobCron(job.CronLike)} 分钟</p>

                          <p>IP白名单：{payload.IPMonitor.WhileList}</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;


        }


        /// <summary>
        /// 请求量监控
        /// </summary>
        /// <param name="job"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<AlarmOption> CheckRequestCountMonitor(IMonitorJob job, MonitorJobPayload payload)
        { 
            if (payload.RequestCountMonitor == null)
            {
                return null;
            }

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));
            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count > payload.RequestCountMonitor.Max)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【请求量监控】触发预警 </b>

                          <p>请求量最大预警值：{payload.RequestCountMonitor.Max}  当前值：{count} </p>

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Nodes}</p>

                          <p>监控频率：{_monitorService.ParseJobCron(job.CronLike)} 分钟</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
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
