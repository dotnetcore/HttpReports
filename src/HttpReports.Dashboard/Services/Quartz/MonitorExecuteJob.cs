using System;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

using Microsoft.Extensions.Logging;

using Quartz;

namespace HttpReports.Dashboard.Services.Quartz
{
    /// <summary>
    /// 执行监控Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class MonitorExecuteJob : IJob
    {
        /// <summary>
        /// Monitor存放在JobDataMap中的key
        /// </summary>
        public const string MonitorDataKey = "Monitor";

        /// <summary>
        /// Node存放在JobDataMap中的key
        /// </summary>
        public const string NodeKey = "Node";

        /// <summary>
        /// MonitorRule存放在JobDataMap中的key
        /// </summary>
        public const string MonitorRuleDataKey = "MonitorRule";

        public IHttpReportsStorage Storage { get; }

        public IAlarmService AlarmService { get; }

        public ILogger<MonitorExecuteJob> Logger { get; }

        private TimeSpan _timeSpan;

        private string[] _nodes = null;

        public MonitorExecuteJob(IHttpReportsStorage storage, IAlarmService alarmService, ILogger<MonitorExecuteJob> logger)
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            AlarmService = alarmService ?? throw new ArgumentNullException(nameof(alarmService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var (rule, monitor) = GetMonitorAndRule(context);

            _nodes = new string[] { context.JobDetail.JobDataMap.Get(NodeKey) as string };

            var cron = new CronExpression(monitor.CronExpression);

            _timeSpan = (cron.GetNextValidTimeAfter(DateTimeOffset.UnixEpoch).Value - DateTimeOffset.UnixEpoch);

            AlarmOption alarmOption = null;
            switch (monitor.Type)
            {
                case MonitorType.ResponseTimeOut:
                    alarmOption = await ExecuteMonitor(rule, monitor as IResponseTimeOutMonitor);
                    break;

                case MonitorType.ErrorResponse:
                    alarmOption = await ExecuteMonitor(rule, monitor as IErrorResponseMonitor);
                    break;

                case MonitorType.ToManyRequestWithAddress:
                    alarmOption = await ExecuteMonitor(rule, monitor as IRequestTimesMonitor);
                    break;

                case MonitorType.ToManyRequestBySingleRemoteAddress:
                    alarmOption = await ExecuteMonitor(rule, monitor as IRemoteAddressRequestTimesMonitor);
                    break;

                case MonitorType.UnDefine:
                default:
                    Logger.LogError($"不支持的监控 {context.JobDetail.Key} 类型: {monitor.Type}");
                    break;
            }

            if (alarmOption != null)
            {
                alarmOption.Emails = rule.NotificationEmails;
                alarmOption.Phones = rule.NotificationPhoneNumbers;
                await AlarmService.AlarmAsync(alarmOption);
            }
        }

        protected async Task<AlarmOption> ExecuteMonitor(IMonitorRule rule, IResponseTimeOutMonitor monitor)
        {
            var (now, start, end) = GetNowTimes();

            var timeoutCount = await Storage.GetTimeoutResponeCountAsync(new RequestCountFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
            }, monitor.TimeoutThreshold).ConfigureAwait(false);

            var count = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }

            var percent = timeoutCount * 100.0 / count;

            if (percent > monitor.WarningPercentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【响应超时】触发预警 </b>

                          <p>超时率预警值：{monitor.WarningPercentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{rule.Title}</p>

                          <p>监控节点：{string.Join(", ", rule.Nodes)}</p>

                          <p>监控频率：{_timeSpan.TotalMinutes.ToString("F2")} 分钟</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        protected async Task<AlarmOption> ExecuteMonitor(IMonitorRule rule, IRequestTimesMonitor monitor)
        {
            var (now, start, end) = GetNowTimes();
            var count = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count > monitor.WarningThreshold)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【请求量监控】触发预警 </b>

                          <p>请求量最大预警值：{monitor.WarningThreshold}  当前值：{count} </p>

                          <p>任务标题：{rule.Title}</p>

                          <p>监控节点：{string.Join(", ", rule.Nodes)}</p>

                          <p>监控频率：{_timeSpan.TotalMinutes.ToString("F2")} 分钟</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        protected async Task<AlarmOption> ExecuteMonitor(IMonitorRule rule, IRemoteAddressRequestTimesMonitor monitor)
        {
            var (now, start, end) = GetNowTimes();

            var (max, count) = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
                InList = false,
                List = monitor.WhileList.ToArray(),
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }
            var percent = max * 100.0 / count;

            if (percent > monitor.WarningPercentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【IP异常】触发预警 </b>

                          <p>IP重复率预警值：{monitor.WarningPercentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{rule.Title}</p>

                          <p>监控节点：{string.Join(", ", rule.Nodes)}</p>

                          <p>监控频率：{_timeSpan.TotalMinutes.ToString("F2")} 分钟</p>

                          <p>IP白名单：{string.Join(", ", monitor.WhileList)}</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        protected async Task<AlarmOption> ExecuteMonitor(IMonitorRule rule, IErrorResponseMonitor monitor)
        {
            var (now, start, end) = GetNowTimes();

            var errorCount = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
                StatusCodes = monitor.StatusCodes.ToArray(),
            }).ConfigureAwait(false);

            var count = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                Nodes = _nodes,
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }
            var percent = errorCount * 100.0 / count;

            if (percent > monitor.WarningPercentage)
            {
                return new AlarmOption()
                {
                    IsHtml = true,
                    Content = $@"

                          <br>
                          <b>【请求错误】触发预警 </b>

                          <p>命中率预警值：{monitor.WarningPercentage.ToString("F2")}%  当前值：{percent.ToString("F2")}% </p>

                          <p>任务标题：{rule.Title}</p>

                          <p>监控节点：{string.Join(", ", rule.Nodes)}</p>

                          <p>监控频率：{_timeSpan.TotalMinutes.ToString("F2")} 分钟</p>

                          <p>设定Http状态码：{string.Join(", ", monitor.StatusCodes)}</p>

                          <p>时间段：{start.ToStandardTime()}-{end.ToStandardTime()} </p>"
                };
            }
            return null;
        }

        /// <summary>
        /// 获取基于监控间隔的时间
        /// </summary>
        /// <returns></returns>
        protected (DateTime now, DateTime start, DateTime end) GetNowTimes()
        {
            var now = DateTime.Now;
            return (now, now.Add(-_timeSpan), now);
        }

        /// <summary>
        /// 创建job名称
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="node"></param>
        /// <param name="monitorId"></param>
        /// <returns></returns>
        public static string CreateJobName(int ruleId, string node, int monitorId) => $"job-{ruleId}-{node}-{monitorId}";

        /// <summary>
        /// 创建job名称
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string CreateJobNamePrefix(int ruleId, string node) => $"job-{ruleId}-{node}";

        /// <summary>
        /// 创建job名称
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public static string CreateJobNamePrefix(int ruleId) => $"job-{ruleId}";

        /// <summary>
        /// 创建trigger名称
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="node"></param>
        /// <param name="monitorId"></param>
        /// <returns></returns>
        public static string CreateTriggerName(int ruleId, string node, int monitorId) => $"trigger-{ruleId}-{node}-{monitorId}";

        protected static (IMonitorRule Rule, IMonitor Monitor) GetMonitorAndRule(IJobExecutionContext context) => (context.JobDetail.JobDataMap.Get(MonitorRuleDataKey) as IMonitorRule, context.JobDetail.JobDataMap.Get(MonitorDataKey) as IMonitor);
    }
}