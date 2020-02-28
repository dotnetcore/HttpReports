using System;

using Quartz;
using Quartz.Spi;

namespace HttpReports.Dashboard.Services.Quartz
{
    /// <summary>
    /// 支持依赖注入的QuartzJobFactory
    /// </summary>
    internal class AutoInjectionJobFactory : IJobFactory
    {
        public IServiceProvider ServiceProvider { get; }

        public AutoInjectionJobFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) => ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;

        public void ReturnJob(IJob job) => (job as IDisposable)?.Dispose();
    }
}