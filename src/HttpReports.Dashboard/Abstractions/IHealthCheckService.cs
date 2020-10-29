using HttpReports.Core.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Abstractions
{
    public interface IHealthCheckService
    { 
        Task<ConcurrentBag<ServiceInstanceHealthInfo>> GetServiceInstance();

        Task<bool> SetServiceInstance(ConcurrentBag<ServiceInstanceHealthInfo> list);

    }
}
