using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Dashboard.DTO;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;
using HttpReports.Dashboard.Models.ViewModels;
using HttpReports.Dashboard.Services; 
using HttpReports.Dashboard.ViewModels;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.FilterOptions; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization; 

namespace HttpReports.Dashboard.Handle
{
    public class DashboardDataHandle : DashboardHandleBase
    {
        private readonly IHttpReportsStorage _storage;

        private readonly MonitorService _monitorService;

        private readonly ScheduleService _scheduleService; 

        private readonly LocalizeService _localizeService;  
        private Localize _lang => _localizeService.Current; 

        private IAuthService _authService;


        public DashboardDataHandle(IServiceProvider serviceProvider, IAuthService authService, IHttpReportsStorage storage, MonitorService monitorService, ScheduleService scheduleService,LocalizeService localizeService) : base(serviceProvider)
        {
            _storage = storage;
            _monitorService = monitorService;
            _scheduleService = scheduleService; 
            _localizeService = localizeService;
            _authService = authService;
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
                    List<string> instance = serviceInstance.Where(k => k.Service == service).Select(x=>x.Instance).ToList();

                    response.Add(new ServiceInstanceResponse {  
                         Service = service,
                         Instance = instance 
                    });
                }  
            } 

           return Json(new HttpResultEntity(1,"ok", response));  
        }


        public async Task<string> GetPerformanceChart(PerformanceFilterIOption option)
        { 
            if (option.Service.IsEmpty() || option.Service == "ALL")
            {
                option.Service = "";
            }

            if (option.Instance.IsEmpty() || option.Instance == "ALL")
            {
                option.Instance = "";
            }


            List<Performance> performances = new List<Performance>(); 

            if (option.Start > option.End)
            {
                return Json(new HttpResultEntity(-1,_lang.TimeRangeFormatError,null));
            }

            int Times = ((option.End - option.Start).TotalSeconds.ToInt() / option.TimeFormat);

            if (Times >= 60) Times = 60;

            var list = await _storage.GetPerformances(option);

            for (int i = 0; i < Times; i++)
            {
                var items = list.Where(x => x.CreateTime >= option.End.AddSeconds(-(i+1) * option.TimeFormat) && x.CreateTime < option.End.AddSeconds(-i * option.TimeFormat));

                performances.Insert(0, new Performance
                { 
                    Id = option.End.AddSeconds(-i * option.TimeFormat).ToString("mm:ss"),
                    GCGen0 = !items.Any() ? 0: items.Average(x => x.GCGen0).ToInt(),
                    GCGen1 = !items.Any() ? 0 : items.Average(x => x.GCGen1).ToInt(),
                    GCGen2 = !items.Any() ? 0 : items.Average(x => x.GCGen2).ToInt(),
                    HeapMemory = !items.Any() ? 0 : items.Average(x => x.HeapMemory).ToString().ToDouble(2),
                    ProcessCPU = !items.Any() ? 0 : items.Average(x => x.ProcessCPU).ToString().ToDouble(2),
                    ProcessMemory = !items.Any() ? 0 : items.Average(x => x.ProcessMemory).ToString().ToDouble(2),
                    ThreadCount = !items.Any() ? 0 : items.Average(x => x.ThreadCount).ToInt(),
                    PendingThreadCount = !items.Any() ? 0 : items.Average(x => x.PendingThreadCount).ToInt() 

                });
            }   

            return Json(new HttpResultEntity(1, "ok", performances));
        }
         
        public async Task<string> GetIndexChartData(GetIndexDataRequest request)
        {
            var start = (request.Start.IsEmpty() ? DateTime.Now.ToString("yyyy-MM-dd") : request.Start).ToDateTime();
            var end = (request.End.IsEmpty() ? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") : request.End).ToDateTime();

            #region BuildServiceRequest

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

            #endregion

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

            #region BuildServiceRequest

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

            #endregion 

            // 每小时请求次数
            var requestTimesStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
                Type = TimeUnit.Minute,
            });

            //每小时平均处理时间
            var responseTimeStatistics = await _storage.GetResponseTimeStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
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

            #region BuildServiceRequest

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

            #endregion 

            var responseTimeStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Service = request.Service,
                LocalIP = request.LocalIP,
                LocalPort = request.LocalPort,
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


        [AllowAnonymous]
        public async Task<string> GetLanguage()
        {
            var lang = await _storage.GetSysConfig(BasicConfig.Language);

            return Json(new HttpResultEntity(1, "ok", lang ));
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

            #region BuildService
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

            #endregion

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



        public async Task<string> GetIndexBasicData(QueryRequest request)
        { 
            var start = request.Start.ToDateTime();
            var end = request.End.ToDateTime();  

            BasicFilter filter = new BasicFilter
            { 
                Service = request.Service,
                Instance = request.Instance,
                StartTime = start,
                EndTime = end,
                Count = 6 
            };   
            
            var basic = _storage.GetIndexBasicDataAsync(filter); 

            var top = _storage.GetGroupData(filter,GroupType.Service); 

            var range =  GetTimeRange(start,end);

            var trend = _storage.GetServiceTrend(filter, range); 

            var heatmap = _storage.GetServiceHeatMap(filter,range);  
           
            await Task.WhenAll(basic,top,trend,heatmap);

            var result = new HttpResultEntity(1, "ok", new
            {
                Total = basic.Result.Total,
                ServerError = basic.Result.ServerError,
                Service = basic.Result.Service,
                Instance = basic.Result.Instance,
                Top = top.Result,
                Trend = trend.Result,
                HeatMap = heatmap.Result

            }); 

            return Json(result);
             
        }


        public async Task<string> GetDetailData(QueryDetailRequest request)
        {
            var start = request.Start.ToDateTime();
            var end = request.End.ToDateTime();

            QueryDetailFilter filter = new QueryDetailFilter
            {
                Service = request.Service,
                Instance = request.Instance,
                StartTime = start,
                EndTime = end,
                RequestId = request.RequestId,
                Url = request.Url,
                StatusCode = request.StatusCode,
                RequestBody = request.RequestBody,
                ResponseBody = request.ResponseBody,
                PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
                PageSize = request.PageSize == 0 ? 20 : request.PageSize 

            }; 

            var result = await _storage.GetSearchRequestInfoAsync(filter);

            return Json(result); 
        } 
         


        public async Task<string> GetServiceBasicData(QueryRequest request)
        {
            var start = request.Start.ToDateTime();
            var end = request.End.ToDateTime();  

            BasicFilter filter = new BasicFilter {  
                Service = request.Service,
                Instance = request.Instance,
                StartTime = start,
                EndTime = end,
                Count = 6  
            }; 

            var endpoint = _storage.GetGroupData(filter, GroupType.Route);

            var instance = _storage.GetGroupData(filter, GroupType.Instance);

            var range = GetTimeRange(start,end);

            var app = _storage.GetAppStatus(filter,range);

            await Task.WhenAll(endpoint, instance, app);

            var result = new HttpResultEntity(1, "ok", new
            {
                endpoint = endpoint.Result,
                instance = instance.Result,
                range = range,
                app = app.Result

            });

            return Json(result); 

        }


        public async Task<string> GetTopologyData(QueryRequest request)
        {
            var start = request.Start.ToDateTime();
            var end = request.End.ToDateTime();

            BasicFilter filter = new BasicFilter
            {
                Service = request.Service,
                Instance = request.Instance,
                StartTime = start,
                EndTime = end 

            };

            var Edges = await _storage.GetTopologyData(filter);

            List<string> Nodes = Edges.Select(x => x.Key).Concat(Edges.Select(x => x.StringValue)).Distinct().ToList(); 
            return Json(new { Nodes , Edges}); 

        }


        public List<string> GetTimeRange(DateTime start, DateTime end)
        {
            List<string> Time = new List<string>();

            if ((end - start).TotalHours <= 1)
            {
                while (start <= end)
                { 
                    Time.Add(start.ToString("HH:mm"));
                    start = start.AddMinutes(1); 
                } 

            }
            else
            {
                while (start <= end)
                {
                    Time.Add(start.ToString("dd-HH"));
                    start = start.AddHours(1);
                }  
            } 
            return Time;   
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


        [AllowAnonymous]
        public async Task<string> UserLogin(SysUser user)
        {
            var model = await _storage.CheckLogin(user.UserName.Trim(), user.Password.Trim().MD5());

            if (model == null)
                return Json(new HttpResultEntity(-1, _lang.Login_UserOrPassError, null));

            var token = _authService.BuildToken(); 

            return Json(new HttpResultEntity(1, _lang.Login_Success,token));
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
            var parent = await GetGrandParentRequestInfo(new ByIdRequest { Id  = req.Id });

            var tree = await GetRequestInfoTrace(new ByIdRequest { Id = parent.Id });

            return Json(new HttpResultEntity(1, "ok", tree ));
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
      

        private async Task<RequestInfo> GetGrandParentRequestInfo(ByIdRequest req)
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

        private async Task<RequestTraceTree> GetRequestInfoTrace(ByIdRequest req)
        {
            var requestInfo = await _storage.GetRequestInfo(req.Id);

            var requestInfoTrace = new RequestTraceTree
            { 
                Info = requestInfo,
                Nodes = default
            };

            var childs = await _storage.GetRequestInfoByParentId(requestInfo.Id);

            if (childs != null && childs.Count > 0)
            {
                requestInfoTrace.Nodes = new List<RequestTraceTree>();
            }

            foreach (var item in childs)
            {
                var trace = await GetRequestInfoTrace(new ByIdRequest { 
                
                    Id = item.Id

                });

                requestInfoTrace.Nodes.Add(trace);
            }

            return requestInfoTrace;
        } 

    }
}