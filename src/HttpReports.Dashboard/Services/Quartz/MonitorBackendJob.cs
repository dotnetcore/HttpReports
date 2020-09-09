using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
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
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class MonitorBackendJob : IJob
    {
        private IHttpReportsStorage _storage;

        private MonitorService _monitorService;

        private IAlarmService _alarmService;

        private ILogger<MonitorBackendJob> _logger; 
        
        private Localize _lang;


        public MonitorBackendJob()
        {

        }


        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _alarmService = _alarmService ?? ServiceContainer.provider.GetService(typeof(IAlarmService)) as IAlarmService;
            _monitorService = _monitorService ?? ServiceContainer.provider.GetService(typeof(MonitorService)) as MonitorService;
            _logger = _logger ?? ServiceContainer.provider.GetService(typeof(ILogger<MonitorBackendJob>)) as ILogger<MonitorBackendJob>;
            _lang = _lang ?? (ServiceContainer.provider.GetService(typeof(LocalizeService)) as LocalizeService).Current;


            MonitorJob job = context.JobDetail.JobDataMap.Get("job") as MonitorJob;

            MonitorJobPayload payload = JsonConvert.DeserializeObject<MonitorJobPayload>(job.Payload);


            //开始调用任务 
            var response = GetCheckResponse(new List<Func<MonitorJob, MonitorJobPayload, Task<AlarmOption>>> {

                CheckResponseTimeOutMonitor,
                CheckErrorResponseMonitor,
                CheckIPMonitor,
                CheckRequestCountMonitor

           }, job, payload); 
           
            await AlarmAsync(response.Select(x => x.Result).ToList(), job);

        }

        private IEnumerable<Task<AlarmOption>> GetCheckResponse(List<Func<MonitorJob, MonitorJobPayload, Task<AlarmOption>>> funcs, MonitorJob job, MonitorJobPayload payload)
        {
            List<Task<AlarmOption>> alarmOptions = new List<Task<AlarmOption>>();

            foreach (var func in funcs)
            {
                alarmOptions.Add(Task.Run(() => func.Invoke(job, payload)));
            }

            return alarmOptions;
        }


        private async Task AlarmAsync(IList<AlarmOption> alarmOption, MonitorJob job)
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
        private async Task<AlarmOption> CheckResponseTimeOutMonitor(MonitorJob job, MonitorJobPayload payload)
        {  
            if (payload.ResponseTimeOutMonitor == null)
            {
                return null;
            }

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            _logger.LogInformation("CheckResponseTimeOutMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


            #region BuildService 

            string LocalIP = "";
            int LocalPort = 0;

            if (job.Instance.IsEmpty() || job.Instance == "ALL")
            {
                LocalIP = "";
                LocalPort = 0;
            }
            else
            {
                LocalIP = job.Instance.Substring(0, job.Instance.LastIndexOf(':'));
                LocalPort = job.Instance.Substring(job.Instance.LastIndexOf(':') + 1).ToInt();
            }

            #endregion 


            var timeoutCount = await _storage.GetTimeoutResponeCountAsync(new RequestCountFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
                StartTime = start,
                EndTime = end,
            }, payload.ResponseTimeOutMonitor.TimeOutMs);

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
                StartTime = start,
                EndTime = end,
            });

            if (count == 0)
            {
                return null;
            }

            var percent = timeoutCount * 100.0 / count; 

            _logger.LogInformation("CheckResponseTimeOutMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"   {(percent > payload.ResponseTimeOutMonitor.Percentage ? "Alert notification trigger" : "Pass")}");

            if (percent > payload.ResponseTimeOutMonitor.Percentage)
            { 
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{_lang.Monitor_Type_Timeout}】 </b>

                          <p> {_lang.Warning_Threshold}：{payload.ResponseTimeOutMonitor.Percentage.ToString("F2")}%  {_lang.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{_lang.Monitor_Title}：{job.Title}</p>

                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>

                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? BasicConfig.ALLTag : job.Instance)}</p>

                          <p>{_lang.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
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
        private async Task<AlarmOption> CheckErrorResponseMonitor(MonitorJob job, MonitorJobPayload payload)
        {  
            if (payload.ErrorResponseMonitor == null)
            {
                return null;
            }

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            _logger.LogInformation("CheckErrorResponseMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            #region BuildService 

            string LocalIP = "";
            int LocalPort = 0;

            if (job.Instance.IsEmpty() || job.Instance == "ALL")
            {
                LocalIP = "";
                LocalPort = 0;
            }
            else
            {
                LocalIP = job.Instance.Substring(0, job.Instance.LastIndexOf(':'));
                LocalPort = job.Instance.Substring(job.Instance.LastIndexOf(':') + 1).ToInt();
            }

            #endregion 

            var errorCount = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
                StartTime = start,
                EndTime = end,
                StatusCodes = payload.ErrorResponseMonitor.HttpCodeStatus.Split(',').Select(x => x.ToInt()).ToArray()
            });

            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
                StartTime = start,
                EndTime = end,
            });

            if (count == 0)
            {
                return null;
            }
            var percent = errorCount * 100.0 / count;

            _logger.LogInformation("CheckErrorResponseMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  {(percent > payload.ErrorResponseMonitor.Percentage ? "Alert notification trigger" : "Pass")}");

            if (percent > payload.ErrorResponseMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{_lang.Monitor_Type_RequestError}】 </b>

                          <p>{_lang.Warning_Threshold}：{payload.ErrorResponseMonitor.Percentage.ToString("F2")}%  {_lang.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{_lang.Warning_Title}：{job.Title}</p>

                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>

                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? BasicConfig.ALLTag : job.Instance)} </p>

                          <p>{_lang.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{_lang.Monitor_HttpStatusCode}：{payload.ErrorResponseMonitor.HttpCodeStatus}</p>

                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"


                };
            }
            return null;
        }


       
        private async Task<AlarmOption> CheckIPMonitor(MonitorJob job, MonitorJobPayload payload)
        {
            if (payload.IPMonitor == null)
            {
                return null;
            }

            _logger.LogInformation("CheckIPMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            #region BuildService 

            string LocalIP = "";
            int LocalPort = 0;

            if (job.Instance.IsEmpty() || job.Instance == "ALL")
            {
                LocalIP = "";
                LocalPort = 0;
            }
            else
            {
                LocalIP = job.Instance.Substring(0, job.Instance.LastIndexOf(':'));
                LocalPort = job.Instance.Substring(job.Instance.LastIndexOf(':') + 1).ToInt();
            }

            #endregion 

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));

            var (max, count) = await _storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
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

            _logger.LogInformation("CheckIPMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"   {(percent > payload.IPMonitor.Percentage ? "Alert notification trigger" : "Pass")}");

            if (percent > payload.IPMonitor.Percentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{_lang.Monitor_Type_IP}】 </b>

                          <p>{_lang.Warning_Threshold}：{payload.IPMonitor.Percentage.ToString("F2")}% {_lang.Warning_Current}：{percent.ToString("F2")}% </p>

                          <p>{_lang.Warning_Title}：{job.Title}</p>

                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>

                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? BasicConfig.ALLTag : job.Instance)} </p> 

                          <p>{_lang.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{_lang.Monitor_IPWhiteList}：{payload.IPMonitor.WhiteList}</p>

                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;


        }

 
        private async Task<AlarmOption> CheckRequestCountMonitor(MonitorJob job, MonitorJobPayload payload)
        {
            if (payload.RequestCountMonitor == null)
            {
                return null;
            }

            _logger.LogInformation("CheckRequestCountMonitor Start " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            #region BuildService 

            string LocalIP = "";
            int LocalPort = 0;

            if (job.Instance.IsEmpty() || job.Instance == "ALL")
            {
                LocalIP = "";
                LocalPort = 0;
            }
            else
            {
                LocalIP = job.Instance.Substring(0, job.Instance.LastIndexOf(':'));
                LocalPort = job.Instance.Substring(job.Instance.LastIndexOf(':') + 1).ToInt();
            }

            #endregion 

            var (now, start, end) = GetNowTimes(_monitorService.ParseJobCron(job.CronLike));
            var count = await _storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Service = job.Service,
                LocalIP = LocalIP,
                LocalPort = LocalPort,
                StartTime = start,
                EndTime = end,
            });

            _logger.LogInformation("CheckRequestCountMonitor End  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"  {(count > payload.RequestCountMonitor.Max ? "Alert notification trigger" : "Pass")}");

            if (count > payload.RequestCountMonitor.Max)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【{_lang.Monitor_Type_RequestCount}】 </b>

                          <p>{_lang.Warning_Threshold}：{payload.RequestCountMonitor.Max}  {_lang.Warning_Current}：{count} </p>

                          <p>{_lang.Warning_Title}：{job.Title}</p>

                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>

                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? BasicConfig.ALLTag : job.Instance)} </p>

                          <p>{_lang.Monitor_Frequency}：{_monitorService.ParseJobCronString(job.CronLike)} </p>

                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
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
