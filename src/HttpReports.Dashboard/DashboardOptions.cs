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

        public HealthCheckOptions Check { get; set; } = new HealthCheckOptions();

        public DashboardOptions Value => this; 

    }  
}
