using HttpReports.Dashboard.DataAccessors;
using HttpReports.Dashboard.DataContext;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class DataService
    { 
        private IDataAccessor _accessor;

        public DataService(IDataAccessor accessor)
        {
            _accessor = accessor;
        }

        public List<EchartPineDataModel> GetStatusCode(GetIndexDataRequest request) => _accessor.GetStatusCode(request);

        public List<EchartPineDataModel> GetResponseTimePie(GetIndexDataRequest request) => _accessor.GetResponseTimePie(request);

        public List<EchartPineDataModel> GetDayRequestTimes(GetIndexDataRequest request) => _accessor.GetDayRequestTimes(request); 

        public List<EchartPineDataModel> GetDayResponseTime(GetIndexDataRequest request) => _accessor.GetDayResponseTime(request);

        public List<string> GetNodes() => _accessor.GetNodes(); 

        public GetIndexDataResponse GetIndexData(GetIndexDataRequest request) => _accessor.GetIndexData(request);


        public List<GetTopResponse> GetTopRequest(GetTopRequest request) => _accessor.GetTopRequest(request);

        public List<GetTopResponse> GetCode500Response(GetTopRequest request) => _accessor.GetCode500Response(request);

        public List<EchartPineDataModel> GetTOPART(GetTopRequest request) => _accessor.GetTOPART(request);

        public List<RequestInfo> GetRequestList(GetRequestListRequest request, out int totalCount) => _accessor.GetRequestList(request,out totalCount);

        public List<EchartPineDataModel> GetLatelyDayData(GetIndexDataRequest request) => _accessor.GetLatelyDayData(request);

        public List<EchartPineDataModel> GetMonthDataByYear(GetIndexDataRequest request) => _accessor.GetMonthDataByYear(request); 

        public Result VaildJob(JobRequest request)
        {
            if (request.Title.IsEmpty() || request.Title.Length > 50)
            {
                return new Result(-1, "标题格式错误！");
            }


            if (request.Email.IsEmpty())
            {
                return new Result(-1, "邮箱不能为空！");
            } 


            if ( request.Email.Length > 300)
            {
                return new Result(-1, "邮箱过长，请检查！");
            }

            if (request.Mobiles.Length > 300)
            {
                return new Result(-1, "手机号过长，请检查！");
            }


            if (request.Node.IsEmpty())
            {
                return new Result(-1, "至少要选择一个服务节点！");
            }

            if (request.RtStatus > 0)
            {
                if (request.RtTime.IsEmpty() || request.RtRate.IsEmpty())
                {
                    return new Result(-1, "响应超时配置不能为空！");
                }

                if (!request.RtTime.IsNumber() || request.RtTime.ToInt() <= 0 || request.RtTime.ToInt() > 9999999)
                {
                    return new Result(-1, "响应超时配置 时间格式错误！");
                }

                if (!request.RtRate.Contains("%") || !request.RtRate.Replace("%","").IsNumber() || request.RtRate.Replace("%","").ToDouble() < 0.01)
                {
                    return new Result(-1, "响应超时配置 超时率格式错误！");
                } 
            }

            if (request.HttpStatus > 0)
            {
                if (request.HttpCodes.IsEmpty() || request.HttpRate.IsEmpty())
                {
                    return new Result(-1, "请求错误配置 不能为空！");
                }

                int c = 0;

                request.HttpCodes.Replace("，", ",").Split(",").ToList().ForEach(x => {

                    if (!x.IsNumber())   c = 1;   
                });

                if (c == 1)
                {
                    return new Result(-1, "请求错误配置 HTTP状态码配置错误！");
                }

                if (!request.HttpRate.Contains("%") || !request.HttpRate.Replace("%", "").IsNumber() || request.HttpRate.Replace("%", "").ToDouble() < 0.01)
                {
                    return new Result(-1, "请求错误配置 命中率格式错误！");
                }  
            }

            if (request.IPStatus > 0)
            {
                if (request.IPRate.IsEmpty())
                {
                    return new Result(-1, "IP配置 重复率不能为空 ！");
                } 

                if (!request.IPRate.Contains("%") || !request.IPRate.Replace("%","").IsNumber() || request.IPRate.Replace("%","").ToDouble() < 0.01 )
                {
                    return new Result(-1, "IP配置 重复率格式错误！");
                }  
            }

            if (request.RequestStatus > 0)
            {
                if (request.RequestCount.IsEmpty())
                {
                    return new Result(-1, "最大请求量不能为空！");
                }

                if (!request.RequestCount.IsNumber() )
                {
                    return new Result(-1, "最大请求量只能为数字！");
                } 
            } 

            if (request.Status == 1 && request.RtStatus == 0 && request.HttpStatus == 0 && request.IPStatus == 0 && request.RequestStatus == 0)
            {
                return new Result(-1, "开启任务 至少要配置一项！");
            } 

            return new Result(1,"ok"); 
        }

        public string ParseJobRate(int rate)
        {
            if (rate == 1) return "0 0/1 * * * ?";
            if (rate == 3) return "0 0/3 * * * ?";
            if (rate == 5) return "0 0/5 * * * ?";
            if (rate == 10) return "0 0/10 * * * ?";
            if (rate == 30) return "0 0/30 * * * ?";
            if (rate == 60) return "0 0 0/1 * * ?"; 
            if (rate == 120) return "0 0 0/2 * * ?";
            if (rate == 240) return "0 0 0/4 * * ?";
            if (rate == 360) return "0 0 0/6 * * ?";
            if (rate == 480) return "0 0 0/8 * * ?";
            if (rate == 720) return "0 0 0/12 * * ?";
            if (rate == 1440) return "0 0 0 1/1 * ?";

            return "0 0/1 * * * ?";
        }

        public int ParseJobCron(string cron)
        {
            if (cron == "0 0/1 * * * ?") return 1;
            if (cron == "0 0/3 * * * ?") return 3;
            if (cron == "0 0/5 * * * ?") return 5;
            if (cron == "0 0/10 * * * ?") return 10;
            if (cron == "0 0/30 * * * ?") return 30;
            if (cron == "0 0 0/1 * * ?") return 60;
            if (cron == "0 0 0/2 * * ?") return 120;
            if (cron == "0 0 0/4 * * ?") return 240;
            if (cron == "0 0 0/6 * * ?") return 360;
            if (cron == "0 0 0/8 * * ?") return 480;
            if (cron == "0 0 0/12 * * ?") return 720;
            if (cron == "0 0 0 1/1 * ?") return 1440;

            return 1;
        }

        public string ParseJobCronString(string cron)
        {
            if (cron == "0 0/1 * * * ?") return "1分钟";
            if (cron == "0 0/3 * * * ?") return "3分钟";
            if (cron == "0 0/5 * * * ?") return "5分钟";
            if (cron == "0 0/10 * * * ?") return "10分钟";
            if (cron == "0 0/30 * * * ?") return "30分钟";
            if (cron == "0 0 0/1 * * ?") return "1小时";
            if (cron == "0 0 0/2 * * ?") return "2小时";
            if (cron == "0 0 0/4 * * ?") return "4小时";
            if (cron == "0 0 0/6 * * ?") return "6小时";
            if (cron == "0 0 0/8 * * ?") return "8小时";
            if (cron == "0 0 0/12 * * ?") return "12小时";
            if (cron == "0 0 0 1/1 * ?") return "1天";

            return "1分钟";
        }


        public  Models.Job ParseJobRequest(JobRequest request)
        {
            Models.Job job = new Models.Job();

            job.Id = request.Id;
            job.Title = request.Title;
            job.CronLike = ParseJobRate(request.Rate);
            job.Emails = request.Email.Trim().Replace("，", ",");
            job.Mobiles = request.Mobiles.Trim().Replace("，",",");
            job.Status = request.Status;
            job.Servers = request.Node.Trim();
            job.RtStatus = request.RtStatus;
            job.RtTime = request.RtTime.Trim().ToInt();
            job.RtRate = Math.Round(request.RtRate.Replace("%", "").ToDouble(),2) * 0.01;
            job.HttpStatus = request.HttpStatus;
            job.HttpCodes = request.HttpCodes.Replace("，", ",").Trim();
            job.HttpRate = Math.Round(request.HttpRate.Replace("%", "").ToDouble(),2) * 0.01;
            job.IPStatus = request.IPStatus;
            job.IPWhiteList = request.IPWhiteList.Replace("，", ",").Trim();
            job.IPRate = Math.Round(request.IPRate.Replace("%", "").ToDouble(),2) * 0.01;
            job.RequestStatus = request.RequestStatus;
            job.RequestCount = request.RequestCount.ToInt();
            job.CreateTime = DateTime.Now;

            return job; 
        }

        public List<Models.Job> GetJobs() => _accessor.GetJobs();

        public Models.Job GetJob(int Id) => _accessor.GetJob(Id);

        public void AddJob(Models.Job job) => _accessor.AddJob(job);

        public void UpdateJob(Models.Job job) => _accessor.UpdateJob(job);

        public void DeleteJob(Models.Job job) => _accessor.DeleteJob(job);

        public void CheckJob(Models.Job job)
        {
            if (job == null)
            {
                return;
            } 

            if (job.Status == 0)
            {
                return;
            }

            int mintue = ParseJobCron(job.CronLike);

            CheckRt(job,mintue);
            CheckHttp(job,mintue);
            CheckIP(job,mintue);
            CheckRequestCount(job,mintue);  
        }

        public Task CheckRt(Models.Job job,int mintue)
        {
           return Task.Run(()=> {

               if (job.RtStatus == 0)
               {
                   return;
               } 

                var CkRt = _accessor.CheckRt(job, mintue);

                if (!CkRt.Ok)
                { 
                    EmailHelper.Send(job.Emails,"预警触发通知", $@"  

                          <br>
                          <b>【响应超时】触发预警 </b> 

                          <p>超时率预警值：{job.RtRate * 100 + "%"}  当前值：{CkRt.Value.ToDouble() * 100 + "%"} </p>   

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Servers}</p>

                          <p>监控频率：{ParseJobCronString(job.CronLike)}</p>

                          <p>设定超时时间：{job.RtTime}ms</p>

                          <p>时间段：{CkRt.Time}</p>

                    ");
                }

            });  
        }

        public Task CheckHttp(Models.Job job, int mintue)
        {
            return Task.Run(() => {

                if (job.HttpStatus == 0)
                {
                    return;
                } 

                var CkHttp = _accessor.CheckHttp(job, mintue);

                if (!CkHttp.Ok)
                {
                    EmailHelper.Send(job.Emails, "预警触发通知", $@"  

                          <br>
                          <b>【请求错误】触发预警 </b> 

                          <p>命中率预警值：{job.HttpRate * 100 + "%"}  当前值：{CkHttp.Value.ToDouble() * 100 + "%"} </p>   

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Servers}</p>

                          <p>监控频率：{ParseJobCronString(job.CronLike)}</p>

                          <p>设定Http状态码：{job.HttpCodes}</p>

                          <p>时间段：{CkHttp.Time}</p>

                    ");
                }

            });
        }

        public Task CheckIP(Models.Job job, int mintue)
        {
            return Task.Run(() => {

                if (job.IPStatus == 0)
                {
                    return;
                } 

                var CkIP = _accessor.CheckIP(job, mintue);

                if (!CkIP.Ok)
                {
                    EmailHelper.Send(job.Emails, "预警触发通知", $@"  

                          <br>
                          <b>【IP异常】触发预警 </b> 

                          <p>IP重复率预警值：{job.IPRate * 100 + "%"}  当前值：{CkIP.Value.ToDouble() * 100 + "%"} </p>   

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Servers}</p>

                          <p>监控频率：{ParseJobCronString(job.CronLike)}</p>

                          <p>IP白名单：{job.IPWhiteList}</p>

                          <p>时间段：{CkIP.Time}</p>

                    ");
                } 

            });
        }

        public Task CheckRequestCount(Models.Job job, int mintue)
        {
             return Task.Run(() => { 

                 if (job.RequestStatus == 0)
                 {
                     return;
                 }

                var CkCount = _accessor.CheckRequestCount(job, mintue);

                if (!CkCount.Ok)
                {
                     EmailHelper.Send(job.Emails, "预警触发通知", $@"  

                          <br>
                          <b>【请求量监控】触发预警 </b> 

                          <p>请求量最大预警值：{job.RequestCount}  当前值：{CkCount.Value} </p>   

                          <p>任务标题：{job.Title}</p>

                          <p>监控节点：{job.Servers}</p>

                          <p>监控频率：{ParseJobCronString(job.CronLike)}</p>  

                          <p>时间段：{CkCount.Time}</p>

                    ");
                 } 

            });
        }  

    }  
}
