using HttpReports.Core.ViewModels;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Storage.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
                
                this.HealthInfos.Add(new ServiceInstanceHealthInfo
                {

                    ServiceInfo = new ServiceHealthInfo { Service = "AA" },
                    Instances = new List<InstanceHealthInfo> { new InstanceHealthInfo {

                    Instance = "ccc"

                }}

                });

                var list = await _storage.GetServiceInstance(DateTime.Now.AddDays(-1));

                list.Add(new ServiceInstanceInfo { Service = "BB", Instance = "bb" });


                List<ServiceInstanceInfo> Removes = new List<ServiceInstanceInfo>();

                // Remove 
                foreach (var ser in this.HealthInfos)
                {
                    foreach (var ins in ser.Instances)
                    {
                        var model = list.Where(x => x.Service == ser.ServiceInfo.Service && x.Instance == ins.Instance).FirstOrDefault();

                        if (model == null)
                        {
                            Removes.Add(new ServiceInstanceInfo { Service = ser.ServiceInfo.Service, Instance = ins.Instance });
                        }
                    }
                }

                if (Removes.Any())
                {
                    var newHealthInfos = this.HealthInfos.ToList();

                    foreach (var item in Removes)
                    {
                        var model = newHealthInfos.Where(x => x.ServiceInfo.Service == item.Service).FirstOrDefault();

                        if (model != null)
                        {
                            var ins = Removes.Where(x => x.Service == item.Service).Select(x => x.Instance);

                            var remove = model.Instances.Where(x => ins.Contains(x.Instance));

                            model.Instances.RemoveAll(x => ins.Contains(x.Instance));
                        }
                    }


                    newHealthInfos.RemoveAll(x => !x.Instances.Any());

                    this.HealthInfos = new ConcurrentBag<ServiceInstanceHealthInfo>(newHealthInfos);

                }

                // Add
                foreach (var item in list)
                {
                    var model = this.HealthInfos.Where(x => x.ServiceInfo.Service == item.Service).FirstOrDefault();

                    if (model == null)
                    {
                        this.HealthInfos.Add(new ServiceInstanceHealthInfo { 
                            
                            ServiceInfo = new ServiceHealthInfo { Service = item.Service },
                            Instances = new List<InstanceHealthInfo> { }
                            
                        });;

                    }

                    model = this.HealthInfos.Where(x => x.ServiceInfo.Service == item.Service).FirstOrDefault();

                    var instance = list.Where(x => x.Service == item.Service).Select(x => x.Instance);

                    foreach (var ins in instance)
                    {
                        if (!model.Instances.Select(x => x.Instance).Contains(ins))
                        {
                            model.Instances.Add(new InstanceHealthInfo { Instance = ins });
                        } 
                    }   
                }

                ExpireDateTime = DateTime.Now.AddMinutes(5);

                return this.HealthInfos; 

            }

        } 

    }
}
