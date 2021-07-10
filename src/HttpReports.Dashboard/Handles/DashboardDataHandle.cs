using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Core.StorageFilters;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Models;
using HttpReports.Dashboard.Models.ViewModels;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.ViewModels; 
using HttpReports.Storage.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HttpReports.Dashboard.Handles
{
    public class DashboardDataHandle : DashboardHandleBase
    {
        private readonly IHttpReportsStorage _storage; 

        private readonly IScheduleService _scheduleService; 

        private readonly ILocalizeService _localizeService;  
        private Localize _lang => _localizeService.Current; 

        private IAuthService _authService;

        private IHealthCheckService _healthCheckService;

        private readonly DashboardOptions _options;


        public DashboardDataHandle(IServiceProvider serviceProvider, IOptions<DashboardOptions> options, IHealthCheckService healthCheckService, IAuthService authService, IHttpReportsStorage storage, IScheduleService scheduleService, ILocalizeService localizeService) : base(serviceProvider)
        {
            _storage = storage;
            _options = options.Value;
            _scheduleService = scheduleService; 
            _localizeService = localizeService;
            _authService = authService;
            _healthCheckService = healthCheckService;
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

           return Json(true,response);  
        }     

        [AllowAnonymous]
        public async Task<string> GetLanguage()
        {
            var lang = await _storage.GetSysConfig(BasicConfig.Language);

            return Json(true,lang);
        } 

       
        public async Task<string> ChangeLanguage(ChangeLanguageRequest request)
        { 
            await _localizeService.SetLanguageAsync(request.Language); 

            return Json(true);
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
                Count = _options.QueryCount
            };   
            
            var basic = _storage.GetIndexBasicDataAsync(filter); 

            var top = _storage.GetGroupData(filter,GroupType.Service); 

            var range =  GetTimeRange(start,end);

            var trend = _storage.GetServiceTrend(filter, range); 

            var heatmap = _storage.GetServiceHeatMap(filter,range);  
           
            await Task.WhenAll(basic,top,trend,heatmap);

            var result = new {

                Total = basic.Result.Total,
                ServerError = basic.Result.ServerError,
                Service = basic.Result.Service,
                Instance = basic.Result.Instance,
                Top = top.Result,
                Trend = trend.Result,
                HeatMap = heatmap.Result 

            };  

            return Json(true,result);
             
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
                RequestId = request.RequestId.ToLong(),
                Route = request.Route, 
                StatusCode = request.StatusCode,
                Method = request.Method,
                RequestBody = request.RequestBody,
                ResponseBody = request.ResponseBody,
                PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
                PageSize = request.PageSize == 0 ? 20 : request.PageSize,
                MinMs = request.MinMs
            }; 

            var result = await _storage.GetSearchRequestInfoAsync(filter);

            return Json(true,result); 
        }


        public async Task<string> GetHealthCheck()
        {
            var list = await  _healthCheckService.GetServiceInstance();

            return Json(true, list.ToList());

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
                Count = _options.QueryCount
            }; 

            var endpoint = _storage.GetGroupData(filter, GroupType.Route);

            var instance = _storage.GetGroupData(filter, GroupType.Instance);

            var range = GetTimeRange(start,end);

            var app = _storage.GetAppStatus(filter,range);

            await Task.WhenAll(endpoint, instance, app);

            var result = new
            {
                endpoint = endpoint.Result,
                instance = instance.Result,
                range = range,
                app = app.Result 

            };

            return Json(true,result); 

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
            return Json(true, new { Nodes , Edges}); 

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


        public async Task<string> GetMonitorJob(ByIdRequest req)
        {
            var model = await _storage.GetMonitorJob(req.Id.ToLong());

            return Json(true,model);
        }

        public async Task<string> GetEndpoints(QueryRequest request)
        {
            BasicFilter filter = new BasicFilter
            {
                Service = request.Service,
                Instance = request.Instance  

            };

            var list = await _storage.GetEndpoints(filter);

            return Json(true, list); 
        }


        public async Task<string> GetMonitorJobs()
        { 
            var list = await _storage.GetMonitorJobs(); 

            return Json(true,list);
        } 


        public async Task<string> AddOrUpdateMonitorJob(MonitorJob job)
        { 
            bool result = false;

            if (job.Id == 0)
            { 
                result = await _storage.AddMonitorJob(job);
            }
            else
            {
                result = await _storage.UpdateMonitorJob(job);
            }

            await _scheduleService.UpdateMonitorJobAsync();

            return Json(true,result);
        } 


        public async Task<string> DeleteJob(ByIdRequest req)
        {
            var deleteJob = await _storage.GetMonitorJob(req.Id.ToLong());

            await _storage.DeleteMonitorJob(req.Id.ToLong()); 

            await _scheduleService.UpdateMonitorJobAsync(deleteJob);

            return Json(true);
        }

        public async Task<string> ChangeJobState(ByIdRequest req)
        {
            var model = await _storage.GetMonitorJob(req.Id.ToLong());

            model.Status = model.Status == 1 ? 0 : 1;

            await _storage.UpdateMonitorJob(model);

            await _scheduleService.UpdateMonitorJobAsync();

            return Json(true);
        } 


        [AllowAnonymous]
        public async Task<string> UserLogin(SysUser user)
        {
            var model = await _storage.CheckLogin(user.UserName.Trim(), user.Password.Trim().MD5());

            if (model == null)
                return Json(false, _lang.Login_UserOrPassError, null);

            var token = _authService.BuildToken(); 

            return Json(true, _lang.Login_Success, token);
        }  


        public async Task<string> UpdateAccountInfo(UpdateAccountRequest request)
        {
            var user = await _storage.GetSysUser(request.Username);

            if (user.Password != request.OldPwd.MD5())
            {
                return Json(false, _lang.User_OldPasswordError, null);
            }

            if (request.NewUserName.Length <= 2 || request.NewUserName.Length > 20)
            {
                return Json(false, _lang.User_AccountFormatError,null);
            }

            if (request.OldPwd.Length <= 5 || request.OldPwd.Length > 20)
            {
                return Json(false, _lang.User_NewPassFormatError,null);
            }  

            await _storage.UpdateLoginUser(new SysUser
            {
                Id = user.Id,
                UserName = request.NewUserName,
                Password = request.NewPwd.MD5()
            });   

            return Json(true, _lang.UpdateSuccess, null);
        }

        public async Task<string> GetTraceList(ByIdRequest req)
        {
            var parent = await GetGrandParentRequestInfo(new ByIdRequest { Id  = req.Id });

            var tree = await GetRequestInfoTrace(new ByIdRequest { Id = parent.Id.ToString() });

            return Json(true,tree);
        }

        public async Task<string> GetMonitorAlarms(QueryRequest request)
        {
            var list = await _storage.GetMonitorAlarms(new BasicFilter {  

                PageSize = 20,
                PageNumber = 1 

            });

            return Json(true,list); 

        }

        public async Task<string> GetRequestInfoDetail(ByIdRequest req)
        {
            var (requestInfo, requestDetail) = await _storage.GetRequestInfoDetail(req.Id.ToLong());

            return Json(true, new
            { 
                Info = requestInfo,
                Detail = requestDetail 

            });
        } 
      

        private async Task<RequestInfo> GetGrandParentRequestInfo(ByIdRequest req)
        {
            var requestInfo = await _storage.GetRequestInfo(req.Id.ToLong());

            if (requestInfo.ParentId == 0)
            {
                return requestInfo;
            }
            else
            {
                return await GetGrandParentRequestInfo( new ByIdRequest {  
                    Id = requestInfo.ParentId.ToString()
                });
            }
        }



        private async Task<RequestTraceTree> GetRequestInfoTrace(ByIdRequest req)
        {
            var requestInfo = await _storage.GetRequestInfo(req.Id.ToLong());

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
                
                    Id = item.Id.ToString() 

                });

                requestInfoTrace.Nodes.Add(trace);
            }

            return requestInfoTrace;
        } 

    }
}