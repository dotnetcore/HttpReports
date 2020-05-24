using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Dashboard.DTO;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Dashboard.Models.ViewModels;
using HttpReports.Dashboard.Services; 
using HttpReports.Dashboard.ViewModels;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Ocsp;

namespace HttpReports.Dashboard.Handle
{
    public class DashboardDataHandle : DashboardHandleBase
    {
        private readonly IHttpReportsStorage _storage;

        private readonly MonitorService _monitorService;

        private readonly ScheduleService _scheduleService; 

        private readonly LocalizeService _localizeService;

        private readonly Localize _lang;


        public DashboardDataHandle(IServiceProvider serviceProvider, IHttpReportsStorage storage, MonitorService monitorService, ScheduleService scheduleService,LocalizeService localizeService) : base(serviceProvider)
        {
            _storage = storage;
            _monitorService = monitorService;
            _scheduleService = scheduleService; 
            _localizeService = localizeService;
            _lang = localizeService.Current;
        }

        public async Task<string> GetServiceInstance()
        {
           var serviceInstance = await _storage.GetServiceInstance(DateTime.Now.AddDays(-1)); 

           List<ServiceInstanceResponse> response = new List<ServiceInstanceResponse>();

            if (serviceInstance != null)
            {
                var services = serviceInstance.Select(x => x.Service).Distinct(); 

                foreach (var service in services)
                {
                    List<string> instance = serviceInstance.Where(k => k.Service == service).Select(k => k.IP + ":" + k.Port).Distinct().ToList();

                    response.Add(new ServiceInstanceResponse {  
                         Service = service,
                         Instance = instance 
                    });
                }  
            } 

           return Json(new HttpResultEntity(1,"ok", response));  
        } 

        public async Task<string> GetIndexChartData(GetIndexDataRequest request)
        {
            var start = (request.Start.IsEmpty() ? DateTime.Now.ToString("yyyy-MM-dd") : request.Start).ToDateTime();
            var end = (request.End.IsEmpty() ? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") : request.End).ToDateTime();

            if (request.Service.IsEmpty() || request.Service == "ALL")
            {
                request.Service = "";
            }  

            if (request.Instance.IsEmpty() || request.Instance == "ALL")
            {
                request.LocalIP = "";
                request.LocalPort = 0;
            }
            else
            {
                request.LocalIP = request.Instance.Substring(0, request.Instance.LastIndexOf(':'));
                request.LocalPort = request.Instance.Substring(request.Instance.LastIndexOf(':') + 1).ToInt(); 
            } 
             
            var topRequest = await _storage.GetUrlRequestStatisticsAsync(new RequestInfoFilterOption()
            {
                Service = request.Service, 
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                IsAscend = false,
                Take = request.TOP,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            var topError500 = await _storage.GetUrlRequestStatisticsAsync(new RequestInfoFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                IsAscend = false,
                Take = request.TOP,
                StatusCodes = new[] { 500 },
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            var fast = await _storage.GetRequestAvgResponeTimeStatisticsAsync(new RequestInfoFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                IsAscend = true,
                Take = request.TOP,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            var slow = await _storage.GetRequestAvgResponeTimeStatisticsAsync(new RequestInfoFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                IsAscend = false,
                Take = request.TOP,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            var Art = new
            {
                fast = fast.Select(m => new EchartPineDataModel(m.Url, (int)m.Time)),
                slow = slow.Select(m => new EchartPineDataModel(m.Url, (int)m.Time))
            };

            var StatusCode = (await _storage.GetStatusCodeStatisticsAsync(new RequestInfoFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                StatusCodes = new[] { 200, 301, 302, 303, 400, 401, 403, 404, 500, 502, 503 },
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            })).Where(m => true).Select(m => new EchartPineDataModel(m.Code.ToString(), m.Total)).ToArray();

            var ResponseTime = (await _storage.GetGroupedResponeTimeStatisticsAsync(new GroupResponeTimeFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            })).Where(m => true).Select(m => new EchartPineDataModel(m.Name, m.Total)).ToArray();

            return Json(new HttpResultEntity(1, "ok", new { StatusCode, ResponseTime, topRequest, topError500, Art }));
        }

        public async Task<string> GetDayStateBar(GetIndexDataRequest request)
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0, DateTimeKind.Local).AddDays(-1);
            var endTime = DateTime.Now;

            if (request.Service.IsEmpty() || request.Service == "ALL")
            {
                request.Service = "";
            }

            if (request.Instance.IsEmpty() || request.Instance == "ALL")
            {
                request.LocalIP = "";
                request.LocalPort = 0;
            }
            else
            {
                request.LocalIP = request.Instance.Substring(0, request.Instance.LastIndexOf(':'));
                request.LocalPort = request.Instance.Substring(request.Instance.LastIndexOf(':') + 1).ToInt();
            }

            // 每小时请求次数
            var requestTimesStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                Service = request.Service,
                LocalPort = request.LocalPort,
                LocalIP = request.LocalIP,
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss", 
                Type = TimeUnit.Hour,
            });

            //每小时平均处理时间
            var responseTimeStatistics = await _storage.GetResponseTimeStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                Service = request.Service,
                LocalPort = request.LocalPort,
                LocalIP = request.LocalIP,
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss", 
                Type = TimeUnit.Hour,
            });

            List<int> timesList = new List<int>();
            List<int> avgList = new List<int>();

            List<int> hours = new List<int>();

            for (int i = 1; i <= 24; i++)
            {
                var start = startTime.AddHours(i).ToString("dd-HH");

                // 每小时请求次数
                var times = requestTimesStatistics.Items.TryGetValue(start, out var tTimes) ? tTimes : 0;
                timesList.Add(times);

                //每小时平均处理时间
                var avg = responseTimeStatistics.Items.TryGetValue(start, out var tAvg) ? tAvg : 0;
                avgList.Add(avg);

                hours.Add(startTime.AddHours(i).ToString("HH").ToInt());

            }

            return Json(new HttpResultEntity(1, "ok", new { timesList, avgList, hours }));
        }

        public async Task<string> GetMinuteStateBar(GetIndexDataRequest request)
        {
            var startTime = DateTime.Now.AddHours(-1).AddSeconds(-DateTime.Now.Second);

            var endTime = DateTime.Now;

            var nodes = request.Service.IsEmpty() ? null : request.Service.Split(',');

            // 每小时请求次数
            var requestTimesStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Nodes = nodes,
                Type = TimeUnit.Minute,
            });

            //每小时平均处理时间
            var responseTimeStatistics = await _storage.GetResponseTimeStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Nodes = nodes,
                Type = TimeUnit.Minute,
            });

            List<int> timesList = new List<int>();
            List<int> avgList = new List<int>();

            List<int> time = new List<int>();

            for (int i = 1; i <= 60; i++)
            {
                var start = startTime.AddMinutes(i).ToString("HH-mm");

                // 每小时请求次数
                var times = requestTimesStatistics.Items.TryGetValue(start, out var tTimes) ? tTimes : 0;
                timesList.Add(times);

                //每小时平均处理时间
                var avg = responseTimeStatistics.Items.TryGetValue(start, out var tAvg) ? tAvg : 0;
                avgList.Add(avg);

                time.Add(startTime.AddMinutes(i).ToString("mm").ToInt());
            }

            return Json(new HttpResultEntity(1, "ok", new { timesList, avgList, time }));
        }

        public async Task<string> GetLatelyDayChart(GetIndexDataRequest request)
        {
            var startTime = DateTime.Now.Date.AddDays(-31);
            var endTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var nodes = request.Service.IsEmpty() ? null : request.Service.Split(',');

            var responseTimeStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Nodes = nodes,
                Type = TimeUnit.Day,
            });

            List<string> time = new List<string>();
            List<int> value = new List<int>();

            var monthDayCount = (endTime - startTime).Days;
            for (int i = 1; i <= monthDayCount; i++)
            {
                var day = startTime.AddDays(i).ToString("yyyy-MM-dd");

                var times = responseTimeStatistics.Items.TryGetValue(day, out var tTimes) ? tTimes : 0;

                time.Add(startTime.AddDays(i).ToString("dd").ToInt().ToString());
                value.Add(times);
            }

            return Json(new HttpResultEntity(1, "ok", new { time, value }));
        }

        public async Task<string> GetMonthDataByYear(GetIndexDataRequest request)
        {
            var startTime = $"{request.Year}-01-01".ToDateTimeOrDefault(() => new DateTime(DateTime.Now.Year, 1, 1));
            var endTime = startTime.AddYears(1);
            var nodes = request.Service?.Split(',');

            var responseTimeStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                Nodes = nodes,
                Type = TimeUnit.Month,
            });

            List<string> time = new List<string>();
            List<int> value = new List<int>();

            string Range = $"{request.Year}-01-{request.Year}-12";

            for (int i = 0; i < 12; i++)
            {
                var month = (i + 1).ToString();

                var times = responseTimeStatistics.Items.TryGetValue(month, out var tTimes) ? tTimes : 0;

                time.Add(month);
                value.Add(times);
            }

            return Json(new HttpResultEntity(1, "ok", new { time, value, Range }));
        }

        public async Task<string> ChangeLanguage(ChangeLanguageRequest request)
        { 
            await _localizeService.SetLanguageAsync(request.Language); 

            return Json(new HttpResultEntity(1, "ok", null));
        }

        public async Task<string> GetIndexData(GetIndexDataRequest request)
        {
            var start = request.Start.ToDateTime();
            var end = request.End.ToDateTime();

            if (request.Service.IsEmpty() || request.Service == "ALL")
            {
                request.Service = "";
            }

            if (request.Instance.IsEmpty() || request.Instance == "ALL")
            {
                request.LocalIP = "";
                request.LocalPort = 0;
            }
            else
            {
                request.LocalIP = request.Instance.Substring(0, request.Instance.LastIndexOf(':'));
                request.LocalPort = request.Instance.Substring(request.Instance.LastIndexOf(':') + 1).ToInt();
            }

            var result = await _storage.GetIndexPageDataAsync(new IndexPageDataFilterOption()
            {
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                StartTime = start,
                EndTime = end,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            return Json(new HttpResultEntity(1, "ok", new
            {
                ART = result.AvgResponseTime.ToInt(),
                Total = result.Total,
                Code404 = result.NotFound,
                Code500 = result.ServerError,
                APICount = result.APICount,
                ErrorPercent = result.ErrorPercent.ToString("0.00%"),
            }));
        }

        public async Task<string> GetRequestList(GetRequestListRequest request)
        {
            var result = await _storage.SearchRequestInfoAsync(new RequestInfoSearchFilterOption()
            {
                TraceId = request.TraceId,
                StatusCodes = request.StatusCode.IsEmpty() ? null : request.StatusCode.Split(',').Select(x => x.ToInt()).ToArray(),
                Nodes = request.Node.IsEmpty() ? null : request.Node.Split(','),
                IP = request.IP,
                Url = request.Url,
                StartTime = request.Start.ToDateTime(),
                EndTime = request.End.TryToDateTime(),
                Page = request.pageNumber,
                PageSize = request.pageSize,
                IsOrderByField = true,
                Field = RequestInfoFields.CreateTime,
                IsAscend = false,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            return Json(new { total = result.AllItemCount, rows = result.List });
        }

        public async Task<string> EditMonitor(MonitorJobRequest request)
        {
            string vaild = _monitorService.VaildMonitorJob(request);

            if (!vaild.IsEmpty())
                return Json(new HttpResultEntity(-1, vaild, null));

            IMonitorJob model = _monitorService.GetMonitorJob(request);

            if (request.Id.IsEmpty() || request.Id == "0")
                await _storage.AddMonitorJob(model);
            else
                await _storage.UpdateMonitorJob(model);

            await _scheduleService.UpdateMonitorJobAsync();

            return Json(new HttpResultEntity(1, "ok", null));
        }

        public async Task<string> GetMonitor(ByIdRequest req)
        {
            string Id = req.Id;

            if (Id.IsEmpty() || Id == "0")
                return string.Empty;

            var job = await _storage.GetMonitorJob(Id);

            if (job == null)
                return string.Empty;

            var request = _monitorService.GetMonitorJobRequest(job);

            return Json(new HttpResultEntity(1, "ok", request));
        }

        public async Task<string> DeleteJob(ByIdRequest req)
        {
            await _storage.DeleteMonitorJob(req.Id);

            await _scheduleService.UpdateMonitorJobAsync();

            return Json(new HttpResultEntity(1, "ok", null));
        }

        public async Task<string> ChangeJobState(ByIdRequest req)
        {
            var model = await _storage.GetMonitorJob(req.Id);

            model.Status = model.Status == 1 ? 0 : 1;

            await _storage.UpdateMonitorJob(model);

            await _scheduleService.UpdateMonitorJobAsync();

            return Json(new HttpResultEntity(1, "ok", null));
        }

        [AllowAnonymous]
        public async Task<string> CheckUserLogin(SysUser user)
        {
            var model = await _storage.CheckLogin(user.UserName.Trim(), user.Password.Trim().MD5());

            if (model == null)
                return Json(new HttpResultEntity(-1, _lang.Login_UserOrPassError, null));

            Context.HttpContext.SetCookie(BasicConfig.LoginCookieId, user.UserName, 60 * 30 * 7);

            return Json(new HttpResultEntity(1, _lang.Login_Success, null));
        }

        public async Task<string> UpdateAccountInfo(UpdateAccountRequest request)
        {
            var user = await _storage.GetSysUser(request.Username);

            if (user.Password != request.OldPwd.MD5())
            {
                return Json(new HttpResultEntity(-1, _lang.User_OldPasswordError, null));
            }

            if (request.NewUserName.Length <= 2 || request.NewUserName.Length > 20)
            {
                return Json(new HttpResultEntity(-1, _lang.User_AccountFormatError, null));
            }

            if (request.OldPwd.Length <= 5 || request.OldPwd.Length > 20)
            {
                return Json(new HttpResultEntity(-1, _lang.User_NewPassFormatError, null));
            }

            await _storage.UpdateLoginUser(new SysUser
            {
                Id = user.Id,
                UserName = request.NewUserName,
                Password = request.NewPwd.MD5()
            });

            return Json(new HttpResultEntity(1, _lang.UpdateSuccess, null));
        }

        public async Task<string> GetTraceList(ByIdRequest req)
        {
            var parent = await GetGrandParentRequestInfo(new ByIdRequest { Id   = req.Id });

            var tree = await GetRequestInfoTrace(new ByIdRequest { Id = parent.Id });

            return Json(new HttpResultEntity(1, "ok", new List<RequestInfoTrace>() { tree }));
        }

        public async Task<string> GetRequestInfoDetail(ByIdRequest req)
        {
            var (requestInfo, requestDetail) = await _storage.GetRequestInfoDetail(req.Id);

            return Json(new HttpResultEntity(1, "ok", new
            {

                Info = requestInfo,
                Detail = requestDetail

            }));
        } 
      

        private async Task<IRequestInfo> GetGrandParentRequestInfo(ByIdRequest req)
        {
            var requestInfo = await _storage.GetRequestInfo(req.Id);

            if (requestInfo.ParentId.IsEmpty())
            {
                return requestInfo;
            }
            else
            {
                return await GetGrandParentRequestInfo( new ByIdRequest {  
                    Id = requestInfo.ParentId
                });
            }
        }

        private async Task<RequestInfoTrace> GetRequestInfoTrace(ByIdRequest req)
        {
            var requestInfo = await _storage.GetRequestInfo(req.Id);

            var requestInfoTrace = MapRequestInfo(requestInfo);

            var childs = await _storage.GetRequestInfoByParentId(requestInfo.Id);

            if (childs != null && childs.Count > 0)
            {
                requestInfoTrace.Nodes = new List<RequestInfoTrace>();
            }

            foreach (var item in childs)
            {
                var child = MapRequestInfo(item);

                var trace = await GetRequestInfoTrace(new ByIdRequest { 
                
                    Id = item.Id
                });

                requestInfoTrace.Nodes.Add(trace);
            }

            return requestInfoTrace;
        }

        private RequestInfoTrace MapRequestInfo(IRequestInfo requestInfo)
        {
            return new RequestInfoTrace
            {

                Id = requestInfo.Id,
                Text = requestInfo.Id,
                Node = requestInfo.Node,
                Url = requestInfo.Url,
                Milliseconds = requestInfo.Milliseconds,
                StatusCode = requestInfo.StatusCode,
                RequestType = requestInfo.RequestType

            };

        } 


    }
}