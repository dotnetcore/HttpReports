using HttpReports.Core.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class DashboardOptions : IOptions<DashboardOptions>
    {
        public bool UseHome { get; set; } = true;
        public bool AllowAnonymous { get; set; } = false;

        public MailOptions Mail { get; set; } = new MailOptions();

        public int ExpireDay { get; set; } = BasicConfig.ExpireDay;

        public DashboardOptions Value => this; 
    }  
}
