using HttpReports.Core.ViewModels;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Storage.Abstractions;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class HealthCheckService:IHealthCheckService
    {
        private readonly IHttpReportsStorage _storage;
        private DateTime ExpireDateTime; 

        public ConcurrentBag<ServiceInstanceHealthInfo> HealthInfos = new ConcurrentBag<ServiceInstanceHealthInfo>();
         

        public HealthCheckService(IHttpReportsStorage storage)
        {
            _storage = storage;
            ExpireDateTime = DateTime.Now.AddSeconds(-3); 
        }

        public async Task<bool> SetServiceInstance(ConcurrentBag<ServiceInstanceHealthInfo> list)
        { 
            this.HealthInfos = new ConcurrentBag<ServiceInstanceHealthInfo>();

            foreach (var item in list)
            { 
                this.HealthInfos.Add(new ServiceInstanceHealthInfo { 
                    
                    Instances = item.Instances,
                    ServiceInfo = item.ServiceInfo
                    
                });
            }  

            return await Task.FromResult(true);
        }

        public async Task<ConcurrentBag<ServiceInstanceHealthInfo>> GetServiceInstance()
        {
            if (ExpireDateTime > DateTime.Now)
            {  
                 return HealthInfos; 
            }
            else
            {
                var list = await _storage.GetServiceInstance(DateTime.Now.AddDays(-1));

                this.HealthInfos = new ConcurrentBag<ServiceInstanceHealthInfo>();  

                foreach (var service in list.Select(x => x.Service).Distinct())
                {
                    this.HealthInfos.Add(new ServiceInstanceHealthInfo
                    { 
                        ServiceInfo = new ServiceHealthInfo { Service = service },
                        Instances = list.Where(x => x.Service == service).Select(x => x.Instance).Distinct().Select(x => new InstanceHealthInfo { Instance = x }).ToList()

                    });
                }

                ExpireDateTime = DateTime.Now.AddMinutes(5);

                return this.HealthInfos;
            }

        } 

    }
}
