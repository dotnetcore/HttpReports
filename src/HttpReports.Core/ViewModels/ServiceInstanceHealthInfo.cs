using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.ViewModels
{
    public class ServiceInstanceHealthInfo
    {
        public ServiceHealthInfo ServiceInfo { get; set; }

        public List<InstanceHealthInfo> Instances { get; set; }

    }

    public class InstanceHealthInfo
    { 
        public string Instance { get; set; }

        public HealthStatusEnum Status { get; set; } 

    }

    public class ServiceHealthInfo
    {
        public string Service { get; set; }

        public int Passing { get; set; }

        public int Warning { get; set; }

        public int Critical { get; set; }

    }
}
