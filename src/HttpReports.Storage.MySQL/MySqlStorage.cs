using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
using Dapper; 
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 
using MySql.Data.MySqlClient;  
using Newtonsoft.Json;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : IHttpReportsStorage
    {
        public MySqlStorageOptions Options { get; }

        public MySqlConnectionFactory ConnectionFactory { get; }

        public ILogger<MySqlStorage> Logger { get; }

        private readonly AsyncCallbackDeferFlushCollection<IRequestInfo> _deferFlushCollection = null;

        public MySqlStorage(IOptions<MySqlStorageOptions> options, MySqlConnectionFactory connectionFactory, ILogger<MySqlStorage> logger)
        {
            Options = options.Value;
            ConnectionFactory = connectionFactory;
            Logger = logger;
            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<IRequestInfo>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferTime);
            }
        }

        #region Init

        public async Task InitAsync()
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS `RequestInfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Node` varchar(50) DEFAULT NULL,
  `Route` varchar(50) DEFAULT NULL,
  `Url` varchar(255) DEFAULT NULL,
  `Method` varchar(16) DEFAULT NULL,
  `Milliseconds` int(11) DEFAULT NULL,
  `StatusCode` int(11) DEFAULT NULL,
  `IP` varchar(50) DEFAULT NULL,
  `CreateTime` datetime(3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_node` (`Node`) USING HASH,
  KEY `idx_status_code` (`StatusCode`) USING HASH,
  KEY `idx_create_time` (`CreateTime`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;").ConfigureAwait(false);

                    await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS `MonitorJob` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CronLike` varchar(255) DEFAULT NULL,
  `Emails` varchar(1000) DEFAULT NULL,
  `Mobiles` varchar(1000) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `Nodes` varchar(255) DEFAULT NULL,
  `PayLoad` varchar(2000) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;").ConfigureAwait(false);

                } 
            }
            catch (Exception ex)
            { 
                throw ex;
            }  
        }

        private async Task CreateDatabaseAsync()
        {
            using (var ccon = ConnectionFactory.GetConnectionWithoutDefaultDatabase())
            {
                var num = await ccon.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{ConnectionFactory.DataBase}` DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;").ConfigureAwait(false);
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

        private async Task AddRequestInfoAsync(IEnumerable<IRequestInfo> requests, CancellationToken token)
        {
            await LoggingSqlOperation(async connection =>
            {
                var values = string.Join(",", requests.Select(m => $"('{MySqlHelper.EscapeString(m.Node)}','{MySqlHelper.EscapeString(m.Route)}','{MySqlHelper.EscapeString(m.Url)}','{MySqlHelper.EscapeString(m.Method)}',{m.Milliseconds},{m.StatusCode},'{MySqlHelper.EscapeString(m.IP)}','{m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}')"));

                await connection.ExecuteAsync($"INSERT INTO `RequestInfo`(`Node`, `Route`, `Url`, `Method`, `Milliseconds`, `StatusCode`, `IP`, `CreateTime`) VALUES {values}").ConfigureAwait(false);
            }, "请求数据批量保存失败").ConfigureAwait(false);
        }

        public async Task AddRequestInfoAsync(IRequestInfo request)
        {
            if (Options.EnableDefer)
            {
                _deferFlushCollection.Add(request);
            }
            else
            {
                await LoggingSqlOperation(async connection =>
                {
                    System.Threading.Thread.Sleep(3000);

                    await connection.ExecuteAsync("INSERT INTO `RequestInfo`(`Node`, `Route`, `Url`, `Method`, `Milliseconds`, `StatusCode`, `IP`, `CreateTime`) VALUES (@Node, @Route, @Url, @Method, @Milliseconds, @StatusCode, @IP, @CreateTime)", request).ConfigureAwait(false);
                }, "请求数据保存失败").ConfigureAwait(false);
            }
        }

        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,COUNT(1) as Total From RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<UrlRequestCount>(sql).ConfigureAwait(false)).ToList()).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取所有节点信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<NodeInfo>> GetNodesAsync() =>
            await LoggingSqlOperation(async connection => (await connection.QueryAsync<string>("Select Distinct Node FROM RequestInfo;").ConfigureAwait(false)).Select(m => new NodeInfo { Name = m }).ToList(), "获取所有节点信息失败").ConfigureAwait(false);
        
        /// <summary>
        /// 获取Url的平均请求处理时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,Avg(Milliseconds) Time FROM RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<RequestAvgResponeTime>(sql).ConfigureAwait(false)).ToList(), "获取Url的平均请求处理时间统计异常").ConfigureAwait(false);
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $"Select '{m}' Code,COUNT(1) Total From RequestInfo {where} AND StatusCode = {m}"));

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<StatusCodeCount>(sql).ConfigureAwait(false)).ToList(), "获取http状态码数量统计异常").ConfigureAwait(false);
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
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}-{max}' Name, Count(1) Total From RequestInfo {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}以上' Name, Count(1) Total From RequestInfo {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By ID").ToString();

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<ResponeTimeGroup>(sql).ConfigureAwait(false)).ToList(), "获取http状态码分组统计异常").ConfigureAwait(false);
        }

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"Select COUNT(1) Total From RequestInfo {where};
Select COUNT(1) Code404 From RequestInfo {where} AND StatusCode = 404;
Select COUNT(1) Code500 From RequestInfo {where} AND StatusCode = 500;
Select Count(1) From ( Select Distinct Url From RequestInfo ) A;
Select AVG(Milliseconds) ART From RequestInfo {where};";

            TraceLogSql(sql);

            IndexPageData result = new IndexPageData();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql).ConfigureAwait(false))
                {
                    result.Total = resultReader.ReadFirstOrDefault<int>();
                    result.NotFound = resultReader.ReadFirstOrDefault<int>();
                    result.ServerError = resultReader.ReadFirstOrDefault<int>();
                    result.APICount = resultReader.ReadFirst<int>();
                    result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                    result.AvgResponseTime = double.TryParse(resultReader.ReadFirstOrDefault<string>(), out var avg) ? avg : 0;
                }
            }, "获取首页数据异常").ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 获取请求信息
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        {
            var whereBuilder = new StringBuilder(BuildSqlFilter(filterOption), 512);

            var sqlBuilder = new StringBuilder("Select * From RequestInfo ", 512);

            if (whereBuilder.Length == 0)
            {
                whereBuilder.Append("Where 1=1 ");
            }

            if (filterOption.IPs?.Length > 0)
            {
                if (filterOption.IPs.Length == 1)
                {
                    whereBuilder.Append($" AND IP = '{filterOption.IPs.First()}' ");
                }
                else
                {
                    whereBuilder.Append($" AND IP IN ({string.Join(",", filterOption.IPs.Select(m => $"'{m}'"))}) ");
                }
            }

            if (filterOption.Urls?.Length > 0)
            {
                if (filterOption.Urls.Length > 1)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MySqlStorage)}暂时只支持单条Url查询");
                }
                whereBuilder.Append($" AND  Url like '%{filterOption.Urls[0]}%' ");
            }

            var where = whereBuilder.ToString();

            sqlBuilder.Append(where);
            sqlBuilder.Append(BuildSqlControl(filterOption));

            var sql = sqlBuilder.ToString();

            TraceLogSql(sql);

            var countSql = "Select count(1) From RequestInfo " + where;
            TraceLogSql(countSql);

            var result = new RequestInfoSearchResult()
            {
                SearchOption = filterOption,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.AllItemCount = connection.QueryFirstOrDefault<int>(countSql);

                result.List.AddRange((await connection.QueryAsync<RequestInfo>(sql).ConfigureAwait(false)).ToArray());
            }, "查询请求信息列表异常").ConfigureAwait(false);

            return result;
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

            string sql = $"Select {dateFormat} KeyField,COUNT(1) ValueField From RequestInfo {where} Group by KeyField;";

            TraceLogSql(sql);

            var result = new RequestTimesStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                (await connection.QueryAsync<KVClass<string, int>>(sql).ConfigureAwait(false)).ToList().ForEach(m =>
                {
                    result.Items.Add(m.KeyField.Split('-').Last().ToInt().ToString(), m.ValueField);
                });
            }, "获取请求次数统计异常").ConfigureAwait(false);

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

            string sql = $"Select {dateFormat} KeyField,AVG(Milliseconds) ValueField From RequestInfo {where} Group by KeyField;";

            TraceLogSql(sql);

            var result = new ResponseTimeStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.Items = new Dictionary<string, int>();
                (await connection.QueryAsync<KVClass<string, int>>(sql).ConfigureAwait(false)).ToList().ForEach(m =>
                {
                    result.Items.Add(m.KeyField.ToInt().ToString(), m.ValueField);
                });
            }, "获取响应时间统计异常").ConfigureAwait(false);

            return result;
        }

        #region Monitor

        #region Base

        private static async Task UpdateMonitorRuleAppliedAsync(IDbConnection connection, IDbTransaction transaction, int ruleId, IList<string> nodes)
        {
            var sql = $"DELETE FROM `MonitorRuleApplied` WHERE `RuleId` = {ruleId};";
            await connection.ExecuteAsync(sql, transaction: transaction).ConfigureAwait(false);

            if (nodes?.Count > 0)
            {
                sql = $"INSERT INTO `MonitorRuleApplied`(`RuleId`, `Node`) VALUES {string.Join(",", nodes.Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => $"({ruleId},'{MySqlHelper.EscapeString(m)}')"))};";

                if (await connection.ExecuteAsync(sql, transaction: transaction).ConfigureAwait(false) <= 0)
                {
                    throw new StorageException("更新 MonitorRuleApplied 表失败");
                }
            }
        }

        private static async Task InsertMonitorAsync(IDbConnection connection, IDbTransaction transaction, int ruleId, IList<IMonitor> monitors)
        {
            if (monitors?.Count > 0)
            {
                var sql = $"INSERT INTO `Monitor`(`RuleId`, `Type`, `Description`, `CronExpression`, `Payload`) VALUES {string.Join(",", monitors.Select(m => $"({ruleId}, {(int)m.Type}, '{MySqlHelper.EscapeString(m.Description)}', '{MySqlHelper.EscapeString(m.CronExpression)}', '{MySqlHelper.EscapeString(JsonConvert.SerializeObject(m))}')"))};";

                if (await connection.ExecuteAsync(sql, transaction: transaction).ConfigureAwait(false) <= 0)
                {
                    throw new StorageException("插入 Monitor 表失败");
                }
            }
        }

        private static IMonitor DeserializeMonitor(MonitorType type, string json)
        {
            IMonitor monitor = null;
            switch (type)
            {
                case MonitorType.ResponseTimeOut:
                    monitor = JsonConvert.DeserializeObject<ResponseTimeOutMonitor>(json);
                    break;

                case MonitorType.ErrorResponse:
                    monitor = JsonConvert.DeserializeObject<ErrorResponseMonitor>(json);
                    break;

                case MonitorType.ToManyRequestWithAddress:
                    monitor = JsonConvert.DeserializeObject<RequestTimesMonitor>(json);
                    break;

                case MonitorType.ToManyRequestBySingleRemoteAddress:
                    monitor = JsonConvert.DeserializeObject<RemoteAddressRequestTimesMonitor>(json);
                    break;
            }

            if (monitor == null)
            {
                throw new HttpReportsException($"未知的监控类型：{type} json: {json}");
            }
            return monitor;
        }

        internal class MonitorRuleApplied
        {
            public int RuleId { get; set; }
            public string Node { get; set; }
        }

        internal class UnTypedMonitor
        {
            public int Id { get; set; }
            public int RuleId { get; set; }
            public MonitorType Type { get; set; }
            public string Payload { get; set; }
        }

        internal class TempMonitorRule
        {
            /// <summary>
            /// 唯一ID
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 规则标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 规则描述
            /// </summary>
            public string Description { get; set; }

            public string NotificationEmails { get; set; }

            public string NotificationPhoneNumbers { get; set; }
        }

        #endregion Base 
       

        #region Query

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $"SELECT COUNT(1) FROM RequestInfo {BuildSqlFilter(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql).ConfigureAwait(false));
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

            var sql = $"SELECT COUNT(1) TOTAL FROM RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter} GROUP BY IP ORDER BY TOTAL DESC LIMIT 1";
            TraceLogSql(sql);
            var max = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql).ConfigureAwait(false));
            sql = $"SELECT COUNT(1) TOTAL FROM RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter}";
            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql).ConfigureAwait(false));
            return (max, all);
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $"SELECT COUNT(1) FROM  RequestInfo {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql).ConfigureAwait(false));
        }

        #endregion Query

        #endregion Monitor

        #region Base

        private static string GetDateFormat(TimeSpanStatisticsFilterOption filterOption)
        {
            string dateFormat;
            switch (filterOption.Type)
            {
                case TimeUnit.Hour:
                    dateFormat = "Hour(CreateTime)";
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
                    await func(connection).ConfigureAwait(false);
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
                    return await func(connection).ConfigureAwait(false);
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
                        await func(connection, transaction).ConfigureAwait(false);
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
                        return await func(connection, transaction).ConfigureAwait(false);
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
        protected string BuildSqlFilter(IFilterOption filterOption, bool withOutStatusCode = false)
        {
            var builder = new StringBuilder(256);

            if (filterOption is INodeFilterOption nodeFilterOption && nodeFilterOption.Nodes?.Length > 0)
            {
                CheckSqlWhere(builder).Append($"Node in ({string.Join(",", nodeFilterOption.Nodes.Select(m => $"'{MySqlHelper.EscapeString(m)}'"))}) ");
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

        public async Task<bool> AddMonitorJob(IMonitorJob job)
        {
            string sql = $@"Insert Into MonitorJob 
            (Title,Description,CronLike,Emails,Mobiles,Status,Nodes,PayLoad,CreateTime)
             Values (@Title,@Description,@CronLike,@Emails,@Mobiles,@Status,@Nodes,@PayLoad,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql,job).ConfigureAwait(false)
            
            ) > 0 ).ConfigureAwait(false);

        }

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            string sql = $@"Update MonitorJob 

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,Mobiles = @Mobiles,Status= @Status,Nodes = @Nodes,PayLoad = @PayLoad 

                Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job).ConfigureAwait(false)

            ) > 0).ConfigureAwait(false);
        }

        public async Task<IMonitorJob> GetMonitorJob(int Id)
        {
            string sql = $@"Select * From MonitorJob Where Id = " + Id;

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql).ConfigureAwait(false) 

            )).ConfigureAwait(false);
        }

        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            string sql = $@"Select * From MonitorJob ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.QueryAsync<MonitorJob>(sql).ConfigureAwait(false)

            ).ToList().Select(x => x as IMonitorJob).ToList()).ConfigureAwait(false);
        }

        public async Task<bool> DeleteMonitorJob(int Id)
        {
            string sql = $@"Delete From MonitorJob Where Id = " + Id;

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            ( await connection.ExecuteAsync(sql).ConfigureAwait(false)) > 0 ).ConfigureAwait(false);
        }

        #endregion Base
    }
}