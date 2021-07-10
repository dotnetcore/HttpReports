using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Dashboard.Abstractions; 
using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Options; 
using Quartz;
using Quartz.Impl; 
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class ScheduleService: IScheduleService
    { 
        private const string SchedulerGroup = "HttpReports_Scheduler";

        private const string SchedulerTag = "Monitor_";

        public IScheduler scheduler;

        private IHttpReportsStorage _storage;

        private DashboardOptions _options;

        private JsonSerializerOptions _jsonSetting;

        public ScheduleService(IHttpReportsStorage storage, JsonSerializerOptions jsonSetting, IOptions<DashboardOptions> options)
        {
            _storage = storage;

            _options = options.Value;

            _jsonSetting = jsonSetting;

            scheduler = scheduler ?? new StdSchedulerFactory().GetScheduler().Result; 
        }

        public async Task InitAsync()
        {
            await AutoClearDataJobAsync();

            await HealthCheckAsync();

            await InitMonitorJobAsync();

            await scheduler.Start();
        }  

        public async Task InitMonitorJobAsync()
        { 
            List<MonitorJob> list = await _storage.GetMonitorJobs();  

            if (list == null || !list.Any())
            {
                return;
            }

            list = list.Where(x => x.Status == 1).ToList();

            foreach (var item in list)
            {
                await ScheduleJobAsync(item);
            }  
        }

        public async Task ScheduleJobAsync(MonitorJob model)
        { 
            var job = JobBuilder.Create<MonitorBackendJob>().
                   WithIdentity(SchedulerTag + model.Id, SchedulerGroup)
                   .SetJobData(new JobDataMap { { "job", model } }).Build();

            var trigger = TriggerBuilder.Create().WithCronSchedule(model.CronLike).Build();

            await scheduler.ScheduleJob(job, trigger);    

        }

        public async Task DeleteJobAsync(IJobDetail job)
        {
            if (scheduler.CheckExists(job.Key).Result)
            {
               await scheduler.PauseJob(job.Key);

               await scheduler.DeleteJob(job.Key);
            }
        }

        public async Task UpdateMonitorJobAsync(MonitorJob deleteJob = null)
        {
            if (deleteJob != null)
            {
                var delete = await scheduler.GetJobDetail(new JobKey(SchedulerTag + deleteJob.Id, SchedulerGroup));

                if (delete != null)
                {
                    await DeleteJobAsync(delete);
                } 
            } 


            List<MonitorJob> list = await _storage.GetMonitorJobs();

            if (list == null || list.Count == 0)
            {
                return;
            }

            foreach (var k in list)
            {
                var job = await scheduler.GetJobDetail(new JobKey(SchedulerTag + k.Id, SchedulerGroup));

                if (job == null)
                { 
                    if (k.Status == 1)  await ScheduleJobAsync(k); 
                }
                else
                {
                    if (k.Status == 0)
                    {
                       await DeleteJobAsync(job);
                    }
                    else
                    {
                        MonitorJob monitorJob = job.JobDataMap.Get("job") as MonitorJob; 

                        // 判断是否有修改，如果修改后，重置Job
                        if (System.Text.Json.JsonSerializer.Serialize(k,_jsonSetting) != System.Text.Json.JsonSerializer.Serialize(monitorJob,_jsonSetting))
                        {
                            await DeleteJobAsync(job);
                            await ScheduleJobAsync(k); 
                        }  
                    }  
                }  
            }  
        }

        public async Task AutoClearDataJobAsync()
        { 
            if (_options.ExpireDay > 0)
            {
                var job = JobBuilder.Create<ClearReportsDataJob>().Build();

                var trigger = TriggerBuilder.Create().WithCronSchedule(BasicConfig.ClearDataCornLike).Build();

                await scheduler.ScheduleJob(job, trigger);
            } 
          
        }

        public async Task HealthCheckAsync()
        {
            if (_options.Check.Switch)
            {
                var job = JobBuilder.Create<HealthCheckJob>().Build();

                var trigger = TriggerBuilder.Create().WithCronSchedule(BasicConfig.HealthCheckCornLike).Build();

                await scheduler.ScheduleJob(job, trigger);
            }

        }


    }
}
