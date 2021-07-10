using HttpReports.Core; 
using HttpReports.Core.Models; 
using HttpReports.Core.StorageFilters;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;  
using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

namespace HttpReports.Dashboard.Services
{
    public class MonitorBackendJob : IJob
    {
        private IHttpReportsStorage _storage; 

        private IAlarmService _alarmService;

        private ILogger<MonitorBackendJob> _logger; 
        
        private Localize _lang;

        private JsonSerializerOptions _jsonSetting; 

        public MonitorBackendJob()
        {

        }


        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _alarmService = _alarmService ?? ServiceContainer.provider.GetService(typeof(IAlarmService)) as IAlarmService; 
            _logger = _logger ?? ServiceContainer.provider.GetService(typeof(ILogger<MonitorBackendJob>)) as ILogger<MonitorBackendJob>;
            _lang = _lang ?? (ServiceContainer.provider.GetService(typeof(ILocalizeService)) as ILocalizeService).Current;
            _jsonSetting = _jsonSetting ?? (ServiceContainer.provider.GetService(typeof(JsonSerializerOptions)) as JsonSerializerOptions);

            MonitorJob job = context.JobDetail.JobDataMap.Get("job") as MonitorJob;

            MonitorJobPayload payload = System.Text.Json.JsonSerializer.Deserialize<MonitorJobPayload>(job.Payload,_jsonSetting);  
            
            var response = GetCheckResponse(new List<Func<MonitorJob, MonitorJobPayload, Task<AlarmOption>>> {

                ResponseTimeTask,ResponseErrorTask,RequestCountTask

            },job, payload); 
           
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

        private async Task<AlarmOption> ResponseTimeTask(MonitorJob job, MonitorJobPayload payload)
        {
            if (payload.ResponseTimeMonitor == null) return null; 

            var (now, start, end) = GetNowTimes(ParseJobCron(job.CronLike));  

            if (payload.ResponseTimeMonitor.Timeout == 0) return null;

            if (!job.StartTime.IsEmpty() && !job.EndTime.IsEmpty())
            {
                var startTime = new DateTime(now.Year, now.Month, now.Day, job.StartTime.Split(':')[0].ToInt(), job.StartTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);
                var endTime = new DateTime(now.Year, now.Month, now.Day, job.EndTime.Split(':')[0].ToInt(), job.EndTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);

                if (now < startTime || now > endTime)
                {
                    return null;
                }  
            }

            var (timeout, total) = await _storage.GetTimeoutResponeCountAsync(new ResponseTimeTaskFilter {  

                Service = job.Service,
                Instance = job.Instance,
                StartTime = start,
                EndTime = end ,
                TimeoutMS = payload.ResponseTimeMonitor.Timeout 

            });


            if ( total == 0) return null;

            var percent = timeout * 1.00 / total * 1.00; 

            if (percent > payload.ResponseTimeMonitor.Percentage * 0.01)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Alarm = new MonitorAlarm { 

                        JobId = job.Id,
                        Body =  $@"【{job.Title}】 {_lang.Monitor_Type_Timeout} --- {_lang.Warning_Threshold}:{payload.ResponseTimeMonitor.Timeout}ms, {(payload.ResponseTimeMonitor.Percentage).ToString("F2")}%    {_lang.Warning_Current}：{ (percent * 100.00) .ToString("F2")}% ",
                        CreateTime = DateTime.Now
                    
                    }, 
                    Content = $@"
                          <br>
                          <b>{_lang.Monitor_Type_Timeout} </b>
                          <p>{_lang.Monitor_Title}：{job.Title} </p>
                          <p>{_lang.Warning_Threshold}： {payload.ResponseTimeMonitor.Timeout}ms {(payload.ResponseTimeMonitor.Percentage).ToString("F2")}%   {_lang.Warning_Current}：{(percent * 100.00) .ToString("F2")}% </p>
                          <p>{_lang.Warning_Title}：{job.Title}</p>
                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>
                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? "ALL" : job.Instance)} </p>
                          <p>{_lang.Monitor_Frequency}：{ParseJobCronString(job.CronLike)} </p>
                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()} {_lang.To} {end.ToStandardTime()} </p>" 

                };
            }
            return null;
        }


        public async Task<AlarmOption> ResponseErrorTask(MonitorJob job, MonitorJobPayload payload)
        {
            if (payload.ErrorMonitor == null) return null;

            var (now, start, end) = GetNowTimes(ParseJobCron(job.CronLike));

            if (payload.ErrorMonitor.Percentage == 0) return null;

            if (!job.StartTime.IsEmpty() && !job.EndTime.IsEmpty())
            {
                var startTime = new DateTime(now.Year, now.Month, now.Day, job.StartTime.Split(':')[0].ToInt(), job.StartTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);
                var endTime = new DateTime(now.Year, now.Month, now.Day, job.EndTime.Split(':')[0].ToInt(), job.EndTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);

                if (now < startTime || now > endTime)
                {
                    return null;
                }
            }

            var (error, total) = await _storage.GetErrorResponeCountAsync(new ResponseErrorTaskFilter
            { 
                Service = job.Service,
                Instance = job.Instance,
                StartTime = start,
                EndTime = end, 
                Percentage = payload.ErrorMonitor.Percentage 

            });


            if (total == 0) return null;

            var percent = error * 1.00 / total * 1.00;

            if (percent > payload.ErrorMonitor.Percentage * 0.01)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Alarm = new MonitorAlarm
                    {

                        JobId = job.Id,
                        Body = $"【{job.Title}】 {_lang.Monitor_Type_RequestError} --- {_lang.Warning_Threshold}：{(payload.ErrorMonitor.Percentage).ToString("F2")}%  {_lang.Warning_Current}：{(percent * 100.00) .ToString("F2")}% ",
                        CreateTime = DateTime.Now

                    },
                    Content = $@"
                          <br>
                          <b>{_lang.Monitor_Type_RequestError} </b>
                          <p>{_lang.Monitor_Title}：{job.Title} </p>
                          <p>{_lang.Warning_Threshold}：{(payload.ErrorMonitor.Percentage).ToString("F2")}%  {_lang.Warning_Current}：{(percent * 100.00) .ToString("F2")}% </p>
                          <p>{_lang.Warning_Title}：{job.Title}</p>
                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>
                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? "ALL" : job.Instance)} </p>
                          <p>{_lang.Monitor_Frequency}：{ParseJobCronString(job.CronLike)} </p>
                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()} {_lang.To} {end.ToStandardTime()} </p>"


                };
            }
            return null;
        }

        public async Task<AlarmOption> RequestCountTask(MonitorJob job, MonitorJobPayload payload)
        {
            if (payload.CallMonitor == null) return null;

            var (now, start, end) = GetNowTimes(ParseJobCron(job.CronLike));

            if (payload.CallMonitor.Min == 0 && payload.CallMonitor.Max == 0) return null;

            if (!job.StartTime.IsEmpty() && !job.EndTime.IsEmpty())
            {
                var startTime = new DateTime(now.Year, now.Month, now.Day, job.StartTime.Split(':')[0].ToInt(), job.StartTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);
                var endTime = new DateTime(now.Year, now.Month, now.Day, job.EndTime.Split(':')[0].ToInt(), job.EndTime.Split(':')[1].ToInt(), 0, DateTimeKind.Local);

                if (now < startTime || now > endTime)
                {
                    return null;
                }
            }

            var total = await _storage.GetCallCountAsync(new CallCountTaskFilter
            {
                Service = job.Service,
                Instance = job.Instance,
                StartTime = start,
                EndTime = end

            });

            if (total < payload.CallMonitor.Min || total > payload.CallMonitor.Max)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Alarm = new MonitorAlarm
                    {

                        JobId = job.Id,
                        Body = $"【{job.Title}】 {_lang.Monitor_Type_RequestCount} --- {_lang.Warning_Threshold}：{_lang.Min} {payload.CallMonitor.Min} {_lang.Max} {payload.CallMonitor.Max}  {_lang.Warning_Current}：{total} ",
                        CreateTime = DateTime.Now

                    },
                    Content = $@"
                          <br>
                          <b>{_lang.Monitor_Type_RequestCount} </b>
                          <p>{_lang.Monitor_Title}：{job.Title} </p>
                          <p>{_lang.Warning_Threshold}：{_lang.Min}：{payload.CallMonitor.Min} {_lang.Max}：{payload.CallMonitor.Max}  {_lang.Warning_Current}：{total} </p>
                          <p>{_lang.Warning_Title}：{job.Title}</p>
                          <p>{_lang.Monitor_ServiceNode}：{job.Service}</p>
                          <p>{_lang.Monitor_InstanceName}：{(job.Instance.IsEmpty() ? "ALL" : job.Instance)} </p>
                          <p>{_lang.Monitor_Frequency}：{ParseJobCronString(job.CronLike)} </p>
                          <p>{_lang.Warning_TimeRange}：{start.ToStandardTime()} {_lang.To} {end.ToStandardTime()} </p>" 

                };
            }
            return null;
        }



        protected int ParseJobCron(string cron)
        {
            if (cron == "0 0/1 * * * ?") return 1;
            if (cron == "0 0/3 * * * ?") return 3;
            if (cron == "0 0/5 * * * ?") return 5;
            if (cron == "0 0/10 * * * ?") return 10;
            if (cron == "0 0/30 * * * ?") return 30;
            if (cron == "0 0 0/1 * * ?") return 60;
            if (cron == "0 0 0/2 * * ?") return 120;
            if (cron == "0 0 0/4 * * ?") return 240;
            if (cron == "0 0 0/6 * * ?") return 360;
            if (cron == "0 0 0/8 * * ?") return 480;
            if (cron == "0 0 0/12 * * ?") return 720;
            if (cron == "0 0 0 1/1 * ?") return 1440;

            return 1;
        }

        protected string ParseJobCronString(string cron)
        {
            if (cron == "0 0/1 * * * ?") return _lang.Monitor_Time1Min;
            if (cron == "0 0/3 * * * ?") return _lang.Monitor_Time3Min;
            if (cron == "0 0/5 * * * ?") return _lang.Monitor_Time5Min;
            if (cron == "0 0/10 * * * ?") return _lang.Monitor_Time10Min;
            if (cron == "0 0/30 * * * ?") return _lang.Monitor_Time30Min;
            if (cron == "0 0 0/1 * * ?") return _lang.Monitor_Time1Hour;
            if (cron == "0 0 0/2 * * ?") return _lang.Monitor_Time2Hour;
            if (cron == "0 0 0/4 * * ?") return _lang.Monitor_Time4Hour;
            if (cron == "0 0 0/6 * * ?") return _lang.Monitor_Time6Hour;
            if (cron == "0 0 0/8 * * ?") return _lang.Monitor_Time8Hour;
            if (cron == "0 0 0/12 * * ?") return _lang.Monitor_Time12Hour;
            if (cron == "0 0 0 1/1 * ?") return _lang.Monitor_Time1Day;

            return _lang.Monitor_Time1Min;
        }


        protected (DateTime now, DateTime start, DateTime end) GetNowTimes(int minute)
        {
            var now = DateTime.Now.AddMinutes(-BasicConfig.DeferTaskMinutes);
            return (now, now.AddMinutes(-minute), now);
        } 

    }
}
