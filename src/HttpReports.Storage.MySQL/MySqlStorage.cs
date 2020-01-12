using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using HttpReports.Models;
using HttpReports.Storage.FilterOptions;

using Microsoft.Extensions.Logging;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : IHttpReportsStorage
    {
        public MySqlConnectionFactory ConnectionFactory { get; }

        public ILogger<MySqlStorage> Logger { get; }

        public MySqlStorage(MySqlConnectionFactory connectionFactory, ILogger<MySqlStorage> logger)
        {
            ConnectionFactory = connectionFactory;
            Logger = logger;
        }

        public async Task InitAsync()
        {
            using (var con = ConnectionFactory.GetConnection())
            {
                if (con.QueryFirstOrDefault<int>("  Select count(1) from information_schema.tables where table_name ='RequestInfo' and table_schema = 'HttpReports'; ") == 0)
                {
                    await con.ExecuteAsync(@"
                        CREATE TABLE `RequestInfo` (
                          `Id` int(11) NOT NULL auto_increment,
                          `Node` varchar(50) default NULL,
                          `Route` varchar(50) default NULL,
                          `Url` varchar(200) default NULL,
                          `Method` varchar(50) default NULL,
                          `Milliseconds` int(11) default NULL,
                          `StatusCode` int(11) default NULL,
                          `IP` varchar(50) default NULL,
                          `CreateTime` datetime default NULL,
                          PRIMARY KEY  (`Id`)
                        ) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;  ").ConfigureAwait(false);
                }

                if (con.QueryFirstOrDefault<int>(" Select count(1) from information_schema.tables where table_name ='Job' and table_schema = 'HttpReports'; ") == 0)
                {
                    await con.ExecuteAsync(@"

                            CREATE TABLE `Job` (
                              `Id` int(11) NOT NULL AUTO_INCREMENT,
                              `Title` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `CronLike` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Emails` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Mobiles` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Status` int(2) DEFAULT NULL,
                              `Servers` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `RtStatus` int(2) DEFAULT NULL,
                              `RtTime` int(11) DEFAULT NULL,
                              `RtRate` double DEFAULT NULL,
                              `HttpStatus` int(11) DEFAULT NULL,
                              `HttpCodes` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `HttpRate` double DEFAULT NULL,
                              `IPStatus` int(11) DEFAULT NULL,
                              `IPWhiteList` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `IPRate` double DEFAULT NULL,
                              `CreateTime` datetime DEFAULT NULL,
                              `RequestStatus` int(2) DEFAULT NULL,
                              `RequestCount` int(11) DEFAULT NULL,
                              PRIMARY KEY (`Id`)
                            ) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

                     ").ConfigureAwait(false);
                }
            }
        }

        public async Task AddRequestInfoAsync(IRequestInfo request)
        {
            //TODO 实现批量存储
            await LoggingSqlOperation(async connection =>
            {
                await connection.InsertAsync(request as RequestInfo).ConfigureAwait(false);
                //await connection.ExecuteAsync("insert into RequestInfo(Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime) values(@Node,@Route,@Url,@Method,@Milliseconds,@StatusCode,@IP,@CreateTime)", request).ConfigureAwait(false);
            }, "请求数据保存失败").ConfigureAwait(false);
        }

        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,COUNT(1) as Total From RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            Logger.LogTrace($"{nameof(GetUrlRequestStatisticsAsync)} SQL: {sql}");

            List<UrlRequestCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<UrlRequestCount>(sql).ConfigureAwait(false)).ToList();
            }).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 获取所有节点信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<NodeInfo>> GetNodesAsync()
        {
            string[] nodeNames = null;
            await LoggingSqlOperation(async connection =>
            {
                nodeNames = (await connection.QueryAsync<string>("Select Distinct Node FROM RequestInfo;").ConfigureAwait(false)).ToArray();
            }, "获取所有节点信息失败").ConfigureAwait(false);

            return nodeNames?.Select(m => new NodeInfo { Name = m }).ToList();
        }

        /// <summary>
        /// 获取Url的平均请求处理时间统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select Url,Avg(Milliseconds) Time FROM RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            Logger.LogTrace($"{nameof(GetRequestAvgResponeTimeStatisticsAsync)} SQL: {sql}");

            List<RequestAvgResponeTime> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<RequestAvgResponeTime>(sql).ConfigureAwait(false)).ToList();
            }, "获取Url的平均请求处理时间统计异常").ConfigureAwait(false);

            return result;
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $"Select '{m}' Code,COUNT(1) Total From RequestInfo {where} AND StatusCode = {m}"));

            Logger.LogTrace($"{nameof(GetStatusCodeStatisticsAsync)} SQL: {sql}");

            List<StatusCodeCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<StatusCodeCount>(sql).ConfigureAwait(false)).ToList();
            }, "获取http状态码数量统计异常").ConfigureAwait(false);

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
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}-{max}' Name, Count(1) Total From RequestInfo {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($"Select {i + 1} Id,'{min}以上' Name, Count(1) Total From RequestInfo {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By ID").ToString();

            Logger.LogTrace($"{nameof(GetGroupedResponeTimeStatisticsAsync)} SQL: {sql}");

            List<ResponeTimeGroup> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<ResponeTimeGroup>(sql).ConfigureAwait(false)).ToList();
            }, "获取http状态码分组统计异常").ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"
              Select AVG(Milliseconds) ART From RequestInfo {where};
              Select COUNT(1) Total From RequestInfo {where};
              Select COUNT(1) Code404 From RequestInfo {where} AND StatusCode = 404;
              Select COUNT(1) Code500 From RequestInfo {where} AND StatusCode = 500;
              Select Count(1) From ( Select Distinct Url From RequestInfo ) A;";

            Logger.LogTrace($"{nameof(GetIndexPageDataAsync)} SQL: {sql}");

            IndexPageData result = new IndexPageData();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql).ConfigureAwait(false))
                {
                    result.AvgResponseTime = resultReader.ReadFirstOrDefault<double>();
                    result.Total = resultReader.ReadFirstOrDefault<int>();
                    result.NotFound = resultReader.ReadFirstOrDefault<int>();
                    result.ServerError = resultReader.ReadFirstOrDefault<int>();
                    result.APICount = resultReader.ReadFirst<int>();
                    result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                }
            }, "获取首页数据异常").ConfigureAwait(false);

            return result;
        }

        #region Base

        protected async Task LoggingSqlOperation(Func<IDbConnection, Task> func, string message = null)
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
                Logger.LogError(ex, message ?? "数据库操作异常");
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
                CheckSqlWhere(builder).Append($"Node in ({string.Join(",", nodeFilterOption.Nodes.Select(m => $"'{m}'"))}) ");
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

        #endregion Base
    }
}