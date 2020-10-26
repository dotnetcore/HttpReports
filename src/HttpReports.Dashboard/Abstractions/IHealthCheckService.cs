using HttpReports.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Abstractions
{
    public interface IHealthCheckService
    { 
        Task<List<ServiceInstanceHealthInfo>> GetServiceInstance();

        Task<bool> SetServiceInstance(List<ServiceInstanceHealthInfo> list);

    }
}
