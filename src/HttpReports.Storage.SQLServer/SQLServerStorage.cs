using Dapper;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks; 

namespace HttpReports.Storage.SQLServer
{  
    internal class SQLServerStorage : IHttpReportsStorage
    {
        public SQLServerConnectionFactory ConnectionFactory { get; }

        public ILogger<SQLServerStorage> Logger { get; }

        public SQLServerStorage(SQLServerConnectionFactory connectionFactory, ILogger<SQLServerStorage> logger)
        {
            ConnectionFactory = connectionFactory;
            Logger = logger;
        }

        public async Task InitAsync()
        {
            using (var con = ConnectionFactory.GetConnection())
            {
                // 检查RequestInfo并创建
                if (con.QueryFirstOrDefault<int>($" Select Count(*) from sysobjects where id = object_id('{ConnectionFactory.DataBase}.dbo.RequestInfo') ") == 0)
                {
                   await con.ExecuteAsync(@"  
                        SET ANSI_NULLS ON; 
                        SET QUOTED_IDENTIFIER ON; 
                        CREATE TABLE [dbo].[RequestInfo](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Node] [nvarchar](50) NOT NULL,
	                        [Route] [nvarchar](50) NOT NULL,
	                        [Url] [nvarchar](200) NOT NULL,
	                        [Method] [nvarchar](50) NOT NULL,
	                        [Milliseconds] [int] NOT NULL,
	                        [StatusCode] [int] NOT NULL,
	                        [IP] [nvarchar](50) NOT NULL,
	                        [CreateTime] [datetime] NOT NULL,
                         CONSTRAINT [PK_RequestInfo] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]
                    ").ConfigureAwait(false); 
                }

                // 检查Job并创建
                if (con.QueryFirstOrDefault<int>($"Select Count(*) from sysobjects where id = object_id('{ConnectionFactory.DataBase}.dbo.Job')") == 0)
                {
                   await con.ExecuteAsync(@" 
                            SET ANSI_NULLS ON;
                            SET QUOTED_IDENTIFIER ON; 
                            CREATE TABLE [dbo].[Job](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [Title] [nvarchar](50) NOT NULL,
	                            [CronLike] [nvarchar](50) NOT NULL,
	                            [Emails] [nvarchar](500) NOT NULL,
                                [Mobiles] [nvarchar](500) NOT NULL,
	                            [Status] [int] NOT NULL,
	                            [Servers] [nvarchar](500) NOT NULL,
	                            [RtStatus] [int] NOT NULL,
	                            [RtTime] [int] NOT NULL,
	                            [RtRate] [decimal](18, 4) NOT NULL,
	                            [HttpStatus] [int] NOT NULL,
	                            [HttpCodes] [nvarchar](500) NOT NULL,
	                            [HttpRate] [decimal](18, 4) NOT NULL,
	                            [IPStatus] [int] NOT NULL,
	                            [IPWhiteList] [nvarchar](500) NOT NULL,
	                            [IPRate] [decimal](18, 4) NOT NULL,
	                            [CreateTime] [datetime] NULL,
                                [RequestStatus] [int] NOT NULL,
                                [RequestCount] [int] NOT NULL,
                             CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                      ").ConfigureAwait(false);
                } 

            }
        }

        public Task AddRequestInfoAsync(IRequestInfo request)
        {
            throw new NotImplementedException();
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
            string sql = $"Select TOP {filterOption.Take} Url,Avg(Milliseconds) Time FROM RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

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

            TraceLogSql(sql);

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

            TraceLogSql(sql);

            List<ResponeTimeGroup> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<ResponeTimeGroup>(sql).ConfigureAwait(false)).ToList();
            }, "获取http状态码分组统计异常").ConfigureAwait(false);

            return result;
        }



        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $"Select TOP {filterOption.Take} Url,COUNT(1) as Total From RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            List<UrlRequestCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<UrlRequestCount>(sql).ConfigureAwait(false)).ToList();
            }).ConfigureAwait(false);

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

        protected void TraceLogSql(string sql, [CallerMemberName]string method = null)
        {
            Logger.LogTrace($"Class: {nameof(SQLServerStorage)} Method: {method} SQL: {sql}");
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
                whereBuilder.Append($" AND IP = '{string.Join(",", filterOption.IPs.Select(m => $"'{m}'"))}' ");
            }

            if (filterOption.Urls?.Length > 0)
            {
                if (filterOption.Urls.Length > 1)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(SQLServer)}暂时只支持单条Url查询");
                }
                whereBuilder.Append($" AND  Url like '%{filterOption.Urls[0]}%' ");
            }

            var where = whereBuilder.ToString();

            sqlBuilder.Append(where);

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

                result.List.AddRange((await connection.GetListBySqlAsync<RequestInfo>(sql,"Id desc",filterOption.PageSize,filterOption.Take,result.AllItemCount).ConfigureAwait(false)).ToArray());
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

            string sql = $"Select {dateFormat} KeyField,COUNT(1) ValueField From RequestInfo {where} Group by {dateFormat};";

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
                    result.Items.Add(m.KeyField, m.ValueField);
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

            string sql = $"Select {dateFormat} KeyField,AVG(Milliseconds) ValueField From RequestInfo {where} Group by {dateFormat};";

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
                    result.Items.Add(m.KeyField, m.ValueField);
                });
            }, "获取响应时间统计异常").ConfigureAwait(false);

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
                case TimeUnit.Hour:
                    dateFormat = "DATENAME(HOUR, CreateTime)";
                    break;

                case TimeUnit.Month:
                    dateFormat = "CONVERT(varchar(7),CreateTime, 120)";
                    break;

                case TimeUnit.Year:
                    dateFormat = "";
                    break;

                case TimeUnit.Day:
                default:
                    dateFormat = "CONVERT(varchar(100),CreateTime, 23)";
                    break;
            }

            return dateFormat;
        }

        public Task<bool> AddMonitorRuleAsync(IMonitorRule rule)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMonitorRuleAsync(IMonitorRule rule)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMonitorRuleAsync(int ruleId)
        {
            throw new NotImplementedException();
        }

        public Task<IMonitorRule> GetMonitorRuleAsync(int ruleId)
        {
            throw new NotImplementedException();
        }

        public Task<List<IMonitorRule>> GetAllMonitorRulesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MonitorExecuteResult> ExecuteDataMonitorAsync(IMonitor monitor)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            throw new NotImplementedException();
        }
    }
}