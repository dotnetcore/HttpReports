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
using HttpReports.Core.Interface;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
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

        private string TablePrefix { get; set; } = string.Empty;

        public ILogger<MySqlStorage> Logger { get; }

        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _deferFlushCollection = null;

        public MySqlStorage(IOptions<MySqlStorageOptions> options, MySqlConnectionFactory connectionFactory, ILogger<MySqlStorage> logger)
        {
            Options = options.Value;
            if (!Options.TablePrefix.IsEmpty()) TablePrefix = Options.TablePrefix + ".";
            ConnectionFactory = connectionFactory;
            Logger = logger;
            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferSecond);
            }
        }

        #region Init

        public async Task InitAsync()
        {
            try
            { 
                using (var connection = ConnectionFactory.GetConnection())
                {
                    await connection.ExecuteAsync($@"CREATE TABLE IF NOT EXISTS `{TablePrefix}RequestInfo` (
  `Id` varchar(50) NOT NULL,
  `ParentId` varchar(50) NOT NULL,
  `Node` varchar(50) DEFAULT NULL,
  `Route` varchar(200) DEFAULT NULL,
  `Url` varchar(255) DEFAULT NULL,
  `RequestType` varchar(50) DEFAULT NULL,
  `Method` varchar(16) DEFAULT NULL,
  `Milliseconds` int(11) DEFAULT NULL,
  `StatusCode` int(11) DEFAULT NULL,
  `IP` varchar(50) DEFAULT NULL,
  `Port` int(11) DEFAULT NULL, 
  `LocalIP` varchar(50) DEFAULT NULL,
  `LocalPort` int(11) DEFAULT NULL, 
  `CreateTime` datetime(3) DEFAULT NULL,
  PRIMARY KEY (`Id`) 
) ENGINE=InnoDB DEFAULT CHARSET=utf8;");

                    await connection.ExecuteAsync($@"CREATE TABLE IF NOT EXISTS `{TablePrefix}RequestDetail` (
  `Id` varchar(50) NOT NULL,
  `RequestId` varchar(50) DEFAULT NULL,
  `Scheme` varchar(10) DEFAULT NULL,
  `QueryString` varchar(10000) DEFAULT NULL,
  `Header` text DEFAULT NULL,
  `Cookie` text DEFAULT NULL,
  `RequestBody` text DEFAULT NULL,
  `ResponseBody`text DEFAULT NULL,
  `ErrorMessage` text DEFAULT NULL,
  `ErrorStack` text DEFAULT NULL,
  `CreateTime` datetime(3) DEFAULT NULL ,
  PRIMARY KEY (`Id`) 
) ENGINE=InnoDB DEFAULT CHARSET=utf8;");


                if (await connection.QueryFirstOrDefaultAsync<int>($"select  COUNT(1) from information_schema.columns WHERE  TABLE_SCHEMA = '{ConnectionFactory.DataBase}' and table_name = '{TablePrefix}MonitorJob' and column_name = 'Nodes' ") > 0)
                {
                    await connection.ExecuteAsync($"DROP TABLE `{TablePrefix}MonitorJob` ");
                }

                        await connection.ExecuteAsync($@"
CREATE TABLE IF NOT EXISTS `{TablePrefix}MonitorJob` (
  `Id` varchar(50) NOT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CronLike` varchar(255) DEFAULT NULL,
  `Emails` varchar(1000) DEFAULT NULL,
  `WebHook` varchar(1000) DEFAULT NULL,
  `Mobiles` varchar(1000) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `Service` varchar(255) DEFAULT NULL,
  `Instance` varchar(255) DEFAULT NULL,
  `PayLoad` varchar(2000) DEFAULT NULL,
  `CreateTime` datetime(3) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;");

                    await connection.ExecuteAsync($@"
CREATE TABLE IF NOT EXISTS `{TablePrefix}SysUser` (
  `Id` varchar(50) NOT NULL,
  `UserName` varchar(255) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL, 
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;");


                    await connection.ExecuteAsync($@"
CREATE TABLE IF NOT EXISTS `{TablePrefix}SysConfig` (
  `Id` varchar(50) NOT NULL,
  `Key` varchar(255) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL, 
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;");

                    await connection.ExecuteAsync($@"
CREATE TABLE IF NOT EXISTS `{TablePrefix}Performance` (
  `Id` varchar(50) NOT NULL,
  `Service` varchar(255) DEFAULT NULL,
  `Instance` varchar(255) DEFAULT NULL,
  `GCGen0` int DEFAULT NULL,
  `GCGen1` int DEFAULT NULL,
  `GCGen2` int DEFAULT NULL,
  `HeapMemory` double DEFAULT NULL,
  `ProcessCPU` double DEFAULT NULL,
  `ProcessMemory` double DEFAULT NULL,
  `ThreadCount` int DEFAULT NULL,
  `PendingThreadCount` int DEFAULT NULL,
  `CreateTime` datetime(3) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;"); 


                    if (await connection.QueryFirstOrDefaultAsync<int>($"Select count(1) from `{TablePrefix}SysUser`") == 0)
                    {
                        await connection.ExecuteAsync($@" Insert Into `{TablePrefix}SysUser` (`Id`,`UserName`,`Password`) Values ('{MD5_16(Guid.NewGuid().ToString())}','{Core.Config.BasicConfig.DefaultUserName}','{Core.Config.BasicConfig.DefaultPassword}') ");
                    }

                    var lang = await connection.QueryFirstOrDefaultAsync<string>($"Select * from `{TablePrefix}SysConfig` Where `Key` =  '{BasicConfig.Language}' ");
                     
                    if (!lang.IsEmpty())
                    {
                        if (lang.ToLowerInvariant() == "chinese" || lang.ToLowerInvariant() == "english")
                        {
                            await connection.ExecuteAsync($@" Delete From `{TablePrefix}SysConfig` Where `Key` =  '{BasicConfig.Language}'  ");

                            await connection.ExecuteAsync($@" Insert Into `{TablePrefix}SysConfig` Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");

                        }  
                    }
                    else
                    {
                        await connection.ExecuteAsync($@" Insert Into `{TablePrefix}SysConfig` Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");
                    } 

                    if (await connection.QueryFirstOrDefaultAsync<string>($" SELECT COLUMN_TYPE FROM INFORMATION_SCHEMA.COLUMNS where table_schema ='{ConnectionFactory.DataBase}' AND table_name  = '{TablePrefix}RequestInfo'  AND COLUMN_NAME = 'Route'") != "varchar(120)")
                    {
                        await connection.ExecuteAsync($" Alter Table `{TablePrefix}RequestInfo` modify column `Route` varchar(120)");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Init Error:{ex.Message}");

                throw ex;
            }
        } 

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

        public async Task AddRequestInfoAsync(List<RequestBag> list, CancellationToken token)
        {
            await LoggingSqlOperation(async connection =>
            { 
                List<IRequestInfo> requestInfos = list.Select(x => x.RequestInfo).ToList();

                List<IRequestDetail> requestDetails = list.Select(x => x.RequestDetail).ToList(); 

                if (requestInfos.Where(x => x != null).Any())
                {

                    string requestSql = string.Join(",", requestInfos.Select(request => {

                        int i = requestInfos.IndexOf(request) + 1;

                        return $"(@Id{i}, @ParentId{i}, @Node{i}, @Route{i}, @Url{i}, @RequestType{i}, @Method{i}, @Milliseconds{i}, @StatusCode{i}, @IP{i}, @Port{i}, @LocalIP{i}, @LocalPort{i}, @CreateTime{i})";

                    }));

                    await connection.ExecuteAsync($"INSERT INTO `{TablePrefix}RequestInfo` (`Id`,`ParentId`,`Node`, `Route`, `Url`,`RequestType`,`Method`, `Milliseconds`, `StatusCode`, `IP`,`Port`,`LocalIP`,`LocalPort`,`CreateTime`) VALUES {requestSql}", BuildParameters(requestInfos));
                     
                } 

                if (requestDetails.Where(x => x != null).Any())
                {
                    string detailSql = string.Join(",", requestDetails.Select(detail =>
                    { 
                        int i = requestDetails.IndexOf(detail) + 1;

                        return $"(@Id{i},@RequestId{i},@Scheme{i},@QueryString{i},@Header{i},@Cookie{i},@RequestBody{i},@ResponseBody{i},@ErrorMessage{i},@ErrorStack{i},@CreateTime{i})";

                    }));

                    await connection.ExecuteAsync($"Insert into `{TablePrefix}RequestDetail` (`Id`,`RequestId`,`Scheme`,`QueryString`,`Header`,`Cookie`,`RequestBody`,`ResponseBody`,`ErrorMessage`,`ErrorStack`,`CreateTime`) VALUES {detailSql}", BuildParameters(requestDetails));

                }

            }, "请求数据批量保存失败");
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


        public async Task AddRequestInfoAsync(RequestBag bag)
        {
            if (Options.EnableDefer)
            {
                _deferFlushCollection.Flush(bag);
            }
            else
            {
                await LoggingSqlOperation(async connection =>
                {
                    await connection.ExecuteAsync($"INSERT INTO `{TablePrefix}RequestInfo` (`Id`,`ParentId`,`Node`, `Route`, `Url`, `RequestType`, `Method`, `Milliseconds`, `StatusCode`, `IP`,`Port`,`LocalIP`,`LocalPort`,`CreateTime`) VALUES (@Id,@ParentId, @Node, @Route, @Url,@RequestType, @Method, @Milliseconds, @StatusCode, @IP,@Port,@LocalIP,@LocalPort, @CreateTime)",bag.RequestInfo);

                    await connection.ExecuteAsync($"INSERT INTO `{TablePrefix}RequestDetail` (`Id`,`RequestId`,`Scheme`,`QueryString`,`Header`,`Cookie`,`RequestBody`,`ResponseBody`,`ErrorMessage`,`ErrorStack`,`CreateTime`)  VALUES (@Id,@RequestId,@Scheme,@QueryString,@Header,@Cookie,@RequestBody,@ResponseBody,@ErrorMessage,@ErrorStack,@CreateTime)", bag.RequestDetail);

                }, "请求数据保存失败");
            }
        } 

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
     
        public async Task<IndexPageData> GetIndexBasicDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"
        Select COUNT(1) Total From {TablePrefix}RequestInfo {where}; 
        Select COUNT(1) Code500 From {TablePrefix}RequestInfo {where} AND StatusCode = 500;
        SELECT Count(DISTINCT(Node)) From {TablePrefix}RequestInfo {where};  
        Select Count(1) from ( SELECT LocalIP,LocalPort from {TablePrefix}RequestInfo {where} GROUP BY LocalIP,LocalPort) Z;";

            TraceLogSql(sql);

            IndexPageData result = new IndexPageData();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql))
                {
                    result.Total = resultReader.ReadFirstOrDefault<int>(); 
                    result.ServerError = resultReader.ReadFirstOrDefault<int>();
                    result.Service = resultReader.ReadFirstOrDefault<int>();
                    result.Instance = resultReader.ReadFirstOrDefault<int>();
                }
            }, "获取首页数据异常");

            return result;
        }



        public async Task<List<List<TopServiceResponse>>> GetGroupData(IndexPageDataFilterOption filterOption, GroupType group)
        {
            string groupName = default;

            if (group == GroupType.Node) groupName = "Node";
            if (group == GroupType.Route) groupName = "Route";
            if (group == GroupType.Instance) groupName = "LocalIP,LocalPort";

            string where = BuildSqlFilter(filterOption);

            string sql = $@"

                Select {groupName},COUNT(1) From {TablePrefix}RequestInfo {where} Group by {groupName}  ORDER BY COUNT(1) Desc Limit {filterOption.Take} ;
                Select {groupName},AVG(Milliseconds) From {TablePrefix}RequestInfo {where} Group by {groupName}  ORDER BY  Avg(Milliseconds) Desc Limit {filterOption.Take} ; 
                Select {groupName},COUNT(1) From {TablePrefix}RequestInfo {where} AND StatusCode = 500 Group by {groupName}  ORDER BY COUNT(1) Desc Limit {filterOption.Take} ;  ";

            TraceLogSql(sql);

            List<List<TopServiceResponse>> result = new List<List<TopServiceResponse>>();

            await LoggingSqlOperation(async connection =>
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql))
                { 
                    if (group == GroupType.Instance)
                    {
                        result.Add(resultReader.Read<(string localIP, string localPort, double value)>().Select(x => new TopServiceResponse { Service = x.localIP + ":" + x.localPort, Value = x.value.ToInt() }).ToList());
                        result.Add(resultReader.Read<(string localIP, string localPort, double value)>().Select(x => new TopServiceResponse { Service = x.localIP + ":" + x.localPort, Value = x.value.ToInt() }).ToList());
                        result.Add(resultReader.Read<(string localIP, string localPort, double value)>().Select(x => new TopServiceResponse { Service = x.localIP + ":" + x.localPort, Value = x.value.ToInt() }).ToList());
                    }
                    else
                    {
                        result.Add(resultReader.Read<(string service, double value)>().Select(x => new TopServiceResponse { Service = x.service, Value = x.value.ToInt() }).ToList());
                        result.Add(resultReader.Read<(string service, double value)>().Select(x => new TopServiceResponse { Service = x.service, Value = x.value.ToInt() }).ToList());
                        result.Add(resultReader.Read<(string service, double value)>().Select(x => new TopServiceResponse { Service = x.service, Value = x.value.ToInt() }).ToList());
                    }

                }
            }, "获取服务数据异常");

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

            var sqlBuilder = new StringBuilder($"Select * From {TablePrefix}RequestInfo ", 512);

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
                whereBuilder.Append($" AND Id IN ({string.Join(",",RequestIdCollection.Select(x => "'" + x + "'" ))}) ");
            } 

            var where = whereBuilder.ToString();

            sqlBuilder.Append(where);
            sqlBuilder.Append(BuildSqlControl(filterOption));

            var sql = sqlBuilder.ToString();

            TraceLogSql(sql);

            var countSql = $"Select count(1) From {TablePrefix}RequestInfo " + where;
            TraceLogSql(countSql);

            var result = new RequestInfoSearchResult()
            {
                SearchOption = filterOption,
            };

            await LoggingSqlOperation(async connection =>
            {
                result.AllItemCount = connection.QueryFirstOrDefault<int>(countSql);

                result.List.AddRange((await connection.QueryAsync<RequestInfo>(sql)).ToArray());
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

        public async Task<bool> AddMonitorJob(IMonitorJob job)
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

        public async Task<bool> AddPerformanceAsync(IPerformance performance)
        {
            performance.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into {TablePrefix}Performance 
            (Id,Service,Instance,GCGen0,GCGen1,GCGen2,HeapMemory,ProcessCPU,ProcessMemory,ThreadCount,PendingThreadCount,CreateTime)
             Values (@Id,@Service,@Instance,@GCGen0,@GCGen1,@GCGen2,@HeapMemory,@ProcessCPU,@ProcessMemory,@ThreadCount,@PendingThreadCount,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, performance)

            ) > 0); 

        }  
        

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            string sql = $@"Update {TablePrefix}MonitorJob 

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,WebHook = @WebHook,Mobiles = @Mobiles,Status= @Status,Service = @Service,Instance = @Instance,PayLoad = @PayLoad 

                Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);
        }

        public async Task<IMonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From {TablePrefix}MonitorJob Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql, new { Id })

            ));
        }

        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            string sql = $@"Select * From {TablePrefix}MonitorJob ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.QueryAsync<MonitorJob>(sql)

            ).ToList().Select(x => x as IMonitorJob).ToList());
        }

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From {TablePrefix}MonitorJob Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            (await connection.ExecuteAsync(sql, new { Id })) > 0);
        }

        public async Task<SysUser> CheckLogin(string Username, string Password)
        {
            string sql = $" Select * From {TablePrefix}SysUser Where UserName = @UserName AND Password = @Password ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { Username, Password })

            ));

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

        public async Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            string sql = $" Select * From {TablePrefix}RequestInfo Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            string detailSql = $" Select * From {TablePrefix}RequestDetail Where RequestId = @Id";

            TraceLogSql(detailSql);

            var requestDetail = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestDetail>(detailSql, new { Id })

           ));

            return (requestInfo, requestDetail);
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public async Task<IRequestInfo> GetRequestInfo(string Id)
        {
            string sql = $" Select * From {TablePrefix}RequestInfo Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            return requestInfo;
        }

        public async Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            string sql = $"Select * From {TablePrefix}RequestInfo Where ParentId = @ParentId Order By CreateTime ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryAsync<RequestInfo>(sql, new { ParentId })

           ));

            return requestInfo.Select(x => x as IRequestInfo).ToList();
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

        public async Task SetLanguage(string Language)
        {
            string sql = $"Update `{TablePrefix}SysConfig` Set `Value` = @Language Where `Key` = '{BasicConfig.Language}' ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.ExecuteAsync(sql, new { Language })

           ));
        }

        public async Task<string> GetSysConfig(string Key)
        {
            string sql = $"Select `Value` From `{TablePrefix}SysConfig` Where `Key` = @Key ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryFirstOrDefaultAsync<string>(sql, new { Key })

           ));

            return result;
        }

        public async Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        { 
            string sql = $"Select `Node`,`LocalIP`,`LocalPort` From {TablePrefix}RequestInfo where CreateTime >= @CreateTime GROUP BY `Node`,`LocalIP`,`LocalPort`  ORDER BY `LocalIP`,`LocalPort`  ";

            TraceLogSql(sql);  

            var result = await LoggingSqlOperation(async connection => (  await connection.QueryAsync<ServiceInstanceInfoModel>(sql, new { CreateTime = startTime }) ));

            return result.Select(x => new ServiceInstanceInfo {
            
                Service = x.Node,
                IP = x.LocalIP,
                Port = x.LocalPort
            
            }).ToList();
        }

        public async Task<List<IPerformance>> GetPerformances(PerformanceFilterIOption option)
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

            return result.Select(x => x as IPerformance).ToList();

        }


        public async Task<IEnumerable<string>> GetTopServiceLoad(IndexPageDataFilterOption filterOption)
        {
            string sql = $"Select Node  From {TablePrefix}RequestInfo {BuildSqlFilter(filterOption, false, true)} Group by Node  ORDER BY COUNT(1)  Desc Limit {filterOption.Take}  ";

            return await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

        } 
      

        public async Task<List<BaseTimeModel>> GetServiceTrend(IndexPageDataFilterOption filterOption,List<string> range)
        {
            IEnumerable<string> service = new List<string>() { filterOption.Service };

            if (filterOption.Service.IsEmpty()) 
            {
                service = await GetTopServiceLoad(filterOption);
            } 

            var timeSpan = new TimeSpanStatisticsFilterOption
            { 
                Type = (filterOption.EndTime.Value - filterOption.StartTime.Value).TotalHours > 1 ? TimeUnit.Hour : TimeUnit.Minute,

            };

            var DateFormat = GetDateFormat(timeSpan);

            string where = " where  CreateTime >= @Start AND CreateTime < @End  ";

            if (service.Any())
            {
                if (service.Count() == 1)
                {
                    where = where + $" AND Node = '{service.FirstOrDefault()}' ";
                }
                else
                {
                    where = where + $" AND Node In  @NodeList ";
                }  
            } 

            if (!filterOption.LocalIP.IsEmpty()) where = where + $" AND LocalIP = '{filterOption.LocalIP}' ";
            if (filterOption.LocalPort > 0) where = where + $" AND LocalPort = {filterOption.LocalPort} ";

            string sql = $@"SELECT Node KeyField, {DateFormat} TimeField,COUNT(1) ValueField From RequestInfo {where} GROUP BY Node,{DateFormat} "; 

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<BaseTimeModel>(sql,new {
            
                Start =  filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat),
                End = filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat),
                NodeList = service.ToArray()

            }));

            var model = new List<BaseTimeModel>();

            foreach (var s in service)
            {
                foreach (var r in range)
                {
                    var c = list.Where(x =>  x.KeyField == s && x.TimeField == r).FirstOrDefault(); 

                    model.Add(new BaseTimeModel
                    { 
                        KeyField = s, 
                        TimeField = r,
                        ValueField = c == null ? 0 : c.ValueField

                    });

                } 
            } 

            return model; 
        }
         

        public async Task<List<BaseTimeModel>> GetServiceHeatMap(IndexPageDataFilterOption filterOption, List<string> Time,List<string> Span)
        { 
            string where = " where  CreateTime >= @Start AND CreateTime < @End ";

            if (!filterOption.Service.IsEmpty()) where = where + $" AND Node = '{filterOption.Service}' ";
            if (!filterOption.LocalIP.IsEmpty()) where = where + $" AND LocalIP = '{filterOption.LocalIP}' ";
            if (filterOption.LocalPort > 0) where = where + $" AND LocalPort = {filterOption.LocalPort} ";

            var timeSpan = new TimeSpanStatisticsFilterOption
            {
                Type = (filterOption.EndTime.Value - filterOption.StartTime.Value).TotalHours > 1 ? TimeUnit.Hour : TimeUnit.Minute

            };

            var DateFormat = GetDateFormat(timeSpan);

            string sql = $@"

                  select {DateFormat} TimeField,  
                  case 
                  when (0 < Milliseconds and Milliseconds <= 200  ) then '0-200'
                  when (200 < Milliseconds and Milliseconds <= 400) then '200-400'
                  when (400 < Milliseconds and Milliseconds <= 600) then '400-600'
                  when (600 < Milliseconds and Milliseconds <= 800) then '600-800'
                  when (800 < Milliseconds and Milliseconds <= 1000) then '800-1000'
                  when (1000 < Milliseconds and Milliseconds <= 1200) then '1000-1200'
                  when (1200 < Milliseconds and Milliseconds <= 1400) then '1200-1400'
                  when (1400 < Milliseconds and Milliseconds <= 1600) then '1400-1600'
                  else '1600+' end KeyField, count(1) ValueField 
                  From requestinfo {where} GROUP BY KeyField,TimeField  "; 

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<BaseTimeModel>(sql,new {

                Start = filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat),
                End = filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat),
                Node = filterOption.Service


            }));

            var model = new List<BaseTimeModel>();

            foreach (var t in Time)
            {
                foreach (var s in Span)
                {
                    var c = list.Where(x => x.TimeField == t && x.KeyField == s).FirstOrDefault(); 

                    model.Add(new BaseTimeModel { 
                     
                        TimeField = t,
                        KeyField = s,
                        ValueField = c == null ? 0 : c.ValueField

                    });
                } 
            } 
             
            return model; 
        } 


        #endregion Base
    }
}