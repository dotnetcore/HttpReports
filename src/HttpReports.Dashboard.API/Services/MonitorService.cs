using System;
using System.Linq;

using HttpReports.Dashboard.ViewModels;
using HttpReports.Models;
using HttpReports.Monitor;

using Newtonsoft.Json;

namespace HttpReports.Dashboard.Services
{
    public class MonitorService
    {
        private readonly LocalizeService _localizeService;
        private Localize Localize => _localizeService.Current;

        public MonitorService(LocalizeService localizeService)
        {
            _localizeService = localizeService;
        }

        public string VaildMonitorJob(MonitorJobRequest request)
        {
            if (request.Title.IsEmpty())
                return Localize.Monitor_TitleNotNull;

            if (request.Title.Length > 30)
                return Localize.Monitor_TitleTooLong;

            if (!request.Description.IsEmpty() && request.Description.Length > 100)
                return Localize.Monitor_DescTooLong;

            if (request.Nodes.IsEmpty())
                return Localize.Monitor_MustSelectNode;

            if (request.Emails.IsEmpty() && request.WebHook.IsEmpty())
            {
                return Localize.Monitor_EmailOrWebHookNotNull;
            }

            if (!request.Emails.IsEmpty() && request.Emails.Length > 100)
                return Localize.Monitor_EmailTooLong;

            if (!request.WebHook.IsEmpty() && request.WebHook.Length > 100)
                return Localize.Monitor_WebHookTooLong;

            if (request.ResponseTimeOutMonitor != null)
            {
                if (!request.ResponseTimeOutMonitor.TimeOutMs.IsInt() || request.ResponseTimeOutMonitor.TimeOutMs.ToInt() <= 0 || request.ResponseTimeOutMonitor.TimeOutMs.ToInt() > 1000000)
                    return Localize.Monitor_TimeOut_TimeFormatError;

                if (!VaildPercentage(request.ResponseTimeOutMonitor.Percentage))
                    return Localize.Monitor_TimeOut_PercnetError;
            }

            if (request.ErrorResponseMonitor != null)
            {
                if (request.ErrorResponseMonitor.HttpCodeStatus.IsEmpty())
                    return Localize.Monitor_Error_StatusCodeNotNull;

                if (request.ErrorResponseMonitor.HttpCodeStatus.Length > 100)
                    return Localize.Monitor_Error_StatusCodeTooLong;

                if (!VaildPercentage(request.ErrorResponseMonitor.Percentage))
                    return Localize.Monitor_Error_PercnetError;
            }

            if (request.IPMonitor != null)
            {
                if (!request.IPMonitor.WhiteList.IsEmpty() && request.IPMonitor.WhiteList.Length > 100)
                    return Localize.Monitor_IP_WhiteListTooLong;

                if (!VaildPercentage(request.IPMonitor.Percentage))
                    return Localize.Monitor_IP_PercentError;
            }

            if (request.RequestCountMonitor != null)
            {
                if (request.RequestCountMonitor.Max.ToInt() <= 0)
                    return Localize.Monitor_Request_FormatError;
            }

            if (request.ResponseTimeOutMonitor == null && request.ErrorResponseMonitor == null && request.IPMonitor == null && request.RequestCountMonitor == null)
            {
                return Localize.Monitor_MustSelectType;
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
            if (cron == "0 0/1 * * * ?") return Localize.Monitor_Time1Min;
            if (cron == "0 0/3 * * * ?") return Localize.Monitor_Time3Min;
            if (cron == "0 0/5 * * * ?") return Localize.Monitor_Time5Min;
            if (cron == "0 0/10 * * * ?") return Localize.Monitor_Time10Min;
            if (cron == "0 0/30 * * * ?") return Localize.Monitor_Time30Min;
            if (cron == "0 0 0/1 * * ?") return Localize.Monitor_Time1Hour;
            if (cron == "0 0 0/2 * * ?") return Localize.Monitor_Time2Hour;
            if (cron == "0 0 0/4 * * ?") return Localize.Monitor_Time4Hour;
            if (cron == "0 0 0/6 * * ?") return Localize.Monitor_Time6Hour;
            if (cron == "0 0 0/8 * * ?") return Localize.Monitor_Time8Hour;
            if (cron == "0 0 0/12 * * ?") return Localize.Monitor_Time12Hour;
            if (cron == "0 0 0 1/1 * ?") return Localize.Monitor_Time1Day;

            return Localize.Monitor_Time1Min;
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
                    WhiteList = (request.IPMonitor.WhiteList ?? string.Empty),
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
                    WhiteList = payload.IPMonitor.WhiteList,
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