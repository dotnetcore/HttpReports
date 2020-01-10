using System;
using System.Collections.Generic; 
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services;
using Quartz;

namespace HttpReports.Dashboard.Job
{
    public class BackendJob : IJob
    {
        public DataService dataService;

        public Task Execute(IJobExecutionContext context)
        {  
            dataService = ServiceContainer.provider.GetService(typeof(DataService)) as DataService;   

            var job = context.JobDetail.JobDataMap.Get("job") as Models.Job;

            Console.WriteLine($"--- {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------------- {job.Title} -------------");
            Console.WriteLine(); 
            Console.WriteLine(); 

            dataService.CheckJob(job);   

            return Task.CompletedTask;    
        }   

    }
}
