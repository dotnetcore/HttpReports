using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Dashboard.DTO;
using HttpReports.Dashboard.Models;
using HttpReports.Storage.FilterOptions;

using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    public class DataController : Controller
    {
        private readonly IHttpReportsStorage _storage;

        private static readonly IReadOnlyList<int> Hours = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };

        public DataController(IHttpReportsStorage storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> GetIndexChartA(GetIndexDataRequest request)
        {
            var startTime = request.Start.ToDateTimeOrNow().Date;
            var endTime = request.End.ToDateTimeOrNow().Date.AddDays(1);

            var nodes = request.Node.Split(',');

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
            }).ConfigureAwait(false)).Select(m => new EchartPineDataModel(m.Code.ToString(), m.Total)).ToArray();

            var ResponseTime = (await _storage.GetGroupedResponeTimeStatisticsAsync(new GroupResponeTimeFilterOption()
            {
                Nodes = nodes,
                StartTime = startTime,
                EndTime = endTime,
            }).ConfigureAwait(false)).Select(m => new EchartPineDataModel(m.Name, m.Total)).ToArray();

            return Json(new HttpResultEntity(1, "ok", new { StatusCode, ResponseTime, topRequest, topError500, Art }));
        }

        public async Task<IActionResult> GetDayStateBar(GetIndexDataRequest request)
        {
            var startTime = request.Day.ToDateTimeOrNow().Date;
            var endTime = startTime.AddDays(1);
            var nodes = request.Node?.Split(',');

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
            var nodes = request.Node?.Split(',');

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
                var day = $"{request.Month}-{i + 1}";

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
                Nodes = request.Node.Split(','),
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
                Nodes = request.Node?.Split(','),
                IPs = request.IP?.Split(','),
                Urls = request.Url?.Split(','),
                StartTime = request.Start.ToDateTimeOrNow().Date,
                EndTime = request.End.ToDateTimeOrNow().Date.AddDays(1),
                Page = request.pageNumber,
                PageSize = request.pageSize,
            }).ConfigureAwait(false);

            return Json(new { total = result.AllItemCount, rows = result.List });
        }
    }
}