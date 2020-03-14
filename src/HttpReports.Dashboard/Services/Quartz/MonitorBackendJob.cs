using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class MonitorBackendJob : IJob
    {
        private IHttpReportsStorage _storage;

        private MonitorService _monitorService;

        private IAlarmService _alarmService;

        private ILogger<MonitorBackendJob> _logger;

        public MonitorBackendJob()
        {

        }


        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _alarmService = _alarmService ?? ServiceContainer.provider.GetService(typeof(IAlarmService)) as IAlarmService;
            _monitorService = _monitorService ?? ServiceContainer.provider.GetService(typeof(MonitorService)) as MonitorService;
            _logger = _logger ?? ServiceContainer.provider.GetService(typeof(ILogger<MonitorBackendJob>)) as ILogger<MonitorBackendJob>;


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

            _logger.LogInformation("检查超时监控开始 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

            _logger.LogInformation("检查超时监控结束  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  检查结果 {(percent > payload.ResponseTimeOutMonitor.Percentage ? "预警":"正常")}");

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

            _logger.LogInformation("检查请求错误监控开始 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

            _logger.LogInformation("检查请求错误监控结束  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  检查结果 {(percent > payload.ErrorResponseMonitor.Percentage ? "预警" : "正常")}");

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

            _logger.LogInformation("检查IP异常监控开始 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

            _logger.LogInformation("检查IP异常监控结束  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  检查结果 {(percent > payload.IPMonitor.Percentage ? "预警" : "正常")}");

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

            _logger.LogInformation("检查请求量监控开始 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));
            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = job.Nodes.Split(','),
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            _logger.LogInformation("检查请求量监控结束  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  检查结果 {(count > payload.RequestCountMonitor.Max ? "预警" : "正常")}");

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
