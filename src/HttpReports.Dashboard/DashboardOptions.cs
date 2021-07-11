using HttpReports.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class DashboardOptions : IOptions<DashboardOptions>
    {  
        public MailOptions Mail { get; set; } = new MailOptions();

        public int ExpireDay { get; set; } = BasicConfig.ExpireDay; 

        public int QueryCount { get; set; } = 6;

        public bool Migration { get; set; } = true;

        public HealthCheckOptions Check { get; set; } = new HealthCheckOptions();

        public bool EnableCors { get; set; } = true;

        public DashboardOptions Value => this; 

    }  
}
