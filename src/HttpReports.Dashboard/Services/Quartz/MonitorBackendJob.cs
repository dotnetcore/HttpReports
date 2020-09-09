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

                //CheckResponseTimeOutMonitor,
                //CheckErrorResponseMonitor,
                //CheckIPMonitor,
                //CheckRequestCountMonitor

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
 
    }
}
