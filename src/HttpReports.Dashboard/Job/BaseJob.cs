using System.Threading.Tasks;

using Quartz;

namespace HttpReports.Dashboard.Job
{
    //并发执行
    [DisallowConcurrentExecution]
    public abstract class BaseJob : IJob
    {
        /// <summary>
        /// cronLike 表达式
        /// </summary>
        public abstract string cron { get; }

        public abstract Task Execute(IJobExecutionContext context);
    }
}