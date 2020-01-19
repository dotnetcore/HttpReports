using Dapper;
using Dapper.Contrib.Extensions;
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

namespace HttpReports.Storage.Oracle
{  
    internal class OracleStorage : IHttpReportsStorage
    {
        public OracleConnectionFactory ConnectionFactory { get; }

        public ILogger<OracleStorage> Logger { get; }

        public OracleStorage(OracleConnectionFactory connectionFactory, ILogger<OracleStorage> logger)
        {
            ConnectionFactory = connectionFactory;
            Logger = logger;
        }

        public async Task InitAsync()
        {  
            using (var con = ConnectionFactory.GetConnection())
            {
                // 检查RequestInfo并创建
                if (con.QueryFirstOrDefault<int>($" Select count(*) from user_tables where table_name = upper('RequestInfo') ") == 0)
                {  
                    await con.ExecuteAsync(@"   

                        create table RequestInfo
                        (
	                        id NUMBER(15) primary key,
	                        Node varchar2(50),
	                        Route varchar2(50),
	                        Url varchar2(200),
	                        Method varchar2(50),
	                        Milliseconds NUMBER(15),
	                        StatusCode NUMBER(15),
	                        IP varchar2(50),
	                        CreateTime date
                        )

                     ").ConfigureAwait(false); 

                    await LoggingSqlOperation(async connection =>
                    {
                        await connection.ExecuteAsync(@"   

                        create sequence request_seq_id
                        increment by 1              
                        start with 1                
                        nomaxvalue                      
                        nocycle                         
                        nocache

                     ").ConfigureAwait(false); 

                    }, "").ConfigureAwait(false);    

                }
            }  
         
        }

        public async Task AddRequestInfoAsync(IRequestInfo request)
        {
            //TODO 实现批量存储
            await LoggingSqlOperation(async connection =>
            {
                //await connection.InsertAsync(request as RequestInfo).ConfigureAwait(false);

                string sql = $@"Insert Into RequestInfo Values (request_seq_id.nextval,'
                    {request.Node}','{request.Route}','{request.Url}','{request.Method}',{request.Milliseconds},{request.StatusCode},'{request.IP}', 
                 to_date('{request.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss'))"; 

                await connection.ExecuteAsync(sql).ConfigureAwait(false); 

            }, "请求数据保存失败").ConfigureAwait(false);
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
                nodeNames = (await connection.QueryAsync<string>("Select Distinct Node FROM RequestInfo").ConfigureAwait(false)).ToArray();
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
            string sql = $"Select  Url,Round(Avg(Milliseconds),2) Time FROM RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            sql = BuildTopSql(sql,filterOption.Take);

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
            string sql = $@"Select Url,COUNT(1)  Total From RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)}";

            sql = BuildTopSql(sql,filterOption.Take);

            TraceLogSql(sql);

            List<UrlRequestCount> result = null;
            await LoggingSqlOperation(async connection =>
            {
                result = (await connection.QueryAsync<UrlRequestCount>(sql).ConfigureAwait(false)).ToList();
            }).ConfigureAwait(false);

            return result;
        } 

         
        protected string BuildTopSql(string sql,int Count)
        {
            return " Select * FRom ( " + sql + " ) Where RowNum <= 10  ";  
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

            string sql = $@"
                Select 'Total'    KeyField, COUNT(1) ValueField From RequestInfo {where} Union
                Select 'Code404'  KeyField, COUNT(1) ValueField From RequestInfo {where} AND StatusCode = 404 Union
                Select 'Code500'  KeyField, COUNT(1) ValueField From RequestInfo {where} AND StatusCode = 500 Union
                Select 'APICount' KeyField, Count(1) ValueField  From ( Select Distinct Url From RequestInfo ) A Union
                Select 'ART'      KeyField, Round(AVG(Milliseconds),2) ValueField From RequestInfo {where} ";

            TraceLogSql(sql);

            IndexPageData result = new IndexPageData(); 

            await LoggingSqlOperation(async connection =>
            {
                var data = await connection.QueryAsync<KVClass<string,string>>(sql).ConfigureAwait(false);

                result.Total = data.Where(x => x.KeyField == "Total").FirstOrDefault().ValueField.ToInt();
                result.NotFound = data.Where(x => x.KeyField == "Code404").FirstOrDefault().ValueField.ToInt();
                result.ServerError = data.Where(x => x.KeyField == "Code500").FirstOrDefault().ValueField.ToInt();
                result.APICount = data.Where(x => x.KeyField == "APICount").FirstOrDefault().ValueField.ToInt();
                result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                result.AvgResponseTime = data.Where(x => x.KeyField == "ART").FirstOrDefault().ValueField.ToDouble(); 
               
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
            Logger.LogTrace($"Class: {nameof(OracleStorage)} Method: {method} SQL: {sql}");
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
                    CheckSqlWhere(builder).Append($"CreateTime >= to_date('{timeSpanFilterOption.StartTime.Value.ToString(timeSpanFilterOption.StartTimeFormat)}','YYYY-MM-DD') ");
                }
                if (timeSpanFilterOption.EndTime.HasValue)
                {
                    CheckSqlWhere(builder).Append($"CreateTime < to_date('{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}','YYYY-MM-DD') ");
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
        //public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        //{
        //    var whereBuilder = new StringBuilder(BuildSqlFilter(filterOption), 512);

        //    var sqlBuilder = new StringBuilder("Select * From RequestInfo ", 512);

        //    if (whereBuilder.Length == 0)
        //    {
        //        whereBuilder.Append("Where 1=1 ");
        //    }

        //    if (filterOption.IPs?.Length > 0)
        //    {
        //        whereBuilder.Append($" AND IP = '{string.Join(",", filterOption.IPs.Select(m => $"'{m}'"))}' ");
        //    }

        //    if (filterOption.Urls?.Length > 0)
        //    {
        //        if (filterOption.Urls.Length > 1)
        //        {
        //            throw new ArgumentOutOfRangeException($"{nameof(SQLServer)}暂时只支持单条Url查询");
        //        }
        //        whereBuilder.Append($" AND  Url like '%{filterOption.Urls[0]}%' ");
        //    }

        //    var where = whereBuilder.ToString();

        //    sqlBuilder.Append(where);

        //    var sql = sqlBuilder.ToString();

        //    TraceLogSql(sql);

        //    var countSql = "Select count(1) From RequestInfo " + where;
        //    TraceLogSql(countSql);

        //    var result = new RequestInfoSearchResult()
        //    {
        //        SearchOption = filterOption,
        //    };

        //    await LoggingSqlOperation(async connection =>
        //    { 
        //        result.AllItemCount = connection.QueryFirstOrDefault<int>(countSql);

        //        result.List.AddRange((await connection.GetListBySqlAsync<RequestInfo>(sql,"Id desc",filterOption.PageSize,filterOption.Take,result.AllItemCount).ConfigureAwait(false)).ToArray());
        //    }, "查询请求信息列表异常").ConfigureAwait(false);

        //    return result;
        //}

        /// <summary>
        /// 获取请求次数统计
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $"Select {dateFormat} KeyField,COUNT(1) ValueField From RequestInfo {where} Group by {dateFormat} ";

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

            string sql = $"Select {dateFormat} KeyField,Round(AVG(Milliseconds),2) ValueField From RequestInfo {where} Group by {dateFormat}";

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

        public static string GetListSql(string sql,string order,int page,int size)
        {
            return $@" Select * from ( select ROWNUM as num,PageA.* from ( {sql} Order By {order}  ) PageA where rownum <= {page*size} ) where num >= {(page-1) * size} ";
        } 

        private static string GetDateFormat(TimeSpanStatisticsFilterOption filterOption)
        {
            string dateFormat;
            switch (filterOption.Type)
            {
                case TimeUnit.Hour:
                    dateFormat = "to_char(CreateTime,'hh24')";
                    break;

                case TimeUnit.Month:
                    dateFormat = "to_char(sysdate,'mm')";
                    break;

                case TimeUnit.Year:
                    dateFormat = "";
                    break;

                case TimeUnit.Day:
                default:
                    dateFormat = "to_char(CreateTime,'dd')";
                    break;
            }

            return dateFormat;
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
                    throw new ArgumentOutOfRangeException($"{nameof(OracleStorage)}暂时只支持单条Url查询");
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

                var listSql = GetListSql(sql, "Id Desc",filterOption.Page,filterOption.PageSize);

                result.List.AddRange((await connection.QueryAsync<RequestInfo>(listSql).ConfigureAwait(false)).ToArray());
            }, "查询请求信息列表异常").ConfigureAwait(false);

            return result;
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