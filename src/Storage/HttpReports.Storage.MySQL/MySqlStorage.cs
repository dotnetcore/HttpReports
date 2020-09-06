using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Google.Protobuf.WellKnownTypes;
using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.Abstractions.Models;
using HttpReports.Storage.FilterOptions; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : BaseStorage
    {
        public MySqlStorageOptions Options { get; }

        public MySqlConnectionFactory ConnectionFactory { get; } 
        

        private string TablePrefix { get; set; } = string.Empty;

        public ILogger<MySqlStorage> Logger { get; } 

        public MySqlStorage(IOptions<MySqlStorageOptions> options, MySqlConnectionFactory connectionFactory, ILogger<MySqlStorage> logger)

             : base(new BaseStorageOptions
             { 
                 DeferSecond = options.Value.DeferSecond,
                 DeferThreshold = options.Value.DeferThreshold,
                 ConnectionString = options.Value.ConnectionString,
                 DataType = FreeSql.DataType.MySql

             }) 

        {
            Options = options.Value;
            if (!Options.TablePrefix.IsEmpty()) TablePrefix = Options.TablePrefix + ".";
            ConnectionFactory = connectionFactory;  

            Logger = logger; 
        }

        #region Init 

        private async Task CreateDatabaseAsync()
        {
            using (var ccon = ConnectionFactory.GetConnectionWithoutDefaultDatabase())
            {
                var num = await ccon.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{ConnectionFactory.DataBase}` DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;");
                if (num == 1)
                {
                    Logger.LogWarning($"自动创建了数据库: {ConnectionFactory.DataBase}");
                }
                else
                {
                    throw new HttpReportsInitException($"自动创建数据库 {ConnectionFactory.DataBase} 异常");
                }
            }
        }

        #endregion Init 
       
        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,COUNT(1) as Total From {TablePrefix}RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<UrlRequestCount>(sql)).ToList());
        }

       

        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,Avg(Milliseconds) Time FROM {TablePrefix}RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<RequestAvgResponeTime>(sql)).ToList(), "获取Url的平均请求处理时间统计异常");
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $"Select '{m}' Code,COUNT(1) Total From {TablePrefix}RequestInfo {where} AND StatusCode = {m}"));

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
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}-{max}' Name, Count(1) Total From {TablePrefix}RequestInfo {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}以上' Name, Count(1) Total From {TablePrefix}RequestInfo {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By Id").ToString();

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<ResponeTimeGroup>(sql)).ToList(), "获取http状态码分组统计异常");
        } 




        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"Select COUNT(1) Total From {TablePrefix}RequestInfo {where};
Select COUNT(1) Code404 From {TablePrefix}RequestInfo {where} AND StatusCode = 404;
Select COUNT(1) Code500 From {TablePrefix}RequestInfo {where} AND StatusCode = 500;
Select Count(1) From ( Select Distinct Url From {TablePrefix}RequestInfo ) A;
Select AVG(Milliseconds) ART From {TablePrefix}RequestInfo {where};";

            TraceLogSql(sql);

            IndexPageData result = new IndexPageData();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql))
                {
                    result.Total = resultReader.ReadFirstOrDefault<int>();
                    result.NotFound = resultReader.ReadFirstOrDefault<int>();
                    result.ServerError = resultReader.ReadFirstOrDefault<int>();
                    result.APICount = resultReader.ReadFirstOrDefault<int>();
                    result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                    result.AvgResponseTime = double.TryParse(resultReader.ReadFirstOrDefault<string>(), out var avg) ? avg : 0;
                }
            }, "获取首页数据异常");

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
                    where = where + $" AND CreateTime < '{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}' " ;
                }
            }

            string sql = $"Select RequestId From {TablePrefix}RequestDetail {where}  ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

            return list;
        } 

        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $"Select {dateFormat} KeyField,COUNT(1) ValueField From {TablePrefix}RequestInfo {where} Group by KeyField;";

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

        /// <summary>
        /// 获取响应时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $"Select {dateFormat} KeyField,AVG(Milliseconds) ValueField From {TablePrefix}RequestInfo {where} Group by KeyField;";

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
                    result.Items.Add(m.KeyField.ToString(), m.ValueField);
                });
            }, "获取响应时间统计异常");

            return result;
        }





        #region Monitor



        #region Query

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $"SELECT COUNT(1) From {TablePrefix}RequestInfo {BuildSqlFilter(filterOption)}";

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
            var ipFilter = $"({string.Join(",", filterOption.List.Select(m => $"'{MySqlHelper.EscapeString(m)}'"))})";
            if (filterOption.InList)
            {
                ipFilter = "IP IN " + ipFilter;
            }
            else
            {
                ipFilter = "IP NOT IN " + ipFilter;
            }

            var sql = $"SELECT COUNT(1) TOTAL From {TablePrefix}RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter} GROUP BY IP ORDER BY TOTAL DESC LIMIT 1";
            TraceLogSql(sql);
            var max = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            sql = $"SELECT COUNT(1) TOTAL From {TablePrefix}RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter}";
            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            return (max, all);
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $"SELECT COUNT(1) FROM  {TablePrefix}RequestInfo {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        #endregion Query

        #endregion Monitor

        #region Base

        private static string GetDateFormat(TimeSpanStatisticsFilterOption filterOption)
        {
            string dateFormat;
            switch (filterOption.Type)
            {
                case TimeUnit.Minute:
                    dateFormat = "DATE_FORMAT(CreateTime,'%H:%i')";
                    break;

                case TimeUnit.Hour:
                    dateFormat = "DATE_FORMAT(CreateTime,'%d-%H')";
                    break;

                case TimeUnit.Month:
                    dateFormat = "DATE_FORMAT(CreateTime,'%Y-%m')";
                    break;

                case TimeUnit.Year:
                    dateFormat = "DATE_FORMAT(CreateTime,'%Y')";
                    break;

                case TimeUnit.Day:
                default:
                    dateFormat = "DATE_FORMAT(CreateTime,'%Y-%m-%d')";
                    break;
            }

            return dateFormat;
        } 
         

        private class KVClass<TKey, TValue>
        {
            public TKey KeyField { get; set; }
            public TValue ValueField { get; set; }
        }

        protected void TraceLogSql(string sql, [CallerMemberName]string method = null)
        {
            Logger.LogTrace($"Class: {nameof(MySqlStorage)} Method: {method} SQL: {sql}");
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

        protected async Task LoggingSqlOperationWithTransaction(Func<IDbConnection, IDbTransaction, Task> func, string message = null, [CallerMemberName]string method = null)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var succeed = true;
                    try
                    {
                        await func(connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        succeed = false;
                        transaction.Rollback();
                        Logger.LogError(ex, $"Method: {method} Message: {message ?? "数据库操作异常"}");
                        throw;
                    }
                    finally
                    {
                        if (succeed)
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        protected async Task<T> LoggingSqlOperationWithTransaction<T>(Func<IDbConnection, IDbTransaction, Task<T>> func, string message = null, [CallerMemberName]string method = null)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var succeed = true;
                    try
                    {
                        return await func(connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        succeed = false;
                        transaction.Rollback();
                        Logger.LogError(ex, $"Method: {method} Message: {message ?? "数据库操作异常"}");
                        throw;
                    }
                    finally
                    {
                        if (succeed)
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// where子句
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
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
                    CheckSqlWhere(builder).Append($"`LocalIP` = '{nodeFilterOption.LocalIP}' ");
                }

                if (nodeFilterOption.LocalPort > 0)
                {
                    CheckSqlWhere(builder).Append($"`LocalPort` = {nodeFilterOption.LocalPort} ");
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

        public async Task<bool> AddMonitorJob(MonitorJob job)
        {
            job.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into {TablePrefix}MonitorJob 
            (Id,Title,Description,CronLike,Emails,WebHook,Mobiles,Status,Service,Instance,PayLoad,CreateTime)
             Values (@Id,@Title,@Description,@CronLike,@Emails,@WebHook,@Mobiles,@Status,@Service,@Instance,@PayLoad,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);

        }

       


        public async Task<bool> UpdateMonitorJob(MonitorJob job)
        {
            string sql = $@"Update {TablePrefix}MonitorJob 

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,WebHook = @WebHook,Mobiles = @Mobiles,Status= @Status,Service = @Service,Instance = @Instance,PayLoad = @PayLoad 

                Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);
        }

        public async Task<MonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From {TablePrefix}MonitorJob Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql, new { Id })

            ));
        } 
    

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From {TablePrefix}MonitorJob Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            (await connection.ExecuteAsync(sql, new { Id })) > 0);
        } 

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            string sql = $" Update {TablePrefix}SysUser Set UserName = @UserName , Password = @Password  Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.ExecuteAsync(sql, model)

             ) > 0);

        }

        public async Task<SysUser> GetSysUser(string UserName)
        {
            string sql = $" Select * From {TablePrefix}SysUser Where UserName = @UserName";

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
         

        

        public async Task ClearData(string StartTime)
        {
            string sql = $"Delete From {TablePrefix}RequestInfo Where CreateTime <= @StartTime ";  
            await LoggingSqlOperation(async _ =>  await _.ExecuteAsync(sql, new { StartTime }));  

            string DetailSql = $"Delete From {TablePrefix}RequestDetail Where CreateTime <= @StartTime ";  
            await LoggingSqlOperation(async _ => await _.ExecuteAsync(DetailSql, new { StartTime })); 

            string performanceSql = $"Delete From {TablePrefix}Performance Where CreateTime <= @StartTime "; 
            await LoggingSqlOperation(async _ =>  await _.ExecuteAsync(performanceSql, new { StartTime }));

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


            string sql = $" Select * From {TablePrefix}Performance " + where;

            TraceLogSql(sql); 

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryAsync<Performance>(sql,option)

           ));

            return result.Select(x => x as Performance).ToList();

        }    
       


        #endregion Base
    }
}