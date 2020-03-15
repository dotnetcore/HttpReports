using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class DashboardOptions : IOptions<DashboardOptions>
    {
        public bool UseHome { get; set; } = true;

        public MailOptions Mail { get; set; } = new MailOptions();

        public DashboardOptions Value => this; 
    }  
}
