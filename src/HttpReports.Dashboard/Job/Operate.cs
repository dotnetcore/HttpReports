using HttpReports.Dashboard.Job.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Job
{
    public class Operate
    {  

        /// <summary>
        /// 添加
        /// </summary>
        public void Insert()
        {
            List<BaseMonitor> monitors = new List<BaseMonitor>() {   };

            monitors.Add(new BaseMonitor { 
             
                MonitorType = "IPMonitor",
                Max = "30" 

            });

            monitors.Add(new BaseMonitor
            {

                MonitorType = "ResponseTimeMonitor",
                Min = "30",
                IPWhiteList = "192.0.0.1"

            });


            // TODO 把monitors 序列化成json 存到 JobGroup 

        } 


        /// <summary>
        /// 调用
        /// </summary>
        public void Excute()
        {
            var json  = new JobGroup().MonitorJson;

            var monitors = JsonConvert.DeserializeObject<List<BaseMonitor>>(json);  

            foreach (var item in monitors)
            {  
                if (item.MonitorType == typeof(IPMonitor).Name)
                {
                    new IPMonitor().Excute(item);
                }

                if (item.MonitorType == typeof(ResponseTimeMonitor).Name)
                {
                    new ResponseTimeMonitor().Excute(item);
                } 
            }  

        } 

    }
}
