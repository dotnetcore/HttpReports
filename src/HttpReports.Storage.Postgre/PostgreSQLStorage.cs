using Dapper;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Storage.PostgreSQL
{
    public class PostgreSQLStorage : IHttpReportsStorage
    { 
        public PostgreStorageOptions Options { get; }

        public PostgreConnectionFactory ConnectionFactory { get; }

        public ILogger<PostgreSQLStorage> Logger { get; }

        private readonly AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail> _deferFlushCollection = null;

        public PostgreSQLStorage(IOptions<PostgreStorageOptions> options, PostgreConnectionFactory connectionFactory, ILogger<PostgreSQLStorage> logger)
        {
            Options = options.Value;
            ConnectionFactory = connectionFactory;
            Logger = logger;
            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferSecond);
            }
        }

        private async Task AddRequestInfoAsync(Dictionary<IRequestInfo, IRequestDetail> list, System.Threading.CancellationToken token)
        {
            await LoggingSqlOperation(async connection =>
            { 
                List<IRequestInfo> requestInfos = list.Select(x => x.Key).ToList();

                List<IRequestDetail> requestDetails = list.Select(x => x.Value).ToList();

                var request = string.Join(",", requestInfos.Select(item => {

                    int i = requestInfos.IndexOf(item) + 1;

                    return $"(@Id{i},@ParentId{i},@Node{i}, @Route{i}, @Url{i},@RequestType{i}, @Method{i}, @Milliseconds{i}, @StatusCode{i}, @IP{i},@Port{i},@LocalIP{i},@LocalPort{i},@CreateTime{i})";
               
                }));

                await connection.ExecuteAsync($@"INSERT INTO ""RequestInfo"" (Id,ParentId,Node, Route, Url,RequestType,Method, Milliseconds, StatusCode, IP,Port,LocalIP,LocalPort,CreateTime) VALUES {request}",BuildParameters(requestInfos));

                string detail = string.Join(",", requestDetails.Select(item => { 

                    int i = requestDetails.IndexOf(item) + 1;

                    return $"(@Id{i},@RequestId{i},@Scheme{i},@QueryString{i},@Header{i},@Cookie{i},@RequestBody{i},@ResponseBody{i},@ErrorMessage{i},@ErrorStack{i},@CreateTime{i}) ";


                }));

                await connection.ExecuteAsync($@"Insert into ""RequestDetail"" (Id,RequestId,Scheme,QueryString,Header,Cookie,RequestBody,ResponseBody,ErrorMessage,ErrorStack,CreateTime) VALUES {detail}",BuildParameters(requestDetails));
 

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




        public async Task<bool> AddMonitorJob(IMonitorJob job)
        {
            job.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into ""MonitorJob"" 
            (Id,Title,Description,CronLike,Emails,WebHook,Mobiles,Status,Nodes,PayLoad,CreateTime)
             Values (@Id,@Title,@Description,@CronLike,@Emails,@WebHook, @Mobiles,@Status,@Nodes,@PayLoad,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);

        }

        public async Task AddRequestInfoAsync(IRequestInfo request, IRequestDetail detail)
        {
            if (Options.EnableDefer)
            {
                _deferFlushCollection.Push(request,detail);
            }
            else
            {
                await LoggingSqlOperation(async connection =>
                { 
                    await connection.ExecuteAsync(@"INSERT INTO ""RequestInfo"" (Id,ParentId,Node, Route,Url,RequestType, Method, Milliseconds, StatusCode, IP,Port,LocalIP,LocalPort,CreateTime) VALUES (@Id,@ParentId,@Node, @Route, @Url,@RequestType, @Method, @Milliseconds, @StatusCode, @IP,@Port,@LocalIP,@LocalPort,@CreateTime)", request);

                    await connection.ExecuteAsync(@"INSERT INTO ""RequestDetail"" (Id,RequestId,Scheme,QueryString,Header,Cookie,RequestBody,ResponseBody,ErrorMessage,ErrorStack,CreateTime)  VALUES (@Id,@RequestId,@Scheme,@QueryString,@Header,@Cookie,@RequestBody,@ResponseBody,@ErrorMessage,@ErrorStack,@CreateTime)",detail);
                     
                }, "请求数据保存失败");
            }
        }

        public async Task<SysUser> CheckLogin(string Username, string Password)
        {
            string sql = $@" Select * From ""SysUser"" Where UserName = @UserName AND Password = @Password ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { Username, Password })

            ));

        }

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From ""MonitorJob"" Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            (await connection.ExecuteAsync(sql,new { Id })) > 0);
        }


        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"

Select COUNT(1) AS Total From ""RequestInfo"" {where};
Select COUNT(1) AS Code404 From ""RequestInfo"" {where} AND StatusCode = 404;
Select COUNT(1) AS Code500 From ""RequestInfo"" {where} AND StatusCode = 500;
Select Count(1)  From ( Select Distinct Url From ""RequestInfo"" ) A;
Select AVG(Milliseconds) AS ART From ""RequestInfo"" {where};";

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
         

        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            string sql = $@"Select * From ""MonitorJob"" ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.QueryAsync<MonitorJob>(sql)

            ).ToList().Select(x => x as IMonitorJob).ToList());
        }

        public async Task<List<NodeInfo>> GetNodesAsync() =>
            await LoggingSqlOperation(async connection => (await connection.QueryAsync<string>(@"Select Distinct Node FROM ""RequestInfo"" ")
            ).Select(m => new NodeInfo { Name = m }).ToList(), "获取所有节点信息失败");

        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $@"Select Url,Avg(Milliseconds) AS Time FROM ""RequestInfo"" {BuildSqlFilter(filterOption)} Group By Url order by Time {BuildSqlControl(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<RequestAvgResponeTime>(sql)).ToList(), "获取Url的平均请求处理时间统计异常");
        }

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $@"SELECT COUNT(1) FROM ""RequestInfo"" {BuildSqlFilter(filterOption)}";

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

            var sql = $@"SELECT COUNT(1) AS TOTAL FROM ""RequestInfo"" {BuildSqlFilter(filterOption)} AND {ipFilter} GROUP BY IP ORDER BY TOTAL DESC LIMIT 1";
            TraceLogSql(sql);
            var max = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            sql = $@"SELECT COUNT(1) AS TOTAL FROM ""RequestInfo"" {BuildSqlFilter(filterOption)} AND {ipFilter}";
            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
            return (max, all);
        }

        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption filterOption)
        {
            var where = BuildSqlFilter(filterOption);

            var dateFormat = GetDateFormat(filterOption);

            string sql = $@"Select {dateFormat} AS KeyField,COUNT(1) AS ValueField From ""RequestInfo"" {where} Group by KeyField;";

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

            string sql = $@"Select {dateFormat} AS KeyField,AVG(Milliseconds) AS ValueField From ""RequestInfo"" {where} Group by KeyField;";

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

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $@"Select '{m}' AS Code,COUNT(1) AS Total From ""RequestInfo"" {where} AND StatusCode = {m}"));

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
                    sqlBuilder.Append($@"Select {i + 1} AS Id,'{min}-{max}' AS Name, Count(1) AS Total From ""RequestInfo"" {where} AND Milliseconds >= {min} AND Milliseconds < {max} union ");
                }
                else
                {
                    sqlBuilder.Append($@"Select {i + 1} AS Id,'{min}以上' AS Name, Count(1) AS Total From ""RequestInfo"" {where} AND Milliseconds >= {min} union ");
                }
            }

            var sql = sqlBuilder.Remove(sqlBuilder.Length - 6, 6).Append(")T Order By ID").ToString();

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<ResponeTimeGroup>(sql)).ToList(), "获取http状态码分组统计异常");
        }



        public async Task<SysUser> GetSysUser(string UserName)
        {
            string sql = $@" Select * From ""SysUser"" Where UserName = @UserName";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { UserName })

            ));
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $@"SELECT COUNT(1) FROM  ""RequestInfo"" {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $@"Select Url,COUNT(1) as Total From ""RequestInfo"" {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)};";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (await connection.QueryAsync<UrlRequestCount>(sql)).ToList());
        }

        public async Task InitAsync()
        { 
            try
            {
                using (var con = ConnectionFactory.GetConnection())
                {
                    if (await con.QueryFirstOrDefaultAsync<int>("select count(1) from pg_class where relname = 'RequestInfo' ") == 0 )
                    {
                        await con.ExecuteAsync(@"
                            CREATE TABLE ""RequestInfo"" ( 
                              ID varchar(50) Primary Key,
                              ParentId varchar(50),
                              Node varchar(50) ,
                              Route varchar(255),
                              Url varchar(255),
                              RequestType varchar(50),
                              Method varchar(10), 
                              Milliseconds Int,
                              StatusCode Int,
                              IP varchar(255),
                              Port Int, 
                              LocalIP varchar(50),
                              LocalPort Int, 
                              CreateTime timestamp(3) without time zone
                            ); 
                        "). ConfigureAwait(false); 
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>("select count(1) from pg_class where relname = 'RequestDetail' ") == 0)
                    {
                        await con.ExecuteAsync(@"
                            CREATE TABLE ""RequestDetail"" ( 
                                ID varchar(50) Primary Key,
                                RequestId varchar(50),
                                Scheme varchar(10),
                                QueryString text,
                                Header text,
                                Cookie text,
                                RequestBody text,
                                ResponseBody text,
                                ErrorMessage text,
                                ErrorStack text,
                                CreateTime timestamp(3) without time zone 
                            ); 
                        ");
                    }


                    if (await con.QueryFirstOrDefaultAsync<int>("select count(1) from pg_class where relname = 'MonitorJob' ") == 0)
                    {
                        await con.ExecuteAsync(@"
                            CREATE TABLE ""MonitorJob"" ( 
                              ID varchar(50) Primary Key,
                              Title varchar(255) ,
                              Description varchar(255),
                              CronLike varchar(255),
                              Emails varchar(1000),
                              WebHook varchar(1000),
                              Mobiles varchar(1000),
                              Status Int,
                              Nodes varchar(255),
                              PayLoad varchar(3000),  
                              CreateTime timestamp(3) without time zone
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>("select count(1) from pg_class where relname = 'SysUser' ") == 0)
                    {
                        await con.ExecuteAsync(@"
                            CREATE TABLE ""SysUser"" ( 
                              ID varchar(50) Primary Key,
                              UserName varchar(255) ,
                              Password varchar(255) 
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>("select count(1) from pg_class where relname = 'SysConfig' ") == 0)
                    {
                        await con.ExecuteAsync(@"
                            CREATE TABLE ""SysConfig"" ( 
                              ID varchar(50) Primary Key,
                              Key varchar(255) ,
                              Value varchar(255) 
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>(@"Select count(1) from ""SysUser"" ") == 0)
                    {
                        await con.ExecuteAsync($@" Insert Into ""SysUser"" (Id,UserName,Password) Values ('{MD5_16(Guid.NewGuid().ToString())}', '{Core.Config.BasicConfig.DefaultUserName}','{Core.Config.BasicConfig.DefaultPassword}') ");
                    }
                     


                    var lang = await con.QueryFirstOrDefaultAsync<string>($@"Select * from ""SysConfig"" Where Key =  '{BasicConfig.Language}' ");

                    if (!lang.IsEmpty())
                    {
                        if (lang.ToLowerInvariant() == "chinese" || lang.ToLowerInvariant() == "english")
                        {
                            await con.ExecuteAsync($@" Delete From ""SysConfig"" Where Key =  '{BasicConfig.Language}'  ");

                            await con.ExecuteAsync($@" Insert Into ""SysConfig"" (Id,Key,Value) Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");

                        }
                    }
                    else
                    {
                        await con.ExecuteAsync($@" Insert Into ""SysConfig"" (Id,Key,Value) Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");
                    }  
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据库初始化失败：" + ex.Message,ex);
            } 
        }

        public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        {
            var whereBuilder = new StringBuilder(BuildSqlFilter(filterOption), 512);

            var sqlBuilder = new StringBuilder(@"Select * From ""RequestInfo"" ", 512);

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

            var where = whereBuilder.ToString();

            sqlBuilder.Append(where);
            sqlBuilder.Append(BuildSqlControl(filterOption));

            var sql = sqlBuilder.ToString();

            TraceLogSql(sql);

            var countSql = @"Select count(1) From ""RequestInfo"" " + where;
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

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            string sql = $@" Update ""SysUser"" Set UserName = @UserName , Password = @Password  Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.ExecuteAsync(sql, model)

             ) > 0);

        }

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            string sql = $@"Update ""MonitorJob""

                Set Title = @Title,Description = @Description,CronLike = @CronLike,Emails = @Emails,WebHook = @WebHook, Mobiles = @Mobiles,Status= @Status,Nodes = @Nodes,PayLoad = @PayLoad 

                Where Id = @Id " ; 

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);
        }

        protected async Task LoggingSqlOperation(Func<IDbConnection, Task> func, string message = null, [System.Runtime.CompilerServices.CallerMemberName]string method = null)
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
            Logger.LogTrace($"Class: {nameof(PostgreSQLStorage)} Method: {method} SQL: {sql}");
        }

       
        protected string BuildSqlFilter(IFilterOption filterOption, bool withOutStatusCode = false)
        {
            var builder = new StringBuilder(256);

            if (filterOption is INodeFilterOption nodeFilterOption)
            {
                if (!nodeFilterOption.Service.IsEmpty())
                {
                    CheckSqlWhere(builder).Append($"Node = '{nodeFilterOption.Service}' ");
                }

                if (!nodeFilterOption.LocalIP.IsEmpty())
                {
                    CheckSqlWhere(builder).Append($"IP = '{nodeFilterOption.LocalIP}' ");
                }

                if (nodeFilterOption.LocalPort > 0)
                {
                    CheckSqlWhere(builder).Append($"Port = {nodeFilterOption.LocalPort} ");
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
                    dateFormat = "to_char(CreateTime,'hh24-mi')";
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

        public async Task<IMonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From ""MonitorJob"" Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql,new { Id })

            ));
        }

        public async Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            string sql = $@" Select * From ""RequestInfo"" Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            string detailSql = $@" Select * From ""RequestDetail"" Where RequestId = @Id";

            TraceLogSql(detailSql);

            var requestDetail = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestDetail>(detailSql, new { Id })

           ));

            return (requestInfo, requestDetail);
        }

        public async Task<IRequestInfo> GetRequestInfo(string Id)
        {
            string sql = $@" Select * From ""RequestInfo"" Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            return requestInfo;
        }

        public async Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            string sql = $@"Select * From ""RequestInfo"" Where ParentId = @ParentId Order By CreateTime ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryAsync<RequestInfo>(sql, new { ParentId })

           ));

            return requestInfo.Select(x => x as IRequestInfo).ToList();
        }

        public async Task ClearData(string StartTime)
        {
            string sql = @"Delete From ""RequestInfo"" Where CreateTime <= @StartTime ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

             await connection.ExecuteAsync(sql, new { StartTime })

           ));
        }

        public async Task SetLanguage(string Language)
        {
            string sql = $@"Update ""SysConfig"" Set Value = @Language Where Key = '{BasicConfig.Language}' ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

             await connection.ExecuteAsync(sql, new { Language })

           ));
        }


        public async Task<string> GetSysConfig(string Key)
        {
            string sql = $@"Select Value From ""SysConfig"" Where Key = @Key ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryFirstOrDefaultAsync<string>(sql, new { Key })

           ));

            return result;
        }

        public async Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        {
            string sql = $@"Select Node,LocalIP,LocalPort from ""RequestInfo"" where CreateTime >= @CreateTime GROUP BY Node,LocalIP,LocalPort ORDER BY LocalIP,LocalPort";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryAsync<ServiceInstanceInfoModel>(sql, new { CreateTime = startTime })

           ));

            return result.Select(x => new ServiceInstanceInfo
            {
                Service = x.Node,
                IP = x.LocalIP,
                Port = x.LocalPort

            }).ToList();
        }

        private class KVClass<TKey, TValue>
        {
            public TKey KeyField { get; set; }
            public TValue ValueField { get; set; }
        }

    }
}
