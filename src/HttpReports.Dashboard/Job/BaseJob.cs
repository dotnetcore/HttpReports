using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
