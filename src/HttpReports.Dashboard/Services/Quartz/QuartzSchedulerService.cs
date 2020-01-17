using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Monitor;

using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Spi;

using GroupMatcher = Quartz.Impl.Matchers.GroupMatcher<Quartz.JobKey>;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class QuartzSchedulerService : ISchedulerService
    {
        public const string SchedulerGroup = "HttpReports_Scheduler";

        private IScheduler _scheduler = null;

        public IJobFactory JobFactory { get; }

        protected QuartzLogProvider QuartzLogProvider { get; }
        protected ILogger<QuartzSchedulerService> Logger { get; }

        public QuartzSchedulerService(IJobFactory jobFactory, QuartzLogProvider quartzLogProvider, ILogger<QuartzSchedulerService> logger)
        {
            JobFactory = jobFactory;
            QuartzLogProvider = quartzLogProvider;
            Logger = logger;
        }

        public async Task InitAsync()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false) ?? throw new TypeInitializationException(nameof(QuartzSchedulerService), null);
            LogProvider.SetCurrentLogProvider(QuartzLogProvider);
            //_scheduler.JobFactory = new PropertySettingJobFactory();
            _scheduler.JobFactory = JobFactory;

            await _scheduler.Start().ConfigureAwait(false);

            Logger.LogInformation($"{nameof(QuartzSchedulerService)}已启动");
        }

        public async Task AddMonitorRuleAsync(IMonitorRule rule)
        {
            if (rule is null
                || rule.Nodes is null
                || rule.Nodes.Count == 0
                || rule.Monitors is null
                || rule.Monitors.Count == 0)
            {
                return;
            }

            foreach (var node in rule.Nodes)
            {
                foreach (var monitor in rule.Monitors)
                {
                    IJobDetail job = JobBuilder.Create<MonitorExecuteJob>()
                        .WithIdentity(MonitorExecuteJob.CreateJobName(rule.Id, node, monitor.Id), SchedulerGroup)
                        .SetJobData(new JobDataMap { { MonitorExecuteJob.MonitorRuleDataKey, rule }, { MonitorExecuteJob.MonitorDataKey, monitor }, { MonitorExecuteJob.NodeKey, node } })
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(MonitorExecuteJob.CreateTriggerName(rule.Id, node, monitor.Id), SchedulerGroup)
                        .WithCronSchedule(monitor.CronExpression)
                        .Build();
                    var nextTime = await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
                    Logger.LogInformation($"已计划执行监控任务: {job.Key} 下次执行时间: {nextTime.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                }
            }
        }

        public async Task UpdateMonitorRuleAsync(IMonitorRule rule)
        {
            if (rule is null)
            {
                return;
            }

            var allJobKeys = await _scheduler.GetJobKeys(GroupMatcher.GroupEquals(SchedulerGroup)).ConfigureAwait(false);

            var ruleJobPrefix = MonitorExecuteJob.CreateJobNamePrefix(rule.Id);
            var ruleJobKeys = allJobKeys.Where(m => m.Name.StartsWith(ruleJobPrefix)).ToList();

            if ((rule.Nodes is null
                || rule.Nodes.Count == 0
                || rule.Monitors is null
                || rule.Monitors.Count == 0) && ruleJobKeys.Count > 0)
            {
                await _scheduler.DeleteJobs(ruleJobKeys).ConfigureAwait(false);
            }

            var currentJobKeys = new List<JobKey>();
            foreach (var node in rule.Nodes)
            {
                foreach (var monitor in rule.Monitors)
                {
                    var jobKey = new JobKey(MonitorExecuteJob.CreateJobName(rule.Id, node, monitor.Id), SchedulerGroup);

                    currentJobKeys.Add(jobKey);

                    if (await _scheduler.CheckExists(jobKey).ConfigureAwait(false))
                    {
                        await _scheduler.DeleteJob(jobKey).ConfigureAwait(false);
                    }

                    IJobDetail job = JobBuilder.Create<MonitorExecuteJob>()
                        .WithIdentity(jobKey)
                        .SetJobData(new JobDataMap { { MonitorExecuteJob.MonitorRuleDataKey, rule }, { MonitorExecuteJob.MonitorDataKey, monitor }, { MonitorExecuteJob.NodeKey, node } })
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(MonitorExecuteJob.CreateTriggerName(rule.Id, node, monitor.Id), SchedulerGroup)
                        .WithCronSchedule(monitor.CronExpression)
                        .Build();

                    var nextTime = await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
                    Logger.LogInformation($"已计划执行监控任务: {job.Key} 下次执行时间: {nextTime.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                }
            }

            var invalidJobKeys = ruleJobKeys.Except(currentJobKeys).ToArray();

            if (invalidJobKeys.Length > 0)
            {
                await _scheduler.DeleteJobs(invalidJobKeys).ConfigureAwait(false);
            }
        }

        public async Task DeleteMonitorRuleAsync(int ruleId)
        {
            var allJobKeys = await _scheduler.GetJobKeys(GroupMatcher.GroupEquals(SchedulerGroup)).ConfigureAwait(false);

            var ruleJobPrefix = MonitorExecuteJob.CreateJobNamePrefix(ruleId);
            var ruleJobKeys = allJobKeys.Where(m => m.Name.StartsWith(ruleJobPrefix)).ToList();

            await _scheduler.DeleteJobs(ruleJobKeys).ConfigureAwait(false);
        }
    }
}