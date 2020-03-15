using HttpReports.Dashboard.ViewModels;
using HttpReports.Models;
using HttpReports.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class MonitorService
    {
        public string VaildMonitorJob(MonitorJobRequest request)
        {
            if (request.Title.IsEmpty())
                return "标题不能为空";

            if (request.Title.Length > 30)
                return "标题过长";

            if (!request.Description.IsEmpty() && request.Description.Length > 100)
                return "描述过长";

            if (request.Nodes.IsEmpty())
                return "至少要选择一个服务节点";

            if (request.Emails.IsEmpty() && request.WebHook.IsEmpty())
            {
                return "通知邮箱,推送地址不能都为空"; 
            }   
            
            if (request.Emails.Length > 100)
                return "邮箱过长";

            if (request.WebHook.Length > 100)
                return "推送地址过长";

            if (request.ResponseTimeOutMonitor != null)
            {
                if (!request.ResponseTimeOutMonitor.TimeOutMs.IsInt() || request.ResponseTimeOutMonitor.TimeOutMs.ToInt() <= 0 || request.ResponseTimeOutMonitor.TimeOutMs.ToInt() > 1000000)
                    return "响应超时监控-超时时间格式错误";

                if (!VaildPercentage(request.ResponseTimeOutMonitor.Percentage))
                    return "响应超时监控-百分比格式错误";
            }

            if (request.ErrorResponseMonitor != null)
            {
                if (request.ErrorResponseMonitor.HttpCodeStatus.IsEmpty())
                    return "请求错误监控-HTTP状态码不能为空";

                if (request.ErrorResponseMonitor.HttpCodeStatus.Length > 100)
                    return "请求错误监控-HTTP状态码过长";

                if (!VaildPercentage(request.ErrorResponseMonitor.Percentage))
                    return "请求错误监控-百分比格式错误";

            }

            if (request.IPMonitor != null)
            {
                if (!request.IPMonitor.WhileList.IsEmpty() && request.IPMonitor.WhileList.Length > 100)
                    return "IP监控-白名单过长";

                if (!VaildPercentage(request.IPMonitor.Percentage))
                    return "IP监控-百分比格式错误";

            }

            if (request.RequestCountMonitor != null)
            {
                if (request.RequestCountMonitor.Max.ToInt() <= 0)
                    return "请求量监控-最大请求数格式错误";
            }

            if (request.ResponseTimeOutMonitor == null && request.ErrorResponseMonitor == null && request.IPMonitor == null && request.RequestCountMonitor == null)
            {
                return "至少要开启一项监控类型";
            }

            return null;
        }

        /// <summary>
        /// 验证监控规则百分比
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public bool VaildPercentage(string Percent)
        {
            if (Percent.IsEmpty()) return false;

            if (Percent.Last() != '%') return false;

            if (!Percent.Replace("%", string.Empty).IsNumber() || Percent.Replace("%", string.Empty).ToDouble() <= 0) return false;

            return true;
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

        public MonitorJob GetMonitorJob(MonitorJobRequest request)
        {
            MonitorJobPayload payload = new MonitorJobPayload();

            if (request.ResponseTimeOutMonitor != null)
            {
                payload.ResponseTimeOutMonitor = new ResponseTimeOutMonitorJob
                {
                    Status = request.ResponseTimeOutMonitor.Status,
                    TimeOutMs = request.ResponseTimeOutMonitor.TimeOutMs.ToInt(),
                    Percentage = request.ResponseTimeOutMonitor.Percentage.Replace("%", "").ToDouble(2)
                };
            }

            if (request.ErrorResponseMonitor != null)
            {
                payload.ErrorResponseMonitor = new ErrorResponseMonitorJob
                {

                    Status = request.ErrorResponseMonitor.Status,
                    HttpCodeStatus = request.ErrorResponseMonitor.HttpCodeStatus.Replace("，", ","),
                    Percentage = request.ErrorResponseMonitor.Percentage.Replace("%", "").ToDouble(2)
                };
            }

            if (request.IPMonitor != null)
            {
                payload.IPMonitor = new IPMonitorJob
                {
                    Status = request.IPMonitor.Status,
                    WhileList = (request.IPMonitor.WhileList ?? string.Empty),
                    Percentage = request.IPMonitor.Percentage.Replace("%", "").ToDouble(2)
                };
            }

            if (request.RequestCountMonitor != null)
            {
                payload.RequestCountMonitor = new RequestCountMonitorJob
                {

                    Status = request.RequestCountMonitor.Status,
                    Max = request.RequestCountMonitor.Max.ToInt()
                };
            }

            MonitorJob model = new MonitorJob()
            {

                Id = request.Id,
                Title = request.Title,
                Description = (request.Description ?? string.Empty),
                CronLike = ParseJobRate(request.Interval),
                Emails = request.Emails.Replace("，", ","),
                WebHook = request.WebHook,
                Mobiles = (request.Mobiles ?? string.Empty).Replace("，", ","),
                Status = request.Status,
                Nodes = request.Nodes,
                Payload = JsonConvert.SerializeObject(payload),
                CreateTime = DateTime.Now
            };

            return model;
        }

        public MonitorJobRequest GetMonitorJobRequest(IMonitorJob job)
        {
            MonitorJobPayload payload = JsonConvert.DeserializeObject<MonitorJobPayload>(job.Payload);

            MonitorJobRequest request = new MonitorJobRequest()
            { 
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Emails = job.Emails,
                Interval = ParseJobCron(job.CronLike),
                Status = job.Status,
                Mobiles = job.Mobiles,
                Nodes = job.Nodes,
                WebHook = job.WebHook
            };

            if (payload.ResponseTimeOutMonitor != null)
            {
                request.ResponseTimeOutMonitor = new ResponseTimeOutMonitorViewModel
                {
                    Status = payload.ResponseTimeOutMonitor.Status,
                    TimeOutMs = payload.ResponseTimeOutMonitor.TimeOutMs.ToString(),
                    Percentage = payload.ResponseTimeOutMonitor.Percentage + "%"
                };
            }

            if (payload.ErrorResponseMonitor != null)
            {
                request.ErrorResponseMonitor = new ErrorResponseMonitorViewModel
                {

                    Status = payload.ErrorResponseMonitor.Status,
                    HttpCodeStatus = payload.ErrorResponseMonitor.HttpCodeStatus,
                    Percentage = payload.ErrorResponseMonitor.Percentage + "%"
                };
            }

            if (payload.IPMonitor != null)
            {
                request.IPMonitor = new IPMonitorViewModel
                {

                    Status = payload.IPMonitor.Status,
                    WhileList = payload.IPMonitor.WhileList,
                    Percentage = payload.IPMonitor.Percentage + "%"

                };
            }

            if (payload.RequestCountMonitor != null)
            {
                request.RequestCountMonitor = new RequestCountMonitorViewModel
                {

                    Status = payload.RequestCountMonitor.Status,
                    Max = payload.RequestCountMonitor.Max.ToString()
                };

            }

            return request;
        }

    }
}
