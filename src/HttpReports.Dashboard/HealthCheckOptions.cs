using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class HealthCheckOptions : IOptions<HealthCheckOptions>
    {
        public string Path { get; set; } = "/Health";

        public int Ok{ get; set; } = 500;

        public int Slow { get; set; } = 2000; 

        public HealthCheckOptions Value => this;
    }
}
