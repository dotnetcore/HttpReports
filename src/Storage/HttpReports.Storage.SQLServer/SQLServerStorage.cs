using Dapper;
using Dapper.Contrib.Extensions;
using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.FilterOptions;
using HttpReports.Storage.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.SQLServer
{
    public class SQLServerStorage : BaseStorage
    {
        public SQLServerStorageOptions _options;

        public SQLServerConnectionFactory ConnectionFactory { get; }

        public ILogger<SQLServerStorage> Logger { get; }  
      
        private string Prefix { get; set; } = string.Empty; 

        public SQLServerStorage(IOptions<SQLServerStorageOptions> options, SQLServerConnectionFactory connectionFactory, ILogger<SQLServerStorage> logger) 

            : base(new BaseStorageOptions
            {
                DeferSecond = options.Value.DeferSecond,
                DeferThreshold = options.Value.DeferThreshold,
                ConnectionString = options.Value.ConnectionString,
                DataType = FreeSql.DataType.SqlServer

            }) 

        {
            _options = options.Value;  

            if (!_options.TablePrefix.IsEmpty()) Prefix = _options.TablePrefix + ".";
            ConnectionFactory = connectionFactory;
            Logger = logger; 
          
        }  
       


        /// <summary>
        /// 获取Url的平均请求处理时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select TOP {filterOption.Take} Url,Avg(Milliseconds) Time FROM [{Prefix}RequestInfo] {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

            List<RequestAvgResponeTime> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<RequestAvgResponeTime>(sql)).ToList();
            }, "获取Url的平均请求处理时间统计异常");

            return result;
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $"Select '{m}' Code,COUNT(1) Total From [{Prefix}RequestInfo] {where} AND StatusCode = {m}"));

            TraceLogSql(sql);

            List<StatusCodeCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<StatusCodeCount>(sql)).ToList();
            }, "获取http状态码数量统计异常");

            return result;
        }

        public async Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            var sqlBuilder = new StringBuilder("Select Name,Total from (", 512);

            var group = filterOption.TimeGroup;
            var groupCount = group.Length / group.Rank;
            for (int i = 0; i < groupCount; i++)
            {
                var min = group[i, 0];
                var max = group[i, 1];
                if (min < max)
                {
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}-{max}' Name, Count(1) Total From [{Prefix}RequestInfo] {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}以上' Name, Count(1) Total From [{Prefix}RequestInfo] {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By ID").ToString();

            TraceLogSql(sql);

            List<ResponeTimeGroup> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<ResponeTimeGroup>(sql)).ToList();
            }, "获取http状态码分组统计异常");

            return result;
        }



        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select TOP {filterOption.Take} Url,COUNT(1) as Total From [{Prefix}RequestInfo] {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            List<UrlRequestCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<UrlRequestCount>(sql)).ToList();
            });

            return result;
        }


        /// <summary>
        /// 控制子句
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        protected string BuildSqlControl(IFilterOption filterOption)
        {
            var builder = new StringBuilder(512);

            if (filterOption is IOrderFilterOption orderFilterOption)
            {
                if (orderFilterOption.IsOrderByField)
                {
                    builder.Append($"ORDER BY {orderFilterOption.GetOrderField()} {(orderFilterOption.IsAscend ? "Asc" : "Desc")} ");
                }
                else
                {
                    builder.Append($"{(orderFilterOption.IsAscend ? "Asc" : "Desc")} ");
                }
            }

            return builder.ToString();
        }


        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"Select COUNT(1) Total From [{Prefix}RequestInfo] {where};
                Select COUNT(1) Code404 From [{Prefix}RequestInfo] {where} AND StatusCode = 404;
                Select COUNT(1) Code500 From [{Prefix}RequestInfo] {where} AND StatusCode = 500;
                Select Count(1) From ( Select Distinct Url From [{Prefix}RequestInfo] ) A;
                Select AVG(Milliseconds) ART From [{Prefix}RequestInfo] {where};";

            TraceLogSql(sql);

            IndexPageData result = new IndexPageData();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql))
                {
                    result.Total = resultReader.ReadFirstOrDefault<int>();
                    result.NotFound = resultReader.ReadFirstOrDefault<int>();
                    result.ServerError = resultReader.ReadFirstOrDefault<int>();
                    result.APICount = resultReader.ReadFirst<int>();
                    result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                    result.AvgResponseTime = double.TryParse(resultReader.ReadFirstOrDefault<string>(), out var avg) ? avg : 0;
                }
            }, "获取首页数据异常");

            return result;
        } 


        protected async Task LoggingSqlOperation(Func<IDbConnection, Task> func, string message = null, [CallerMemberName]string method = null)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    await func(connection);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Method: {method} Message: {message ?? "数据库操作异常"}");
            }
        }

        protected async Task<T> LoggingSqlOperation<T>(Func<IDbConnection, Task<T>> func, string message = null, [CallerMemberName]string method = null)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    return await func(connection);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Method: {method} Message: {message ?? "数据库操作异常"}");
                throw;
            }
        }

        protected void TraceLogSql(string sql, [CallerMemberName]string method = null)
        {
            Logger.LogTrace($"Class: {nameof(SQLServerStorage)} Method: {method} SQL: {sql}");
        }


        protected string BuildSqlFilter(IFilterOption filterOption, bool withOutStatusCode = false, bool withOutService = false)
        {
            var builder = new StringBuilder(256);

            if (!withOutService && filterOption is INodeFilterOption nodeFilterOption)
            {
                if (!nodeFilterOption.Service.IsEmpty())
                {
                    CheckSqlWhere(builder).Append($"Node = '{nodeFilterOption.Service}' ");
                }

                if (!nodeFilterOption.LocalIP.IsEmpty())
                {
                    CheckSqlWhere(builder).Append($"LocalIP = '{nodeFilterOption.LocalIP}' ");
                }

                if (nodeFilterOption.LocalPort > 0)
                {
                    CheckSqlWhere(builder).Append($"LocalPort = {nodeFilterOption.LocalPort} ");
                }
            }

            if (!withOutStatusCode && filterOption is IStatusCodeFilterOption statusCodeFilterOption && statusCodeFilterOption.StatusCodes?.Length > 0)
            {
                if (statusCodeFilterOption.StatusCodes.Length == 1)
                {
                    CheckSqlWhere(builder).Append($"StatusCode = {statusCodeFilterOption.StatusCodes[0]} ");
                }
                else
                {
                    CheckSqlWhere(builder).Append($"StatusCode in ({string.Join(",", statusCodeFilterOption.StatusCodes)}) ");
                }
            }

            if (filterOption is ITimeSpanFilterOption timeSpanFilterOption)
            {
                if (timeSpanFilterOption.StartTime.HasValue)
                {
                    CheckSqlWhere(builder).Append($"CreateTime >= '{timeSpanFilterOption.StartTime.Value.ToString(timeSpanFilterOption.StartTimeFormat)}' ");
                }
                if (timeSpanFilterOption.EndTime.HasValue)
                {
                    CheckSqlWhere(builder).Append($"CreateTime < '{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}' ");
                }
            }

            return builder.ToString();
        } 
        
        protected StringBuilder CheckSqlWhere(StringBuilder builder)
        {
            if (builder.Length == 0)
            {
                builder.Append("WHERE ");
            }
            else
            {
                builder.Append("AND ");
            }
            return builder;
        }


        /// <summary>
        /// 获取请求信息
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        {
            var whereBuilder = new StringBuilder(BuildSqlFilter(filterOption), 512);

            var sqlBuilder = new StringBuilder($"Select * From [{Prefix}RequestInfo] ", 512);

            if (whereBuilder.Length == 0)
            {
                whereBuilder.Append("Where 1=1 ");
            }

            if (!filterOption.IP.IsEmpty())
            {
                whereBuilder.Append($" AND IP = '{filterOption.IP}' ");
            }

            if (!filterOption.Url.IsEmpty())
            {
                whereBuilder.Append($" AND  Url like '%{filterOption.Url}%' ");
            }

            if (!filterOption.TraceId.IsEmpty())
            {
                whereBuilder.Append($" AND ID = '{filterOption.TraceId}' ");
            }


            if (filterOption.StatusCodes != null)
            {
                if (filterOption.StatusCodes.Length == 1)
                {
                    whereBuilder.Append($" AND StatusCode = {filterOption.StatusCodes[0]} ");
                }
                else
                {
                    whereBuilder.Append($" AND StatusCode in ({string.Join(",", filterOption.StatusCodes)}) ");
                }
            }

            // Query Detail
            IEnumerable<string> RequestIdCollection = await QueryDetailAsync(filterOption);

            if (RequestIdCollection != null && RequestIdCollection.Any())
            {
                whereBuilder.Append($" AND Id IN ({string.Join(",", RequestIdCollection.Select(x => "'" + x + "'"))}) ");
            }


            var where = whereBuilder.ToString();

            sqlBuilder.Append(where);

            var sql = sqlBuilder.ToString();

            TraceLogSql(sql);

            var countSql = $"Select count(1) From [{Prefix}RequestInfo] " + where;
            TraceLogSql(countSql);

            var result = new RequestInfoSearchResult()
            {
                SearchOption = filterOption,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.AllItemCount = connection.QueryFirstOrDefault<int>(countSql);

                result.List.AddRange((await connection.GetListBySqlAsync<RequestInfo>(sql, "CreateTime desc", filterOption.PageSize, filterOption.Page, result.AllItemCount)).ToArray());
            }, "查询请求信息列表异常");

            return result;
        }


        private async Task<IEnumerable<string>> QueryDetailAsync(RequestInfoSearchFilterOption option)
        {
            if (!option.TraceId.IsEmpty()) return null;

            if (option.Request.IsEmpty() && option.Response.IsEmpty()) return null;

            string where = " where 1=1 ";

            if (!option.Request.IsEmpty())
            {
                where = where + $"AND RequestBody like '%{option.Request.Trim()}%' ";
            }

            if (!option.Response.IsEmpty())
            {
                where = where + $"AND ResponseBody like '%{option.Response.Trim()}%' ";
            }

            if (option is ITimeSpanFilterOption timeSpanFilterOption)
            {
                if (timeSpanFilterOption.StartTime.HasValue)
                {
                    where = where + $" AND CreateTime >= '{timeSpanFilterOption.StartTime.Value.ToString(timeSpanFilterOption.StartTimeFormat)}' ";
                }
                if (timeSpanFilterOption.EndTime.HasValue)
                {
                    where = where + $" AND CreateTime < '{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}' ";
                }
            }

            string sql = $"Select RequestId From {Prefix}RequestDetail {where}  ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

            return list;
        }



        /// <summary>
        /// 获取请求次数统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $"Select {dateFormat} KeyField,COUNT(1) ValueField From [{Prefix}RequestInfo] {where} Group by {dateFormat};";

            TraceLogSql(sql);

            var result = new RequestTimesStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                var list = (await connection.QueryAsync<KVClass<string, int>>(sql)).ToList();

                if (filterOption.Type == TimeUnit.Minute)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField.Substring(11).Replace(":", "-"), item.ValueField);
                    }
                }

                if (filterOption.Type == TimeUnit.Hour)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField.Substring(8).Replace(" ", "-"), item.ValueField);
                    }
                }


                if (filterOption.Type == TimeUnit.Day)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField, item.ValueField);
                    }
                }


            }, "获取请求次数统计异常");

            return result;
        }

        /// <summary>
        /// 获取响应时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $"Select {dateFormat} KeyField,AVG(Milliseconds) ValueField From [{Prefix}RequestInfo] {where} Group by {dateFormat};";

            TraceLogSql(sql);

            var result = new ResponseTimeStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                var list = (await connection.QueryAsync<KVClass<string, int>>(sql)).ToList();

                if (filterOption.Type == TimeUnit.Minute)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField.Substring(11).Replace(":", "-"), item.ValueField);
                    }
                }

                if (filterOption.Type == TimeUnit.Hour)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField.Substring(8).Replace(" ", "-"), item.ValueField);
                    }
                }

                if (filterOption.Type == TimeUnit.Day)
                {
                    foreach (var item in list)
                    {
                        result.Items.Add(item.KeyField, item.ValueField);
                    }
                }

            }, "获取响应时间统计异常");

            return result;
        }


        private class KVClass<TKey, TValue>
        {
            public TKey KeyField { get; set; }
            public TValue ValueField { get; set; }
        }

        private static string GetDateFormat(TimeSpanStatisticsFilterOption filterOption)
        {
            string dateFormat;
            switch (filterOption.Type)
            {
                case TimeUnit.Minute:
                    dateFormat = "CONVERT(varchar(16),CreateTime,120)";
                    break;

                case TimeUnit.Hour:
                    dateFormat = "CONVERT(varchar(13),CreateTime,120)";
                    break;

                case TimeUnit.Month:
                    dateFormat = "CONVERT(varchar(7),CreateTime, 120)";
                    break;

                case TimeUnit.Day:
                default:
                    dateFormat = "CONVERT(varchar(10),CreateTime,120)";
                    break;
            }

            return dateFormat;
        }

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $"SELECT COUNT(1) FROM [{Prefix}RequestInfo] {BuildSqlFilter(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        /// <summary>
        /// 获取白名单外的获取请求总次数
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption)
        {
            var ipFilter = $"({string.Join(",", filterOption.List.Select(x => $"'{x}'"))})";
            if (filterOption.InList)
            {
                ipFilter = "IP IN " + ipFilter;
            }
            else
            {
                ipFilter = "IP NOT IN " + ipFilter;
            }

            var sql = $"SELECT TOP 1 COUNT(1) FROM [{Prefix}RequestInfo] {BuildSqlFilter(filterOption)} AND {ipFilter} Group By IP Order BY COUNT(1) Desc";
            TraceLogSql(sql);

            var max = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));


            sql = $"SELECT COUNT(1) TOTAL FROM [{Prefix}RequestInfo] {BuildSqlFilter(filterOption)} AND {ipFilter}";
            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            return (max, all);
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $"SELECT COUNT(1) FROM [{Prefix}RequestInfo] {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<bool> AddMonitorJob(MonitorJob job)
        {
            job.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into [{Prefix}MonitorJob]
            (Id,Title,Description,CronLike,Emails,WebHook,Mobiles,Status,Service,Instance,PayLoad,CreateTime)
             Values (@Id,@Title,@Description,@CronLike,@Emails,@WebHook,@Mobiles,@Status,@Service,@Instance,@PayLoad,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);

        }

        public async Task<bool> UpdateMonitorJob(MonitorJob job)
        {
            string sql = $@"Update [{Prefix}MonitorJob]

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,WebHook = @WebHook, Mobiles = @Mobiles,Status= @Status,Service = @Service,Instance = @Instance,PayLoad = @PayLoad 

                Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);
        }

        public async Task<MonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From [{Prefix}MonitorJob] Where Id = '{Id}' ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql)

            ));
        }

      

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From [{Prefix}MonitorJob] Where Id = '{Id}' ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            (await connection.ExecuteAsync(sql)) > 0);
        } 
        

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            string sql = $" Update [{Prefix}SysUser] Set UserName = @UserName , Password = @Password  Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.ExecuteAsync(sql, model)

             ) > 0);

        }

        public async Task<SysUser> GetSysUser(string UserName)
        {
            string sql = $" Select * From [{Prefix}SysUser] Where UserName = @UserName";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { UserName })

            ));
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public async Task<(RequestInfo, RequestDetail)> GetRequestInfoDetail(string Id)
        {
            string sql = $" Select * From [{Prefix}RequestInfo] Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            string detailSql = $" Select * From [{Prefix}RequestDetail] Where RequestId = @Id";

            TraceLogSql(detailSql);

            var requestDetail = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestDetail>(detailSql, new { Id })

           ));

            return (requestInfo, requestDetail);

        }

        public async Task<RequestInfo> GetRequestInfo(string Id)
        {
            string sql = $" Select * From [{Prefix}RequestInfo] Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            return requestInfo;

        }

        public async Task<List<RequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            string sql = $"Select * From [{Prefix}RequestInfo] Where ParentId = @ParentId Order By CreateTime ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryAsync<RequestInfo>(sql, new { ParentId })

           ));

            return requestInfo.Select(x => x as RequestInfo).ToList();
        }

        public async Task ClearData(string StartTime)
        {
             string sql = $"Delete From [{Prefix}RequestInfo] Where CreateTime <= @StartTime ";  
             await LoggingSqlOperation(async _ => await _.ExecuteAsync(sql, new { StartTime }));

            string detailSql = $"Delete From [{Prefix}RequestDetail] Where CreateTime <= @StartTime ";
            await LoggingSqlOperation(async _ => await _.ExecuteAsync(detailSql, new { StartTime }));

            string performanceSql = $"Delete From [{Prefix}Performance] Where CreateTime <= @StartTime "; 
            await LoggingSqlOperation(async _ => await _.ExecuteAsync(performanceSql, new { StartTime }));
        } 
       

        public async Task<List<Performance>> GetPerformances(PerformanceFilterIOption option)
        {
            string where = " where  CreateTime >= @Start AND CreateTime < @End ";

            if (!option.Service.IsEmpty())
            {
                where = where + " AND Service = @Service ";
            }

            if (!option.Instance.IsEmpty())
            {
                where = where + " AND Instance = @Instance ";
            }


            string sql = $" Select * From [{Prefix}Performance] " + where;

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryAsync<Performance>(sql, option)

           ));

            return result.Select(x => x as Performance).ToList();

        }   
     

        string ParseTimeField(string TimeField, TimeUnit timeUnit)
        {
            if (timeUnit == TimeUnit.Minute)
            {
                TimeField = TimeField.Substring(11);
            }

            if (timeUnit == TimeUnit.Hour)
            {
                TimeField = TimeField.Substring(8,2) + "-" + TimeField.Substring(11,2);
            }

            return TimeField; 
        }   
     
    }
}