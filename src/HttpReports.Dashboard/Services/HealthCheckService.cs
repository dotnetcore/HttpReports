using HttpReports.Core.ViewModels;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Storage.Abstractions;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class HealthCheckService:IHealthCheckService
    {
        private readonly IHttpReportsStorage _storage;
        private DateTime ExpireDateTime;
        public List<ServiceInstanceHealthInfo> HealthInfos;

        public object locker = new object();

        public HealthCheckService(IHttpReportsStorage storage)
        {
            _storage = storage;
            ExpireDateTime = DateTime.Now.AddSeconds(-3);
            HealthInfos = new List<ServiceInstanceHealthInfo>();
        }

        public async Task<bool> SetServiceInstance(List<ServiceInstanceHealthInfo> list)
        {
            lock (locker)
            {
                this.HealthInfos = list;  
            }

            return await Task.FromResult(true);
        }

        public async Task<List<ServiceInstanceHealthInfo>> GetServiceInstance()
        {
            if (ExpireDateTime > DateTime.Now)
            {
                lock (locker)
                {
                    return HealthInfos;
                } 
            }
            else
            {
                var list = await _storage.GetServiceInstance(DateTime.Now.AddDays(-1));

                List<ServiceInstanceHealthInfo> healthInfos = new List<ServiceInstanceHealthInfo>();

                foreach (var service in list.Select(x => x.Service).Distinct())
                {
                    healthInfos.Add(new ServiceInstanceHealthInfo
                    { 
                        ServiceInfo = new ServiceHealthInfo { Service = service },
                        Instances = list.Where(x => x.Service == service).Select(x => x.Instance).Distinct().Select(x => new InstanceHealthInfo { Instance = x }).ToList()

                    });
                }

                ExpireDateTime = DateTime.Now.AddMinutes(5);

                return healthInfos;
            }

        } 

    }
}
