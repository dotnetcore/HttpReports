using Dapper;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.PostgreSQL
{
    public class PostgreSQLStorage : BaseStorage 
    {
        public PostgreStorageOptions Options { get; }

        public PostgreConnectionFactory ConnectionFactory { get; }

        public ILogger<PostgreSQLStorage> Logger { get; } 
       
        private string Prefix { get; set; } = string.Empty;  
        
        public PostgreSQLStorage(IOptions<PostgreStorageOptions> options, PostgreConnectionFactory connectionFactory, ILogger<PostgreSQLStorage> logger)

            : base(new BaseStorageOptions {

                DeferSecond = options.Value.DeferSecond,
                DeferThreshold = options.Value.DeferThreshold,
                ConnectionString = options.Value.ConnectionString,
                DataType = FreeSql.DataType.PostgreSQL

            })
        {
            Options = options.Value;   

            ConnectionFactory = connectionFactory;
            if (!Options.TablePrefix.IsEmpty()) Prefix = Options.TablePrefix + ".";
            Logger = logger;
            
        }

        public async Task AddRequestInfoAsync(List<RequestBag> list, System.Threading.CancellationToken token)
        { 
            List<RequestInfo> requestInfos = list.Select(x => x.RequestInfo).ToList();

            List<RequestDetail> requestDetails = list.Select(x => x.RequestDetail).ToList();

            await freeSql.Insert(requestInfos).ExecuteAffrowsAsync();

            await freeSql.Insert(requestDetails).ExecuteAffrowsAsync(); 
        }

        public async Task AddRequestInfoAsync(RequestBag bag)
        {
            if (Options.EnableDefer)
            {
                _deferFlushCollection.Flush(bag);
            }
            else
            {
                await freeSql.Insert(bag.RequestInfo).ExecuteAffrowsAsync();
                await freeSql.Insert(bag.RequestDetail).ExecuteAffrowsAsync();

            }
        }

        private DynamicParameters BuildParameters<K>(List<K> data)
        {
            DynamicParameters parameters = new DynamicParameters();

            AddParameters<K>(data);

            return parameters;

            void AddParameters<T>(List<T> list)
            {
                var props = typeof(T).GetProperties().ToList();

                foreach (var item in list)
                {
                    foreach (var p in props)
                    {
                        if (p.CanRead)
                        {
                            parameters.Add(p.Name + (list.IndexOf(item) + 1), p.GetValue(item));
                        }
                    }
                }
            }
        }  
     

        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"

Select COUNT(1) AS Total From ""{Prefix}RequestInfo"" {where};
Select COUNT(1) AS Code404 From ""{Prefix}RequestInfo"" {where} AND StatusCode = 404;
Select COUNT(1) AS Code500 From ""{Prefix}RequestInfo"" {where} AND StatusCode = 500;
Select Count(1)  From ( Select Distinct Url From ""{Prefix}RequestInfo"" ) A;
Select AVG(Milliseconds) AS ART From ""{Prefix}RequestInfo"" {where};";

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



        


        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $@"Select Url,Avg(Milliseconds) AS Time FROM ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<RequestAvgResponeTime>(sql)).ToList(), "获取Url的平均请求处理时间统计异常");
        }

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $@"SELECT COUNT(1) FROM ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption)
        {
            var ipFilter = $"({string.Join(",", filterOption.List.Select(m => $"'{m}'"))})";
            if (filterOption.InList)
            {
                ipFilter = "IP IN " + ipFilter;
            }
            else
            {
                ipFilter = "IP NOT IN " + ipFilter;
            }

            var sql = $@"SELECT COUNT(1) AS TOTAL FROM ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption)} AND {ipFilter} GROUP BY IP ORDER BY TOTAL DESC LIMIT 1";
            TraceLogSql(sql);
            var max = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            sql = $@"SELECT COUNT(1) AS TOTAL FROM ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption)} AND {ipFilter}";
            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            return (max, all);
        }

        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $@"Select {dateFormat} AS KeyField,COUNT(1) AS ValueField From ""{Prefix}RequestInfo"" {where} Group by KeyField;";

            TraceLogSql(sql);

            var result = new RequestTimesStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                (await connection.QueryAsync<KVClass<string, int>>(sql)).ToList().ForEach(m =>
                {
                    result.Items.Add(m.KeyField, m.ValueField);
                });
            }, "获取请求次数统计异常");

            return result;
        }

        public async Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $@"Select {dateFormat} AS KeyField,AVG(Milliseconds) AS ValueField From ""{Prefix}RequestInfo"" {where} Group by KeyField;";

            TraceLogSql(sql);

            var result = new ResponseTimeStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                (await connection.QueryAsync<KVClass<string, int>>(sql)).ToList().ForEach(m =>
                {
                    result.Items.Add(m.KeyField, m.ValueField);
                });
            }, "获取响应时间统计异常");

            return result;
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $@"Select '{m}' AS Code,COUNT(1) AS Total From ""{Prefix}RequestInfo"" {where} AND StatusCode = {m}"));

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<StatusCodeCount>(sql)).ToList(), "获取http状态码数量统计异常");
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
                    sqlBuilder.Append($@"Select {i + 1} AS Id,'{min}-{max}' AS Name, Count(1) AS Total From ""{Prefix}RequestInfo"" {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($@"Select {i + 1} AS Id,'{min}以上' AS Name, Count(1) AS Total From ""{Prefix}RequestInfo"" {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By ID").ToString();

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<ResponeTimeGroup>(sql)).ToList(), "获取http状态码分组统计异常");
        }



        public async Task<SysUser> GetSysUser(string UserName)
        {
            string sql = $@" Select * From ""{Prefix}SysUser"" Where UserName = @UserName";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { UserName })

            ));
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $@"SELECT COUNT(1) FROM  ""{Prefix}RequestInfo"" {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $@"Select Url,COUNT(1) as Total From ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<UrlRequestCount>(sql)).ToList());
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

            string sql = $@"Select RequestId From ""{Prefix}RequestDetail"" {where}  ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

            return list;
        }

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            string sql = $@" Update ""{Prefix}SysUser"" Set UserName = @UserName , Password = @Password  Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.ExecuteAsync(sql, model)

             ) > 0);

        }

        public async Task<bool> UpdateMonitorJob(MonitorJob job)
        {
            string sql = $@"Update ""{Prefix}MonitorJob""

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,WebHook = @WebHook, Mobiles = @Mobiles,Status= @Status,Service = @Service, Instance = @Instance, PayLoad = @PayLoad 

                Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);
        }

        protected async Task LoggingSqlOperation(Func<IDbConnection, Task> func, string message = null, [System.Runtime.CompilerServices.CallerMemberName] string method = null)
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

        protected async Task<T> LoggingSqlOperation<T>(Func<IDbConnection, Task<T>> func, string message = null, [CallerMemberName] string method = null)
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
                return default(T);
            }
        }


        protected void TraceLogSql(string sql, [CallerMemberName] string method = null)
        {
            Logger.LogTrace($"Class: {nameof(PostgreSQLStorage)} Method: {method} SQL: {sql}");
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

            if (filterOption is ITakeFilterOption takeFilterOption && takeFilterOption.Take > 0)
            {
                if (takeFilterOption.Skip > 0)
                {
                    builder.Append($"LIMIT {takeFilterOption.Take} OFFSET {takeFilterOption.Skip} ");
                }
                else
                {
                    builder.Append($"LIMIT {takeFilterOption.Take} ");
                }
            }

            return builder.ToString();
        }

        private string GetDateFormat(TimeSpanStatisticsFilterOption filterOption)
        {
            string dateFormat;
            switch (filterOption.Type)
            {
                case TimeUnit.Minute:
                    dateFormat = "to_char(CreateTime,'hh24:mi')";
                    break;

                case TimeUnit.Hour:
                    dateFormat = "to_char(CreateTime,'DD-hh24')";
                    break;

                case TimeUnit.Day:
                default:
                    dateFormat = "to_char(CreateTime,'YYYY-MM-DD')";
                    break;
            }

            return dateFormat;
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

      
       

        public async Task ClearData(string StartTime)
        {
            string sql = $@"Delete From ""{Prefix}RequestInfo"" Where CreateTime <= @StartTime ";
            var result = await LoggingSqlOperation(async _ => await _.ExecuteAsync(sql, new { StartTime }));

            string detailSql = $@"Delete From ""{Prefix}RequestDetail"" Where CreateTime <= @StartTime ";
            var detailResult = await LoggingSqlOperation(async _ => await _.ExecuteAsync(detailSql, new { StartTime }));


            string performanceSql = $@"Delete From ""{Prefix}Performance"" Where CreateTime <= @StartTime ";
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


            string sql = $@" Select * From ""{Prefix}Performance"" " + where;

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryAsync<Performance>(sql, option)

           ));

            return result.Select(x => x as Performance).ToList();

        }   
      
       

        private class KVClass<TKey, TValue>
        {
            public TKey KeyField { get; set; }
            public TValue ValueField { get; set; }
        }

    }
}
