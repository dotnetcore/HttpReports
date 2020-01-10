using HttpReports.Dashboard.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class JobService
    { 
        private DataService _dataService;
        private ScheduleService _scheduleService;

        public JobService(DataService dataService, ScheduleService scheduleService)
        {
            _dataService = dataService;
            _scheduleService = scheduleService;  
        }

        public void Start()
        {
            _scheduleService.Excute<CurrentJob>();   

            _scheduleService.Start(); 

        }    
    }
}
