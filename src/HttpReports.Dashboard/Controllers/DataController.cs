using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Dashboard.DTO;
using HttpReports.Dashboard.Models;
using HttpReports.Dashboard.Services;
using HttpReports.Dashboard.Services.Quartz;
using HttpReports.Dashboard.ViewModels;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    public class DataController : Controller
    {
        private readonly IHttpReportsStorage _storage;

        private readonly ISchedulerService _quartzScheduler;

        private static readonly IReadOnlyList<int> Hours = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };

        public DataController(IHttpReportsStorage storage, ISchedulerService schedulerService)
        {
            _storage = storage;
            _quartzScheduler = schedulerService;
        }

        public async Task<IActionResult> GetIndexChartA(GetIndexDataRequest request)
        {
            var startTime = request.Start.ToDateTimeOrNow().Date;
            var endTime = request.End.ToDateTimeOrNow().Date.AddDays(1);

            var nodes = request.Node.IsEmpty() ? null : request.Node.Split(',');

            var topRequest = await _storage.GetUrlRequestStatisticsAsync(new RequestInfoFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
                IsAscend = false,
                Take = request.TOP,
            }).ConfigureAwait(false);

            var topError500 = await _storage.GetUrlRequestStatisticsAsync(new RequestInfoFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
                IsAscend = false,
                Take = request.TOP,
                StatusCodes = new[] { 500 },
            }).ConfigureAwait(false);

            var fast = await _storage.GetRequestAvgResponeTimeStatisticsAsync(new RequestInfoFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
                IsAscend = true,
                Take = request.TOP,
            }).ConfigureAwait(false);

            var slow = await _storage.GetRequestAvgResponeTimeStatisticsAsync(new RequestInfoFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
                IsAscend = false,
                Take = request.TOP,
            }).ConfigureAwait(false);

            var Art = new
            {
                fast = fast.Select(m => new EchartPineDataModel(m.Url, (int)m.Time)),
                slow = slow.Select(m => new EchartPineDataModel(m.Url, (int)m.Time))
            };

            var StatusCode = (await _storage.GetStatusCodeStatisticsAsync(new RequestInfoFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
                StatusCodes = new[] { 200, 301, 302, 303, 400, 401, 403, 404, 500, 502, 503 },
            }).ConfigureAwait(false)).Where(m => m.Total > 0).Select(m => new EchartPineDataModel(m.Code.ToString(), m.Total)).ToArray();

            var ResponseTime = (await _storage.GetGroupedResponeTimeStatisticsAsync(new GroupResponeTimeFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
            }).ConfigureAwait(false)).Where(m => m.Total > 0).Select(m => new EchartPineDataModel(m.Name, m.Total)).ToArray();

            return Json(new HttpResultEntity(1, "ok", new { StatusCode, ResponseTime, topRequest, topError500, Art }));
        }

        public async Task<IActionResult> GetDayStateBar(GetIndexDataRequest request)
        {
            var startTime = request.Day.ToDateTimeOrNow().Date;
            var endTime = startTime.AddDays(1);
            var nodes = request.Node.IsEmpty() ? null : request.Node.Split(',');

            // 每小时请求次数
            var requestTimesStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                Nodes = nodes,
                Type = TimeUnit.Hour,
            }).ConfigureAwait(false);

            //每小时平均处理时间
            var responseTimeStatistics = await _storage.GetResponseTimeStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                Nodes = nodes,
                Type = TimeUnit.Hour,
            }).ConfigureAwait(false);

            List<int> timesList = new List<int>();
            List<int> avgList = new List<int>();

            foreach (var item in Hours)
            {
                // 每小时请求次数
                var times = requestTimesStatistics.Items.TryGetValue(item.ToString(), out var tTimes) ? tTimes : 0;
                //每小时平均处理时间
                var avg = responseTimeStatistics.Items.TryGetValue(item.ToString(), out var tAvg) ? tAvg : 0;

                timesList.Add(times);
                avgList.Add(avg);
            }

            return Json(new HttpResultEntity(1, "ok", new { timesList, avgList, Hours }));
        }

        public async Task<IActionResult> GetLatelyDayChart(GetIndexDataRequest request)
        {
            var startTime = $"{request.Month}-01".ToDateTimeOrDefault(() => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            var endTime = startTime.AddMonths(1);
            var nodes = request.Node.IsEmpty() ? null : request.Node.Split(',');

            var responseTimeStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                Nodes = nodes,
                Type = TimeUnit.Day,
            }).ConfigureAwait(false);

            List<string> time = new List<string>();
            List<int> value = new List<int>();

            string Range = $"{startTime.ToString("yyyy-MM-dd")}-{endTime.AddDays(-1).ToString("yyyy-MM-dd")}";

            var monthDayCount = (endTime - startTime).Days;
            for (int i = 0; i < monthDayCount; i++)
            {
                var day = $"{request.Month}-{(i + 1).ToString("D2")}";

                var times = responseTimeStatistics.Items.TryGetValue(day, out var tTimes) ? tTimes : 0;

                time.Add(string.Format("{0:00}", i + 1));
                value.Add(times);
            }

            return Json(new HttpResultEntity(1, "ok", new { time, value, Range }));
        }

        public async Task<IActionResult> GetMonthDataByYear(GetIndexDataRequest request)
        {
            var startTime = $"{request.Year}-01-01".ToDateTimeOrDefault(() => new DateTime(DateTime.Now.Year, 1, 1));
            var endTime = startTime.AddYears(1);
            var nodes = request.Node?.Split(',');

            var responseTimeStatistics = await _storage.GetRequestTimesStatisticsAsync(new TimeSpanStatisticsFilterOption()
            {
                StartTime = startTime,
                EndTime = endTime,
                Nodes = nodes,
                Type = TimeUnit.Month,
            }).ConfigureAwait(false);

            List<string> time = new List<string>();
            List<int> value = new List<int>();

            string Range = $"{request.Year}-01-{request.Year}-12";

            for (int i = 0; i < 12; i++)
            {
                var month = string.Format("{0:00}", i + 1);
                var key = $"{request.Year}-{month}";

                var times = responseTimeStatistics.Items.TryGetValue(key, out var tTimes) ? tTimes : 0;

                time.Add(month);
                value.Add(times);
            }

            return Json(new HttpResultEntity(1, "ok", new { time, value, Range }));
        }

        public IActionResult GetTimeRange(int Tag)
        {
            //TODO 将此函数放到前端直接获取
            if (Tag == 1)
            {
                return Json(new HttpResultEntity(1, "ok", new
                {
                    start = DateTime.Now.ToString("yyyy-MM-dd"),
                    end = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                }));
            }

            if (Tag == 2)
            {
                return Json(new HttpResultEntity(1, "ok", new
                {
                    start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                    end = DateTime.Now.ToString("yyyy-MM-dd")
                }));
            }

            if (Tag == 3)
            {
                return Json(new HttpResultEntity(1, "ok", new
                {
                    start = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1).ToString("yyyy-MM-dd"),
                    end = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                }));
            }

            if (Tag == 4)
            {
                return Json(new HttpResultEntity(1, "ok", new
                {
                    start = DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToString("yyyy-MM-dd"),
                    end = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                }));
            }

            return NoContent();
        }

        public IActionResult GetTimeTag(string start, string end, int TagValue)
        {
            //TODO 将此函数放到前端直接获取
            var result = new HttpResultEntity<int>(1, "ok", 0);
            if (TagValue > 0)
            {
                if (TagValue == 1 && start == DateTime.Now.ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
                {
                    return Json(new HttpResultEntity<int>(1, "ok", -1));
                }

                if (TagValue == 2 && start == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") && end == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    return Json(new HttpResultEntity<int>(1, "ok", -1));
                }

                if (TagValue == 3 && start == DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1).ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
                {
                    return Json(new HttpResultEntity<int>(1, "ok", -1));
                }

                if (TagValue == 4 && start == DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
                {
                    return Json(new HttpResultEntity<int>(1, "ok", -1));
                }
            }

            if (start.IsEmpty() && end.IsEmpty())
            {
                result = new HttpResultEntity<int>(1, "ok", 1);

                return Json(result);
            }

            if (start == DateTime.Now.ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
            {
                result = new HttpResultEntity<int>(1, "ok", 1);

                return Json(result);
            }

            if (start == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") && end == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                result = new HttpResultEntity<int>(1, "ok", 2);
                return Json(result);
            }

            if (start == DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1).ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
            {
                result = new HttpResultEntity<int>(1, "ok", 3);
                return Json(result);
            }

            if (start == DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToString("yyyy-MM-dd") && end == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
            {
                result = new HttpResultEntity<int>(1, "ok", 4);
                return Json(result);
            }

            return Json(result);
        }

        public async Task<IActionResult> GetIndexData(GetIndexDataRequest request)
        {
            var start = request.Start.ToDateTimeOrNow().Date;
            var end = request.End.ToDateTimeOrNow().Date.AddDays(1);

            var result = await _storage.GetIndexPageDataAsync(new IndexPageDataFilterOption()
            {
                Nodes = request.Node.IsEmpty() ? null :request.Node.Split(','),
                StartTime = start,
                EndTime = end,
            }).ConfigureAwait(false);

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

        public async Task<IActionResult> GetRequestList(GetRequestListRequest request)
        {
            var result = await _storage.SearchRequestInfoAsync(new RequestInfoSearchFilterOption()
            {
                Nodes = request.Node.IsEmpty() ? null : request.Node.Split(','),
                IPs = request.IP?.Split(','),
                Urls = request.Url?.Split(','),
                StartTime = request.Start.ToDateTimeOrNow().Date,
                EndTime = request.End.ToDateTimeOrNow().Date.AddDays(1),
                Page = request.pageNumber,
                PageSize = request.pageSize,
            }).ConfigureAwait(false);

            return Json(new { total = result.AllItemCount, rows = result.List });
        }

         
        public async Task<IActionResult> EditMonitorRule(AddMonitorRuleRequest request)
        {
            var CheckMonitor = VaildMonitor(request);

            if (CheckMonitor != null) return CheckMonitor; 

            // 转换类型 入库   
            var Rule = new MonitorRule()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Monitors = GetMonitorRulesByRequest(request),
                NotificationEmails = request.Email.Split(',')
            };

            if (Rule.Id == 0) 
                await _storage.AddMonitorRuleAsync(Rule).ConfigureAwait(false);
            else
                await _storage.UpdateMonitorRuleAsync(Rule).ConfigureAwait(false); 

            return  Json(new HttpResultEntity(1, "ok",null));
        }

        public async Task<IActionResult> DeleteMonitorRule(int Id)
        {
            // 先修改job , 然后删除 
            await _quartzScheduler.DeleteMonitorRuleAsync(Id).ConfigureAwait(false); 

            await _storage.DeleteMonitorRuleAsync(Id).ConfigureAwait(false);   

            return Json(new HttpResultEntity(1, "ok", null));

        }

        public async Task<IActionResult> BindMonitorRuleAndNode(string nodes,int ruleId,bool reset)
        {
            var rule = await _storage.GetMonitorRuleAsync(ruleId).ConfigureAwait(false);

            if (nodes.IsEmpty())
            {
                rule.Nodes.Clear();
            }
            else
            {
                // 重置节点
                if (reset)
                {
                    rule.Nodes = nodes.Split(',');
                } 
                else
                {
                    // 添加节点 
                    foreach (var item in nodes.Split(','))
                    {
                        if (!rule.Nodes.Contains(item))
                        {
                            rule.Nodes.Add(item);
                        }
                    } 
                } 
            } 

            await _storage.UpdateMonitorRuleAsync(rule).ConfigureAwait(false);

            // 修改后台job
            if (nodes.IsEmpty())
            {
               await _quartzScheduler.DeleteMonitorRuleAsync(ruleId).ConfigureAwait(false);
            } 

            return Json(new HttpResultEntity(1, "ok", null));
        } 


        public async Task<IActionResult> GetMonitorRuleById(int Id)
        {
            var rule = await _storage.GetMonitorRuleAsync(Id).ConfigureAwait(false);

            AddMonitorRuleRequest response = new AddMonitorRuleRequest {

                Id = rule.Id,
                Title = rule.Description,
                Description = rule.Description,
                Email = string.Join(',',rule.NotificationEmails) 
            };

            var ResponseTimeOutMonitor = rule.Monitors.Where(x => x.Type == MonitorType.ResponseTimeOut).FirstOrDefault();
            var ErrorResponseMonitor = rule.Monitors.Where(x => x.Type == MonitorType.ErrorResponse).FirstOrDefault();
            var RemoteAddressRequestTimesMonitor = rule.Monitors.Where(x => x.Type == MonitorType.ToManyRequestBySingleRemoteAddress).FirstOrDefault();
            var RequestTimesMonitor = rule.Monitors.Where(x => x.Type == MonitorType.ToManyRequestWithAddress).FirstOrDefault(); 
           
            if (ResponseTimeOutMonitor != null)
            {
                var model = ResponseTimeOutMonitor as ResponseTimeOutMonitor;

                response.ResponseTimeOutMonitor = new ResponseTimeOutMonitorViewModel
                {   
                    Id = model.Id,  
                    Interval = ParseJobCron(model.CronExpression),
                    Percentage = Math.Round(model.WarningPercentage,2) +"%",
                    TimeoutThreshold = model.TimeoutThreshold.ToString()
                };
            }

            if (ErrorResponseMonitor != null)
            { 
                var model = ErrorResponseMonitor as ErrorResponseMonitor;

                response.ErrorResponseMonitor = new ErrorResponseMonitorViewModel
                {
                    Id = model.Id,
                    Interval = ParseJobCron(model.CronExpression),
                    Percentage = Math.Round(model.WarningPercentage, 2) + "%",
                    StatusCodes = string.Join(',',model.StatusCodes)
                };
            }

            if (RemoteAddressRequestTimesMonitor != null)
            {
                var model = RemoteAddressRequestTimesMonitor as RemoteAddressRequestTimesMonitor;

                response.RemoteAddressRequestTimesMonitor = new RemoteAddressRequestTimesMonitorViewModel
                {
                    Id = model.Id,
                    Interval = ParseJobCron(model.CronExpression),
                    Percentage = Math.Round(model.WarningPercentage, 2) + "%",
                    WhileList  = string.Join(',',model.WhileList) 
                };
            }

            if (RequestTimesMonitor != null)
            {
                var model = RequestTimesMonitor as RequestTimesMonitor;

                response.RequestTimesMonitor = new RequestTimesMonitorViewModel
                {
                    Id = model.Id,
                    Interval = ParseJobCron(model.CronExpression),
                    WarningThreshold = model.WarningThreshold.ToString()
                };
            } 

            return Json(new HttpResultEntity(1, "ok", response)); 
        } 


        private List<IMonitor> GetMonitorRulesByRequest(AddMonitorRuleRequest request)
        { 
            var monitors = new List<IMonitor>();

            if (request.ResponseTimeOutMonitor != null)
            {
                monitors.Add(new ResponseTimeOutMonitor()
                {
                    Id = request.ResponseTimeOutMonitor.Id,
                    RuleId = request.Id,
                    TimeoutThreshold = request.ResponseTimeOutMonitor.TimeoutThreshold.ToInt(),
                    WarningPercentage = request.ResponseTimeOutMonitor.Percentage.Replace("%",string.Empty).ToFloat(4), 
                    CronExpression = ParseJobRate(request.ResponseTimeOutMonitor.Interval)
                });
            }

            if (request.ErrorResponseMonitor != null)
            {
                monitors.Add(new ErrorResponseMonitor()
                {
                    Id = request.ErrorResponseMonitor.Id,
                    RuleId = request.Id,
                    StatusCodes = request.ErrorResponseMonitor.StatusCodes.Replace("，",",").Split(',').Select(x => x.ToInt()).ToList(),
                    WarningPercentage = request.ErrorResponseMonitor.Percentage.Replace("%", string.Empty).ToFloat(4),
                    CronExpression = ParseJobRate(request.ErrorResponseMonitor.Interval)

                });
            }
             
            if (request.RemoteAddressRequestTimesMonitor != null)
            {
                monitors.Add(new RemoteAddressRequestTimesMonitor()
                {
                    Id = request.RemoteAddressRequestTimesMonitor.Id,
                    RuleId = request.Id,
                    WhileList = request.RemoteAddressRequestTimesMonitor.WhileList.Replace("，", ",").Split(','),
                    WarningPercentage = request.RemoteAddressRequestTimesMonitor.Percentage.Replace("%", string.Empty).ToFloat(4),
                    CronExpression = ParseJobRate(request.RemoteAddressRequestTimesMonitor.Interval) 
                });

            }

            if (request.RequestTimesMonitor != null)
            {
                monitors.Add(new RequestTimesMonitor()
                {
                    Id = request.RequestTimesMonitor.Id,
                    RuleId = request.Id,
                    WarningThreshold = request.RequestTimesMonitor.WarningThreshold.ToInt(),
                    CronExpression = ParseJobRate(request.RequestTimesMonitor.Interval)

                }); 
            }

            return monitors;  
        } 

        private string ParseJobRate(int rate)
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

        private int ParseJobCron(string cron)
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

        private string ParseJobCronString(string cron)
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

        /// <summary>
        /// 验证监控规则百分比
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        private bool VaildPercentage(string Percent)
        {
            if (Percent.IsEmpty()) return false;

            if (Percent.Last() != '%') return false;

            if (!Percent.Replace("%", string.Empty).IsNumber() || Percent.Replace("%",string.Empty).ToDouble() <= 0 ) return false;

            return true; 
        }


        /// <summary>
        /// 验证监控规则
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private JsonResult VaildMonitor(AddMonitorRuleRequest request)
        {
            if (request.Title.IsEmpty())
                return Json(new HttpResultEntity(-1, "规则标题不能为空", null));

            if (request.Title.Length >= 50)
                return Json(new HttpResultEntity(-1, "规则标题太长", null));

            if (!request.Description.IsEmpty() && request.Description.Length > 100)
                return Json(new HttpResultEntity(-1, "监控规则描述太长", null));

            if (request.Email.IsEmpty())
                return Json(new HttpResultEntity(-1, "通知邮箱不能为空", null));

            if (request.Email.Length > 100)
                return Json(new HttpResultEntity(-1, "通知邮箱太长", null));

            if (request.ErrorResponseMonitor == null && request.RemoteAddressRequestTimesMonitor == null
                && request.RequestTimesMonitor == null && request.ResponseTimeOutMonitor == null)
            {
                return Json(new HttpResultEntity(-1, "至少要选择一项监控类型", null));
            }

            if (request.ResponseTimeOutMonitor != null)
            {
                if (!request.ResponseTimeOutMonitor.TimeoutThreshold.IsInt())
                    return Json(new HttpResultEntity(-1, "超时监控-时间必须为整数", null));

                if (!VaildPercentage(request.ResponseTimeOutMonitor.Percentage))
                    return Json(new HttpResultEntity(-1, "超时监控-百分比格式错误", null));
            }

            if (request.ErrorResponseMonitor != null)
            {
                if (request.ErrorResponseMonitor.StatusCodes.IsEmpty())
                    return Json(new HttpResultEntity(-1, "请求错误监控-状态码不能为空", null));

                bool checkStatusCode = true;

                request.ErrorResponseMonitor.StatusCodes.Split(',').ToList().ForEach(x =>
                {
                    if (!x.IsInt())
                    {
                        checkStatusCode = false;
                    }
                });

                if (!checkStatusCode) return Json(new HttpResultEntity(-1, "请求错误监控-状态码格式错误", null));

                if (!VaildPercentage(request.ErrorResponseMonitor.Percentage))
                    return Json(new HttpResultEntity(-1, "请求错误监控-百分比格式错误", null));

            }

            if (request.RemoteAddressRequestTimesMonitor != null)
            {
                if (!VaildPercentage(request.RemoteAddressRequestTimesMonitor.Percentage))
                    return Json(new HttpResultEntity(-1, "IP异常监控-百分比格式错误", null));
            }

            if (request.RequestTimesMonitor != null)
            {
                if (!request.RequestTimesMonitor.WarningThreshold.IsInt())
                {
                    return Json(new HttpResultEntity(-1, "请求量监控-请求数量格式错误", null));
                }
            }

            return null;

        }

    }
}