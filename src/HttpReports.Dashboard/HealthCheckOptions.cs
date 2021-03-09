using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class HealthCheckOptions : IOptions<HealthCheckOptions>
    {
        public bool Switch { get; set; }

        public string Mode { get; set; }

        public string Endpoint { get; set; } 

        public string Range { get; set; } 

        public HealthCheckOptions Value => this;
    }
}
