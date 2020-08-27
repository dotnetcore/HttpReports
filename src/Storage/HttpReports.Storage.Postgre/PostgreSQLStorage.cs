using Dapper;
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
    public class PostgreSQLStorage : IHttpReportsStorage
    {
        public PostgreStorageOptions Options { get; }

        public PostgreConnectionFactory ConnectionFactory { get; }

        public ILogger<PostgreSQLStorage> Logger { get; }

        private string Prefix { get; set; } = string.Empty;

        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _deferFlushCollection = null;

        public PostgreSQLStorage(IOptions<PostgreStorageOptions> options, PostgreConnectionFactory connectionFactory, ILogger<PostgreSQLStorage> logger)
        {
            Options = options.Value;
            ConnectionFactory = connectionFactory;
            if (!Options.TablePrefix.IsEmpty()) Prefix = Options.TablePrefix + ".";
            Logger = logger;
            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferSecond);
            }
        }

        public async Task AddRequestInfoAsync(List<RequestBag> list, System.Threading.CancellationToken token)
        {
            await LoggingSqlOperation(async connection =>
            {
                List<IRequestInfo> requestInfos = list.Select(x => x.RequestInfo).ToList();

                List<IRequestDetail> requestDetails = list.Select(x => x.RequestDetail).ToList();

                if (requestInfos.Where(x => x != null).Any())
                {
                    var request = string.Join(",", requestInfos.Select(item =>
                    {

                        int i = requestInfos.IndexOf(item) + 1;

                        return $"(@Id{i},@ParentId{i},@Node{i}, @Route{i}, @Url{i},@RequestType{i}, @Method{i}, @Milliseconds{i}, @StatusCode{i}, @IP{i},@Port{i},@LocalIP{i},@LocalPort{i},@CreateTime{i})";

                    }));

                    await connection.ExecuteAsync($@"INSERT INTO ""{Prefix}RequestInfo"" (Id,ParentId,Node, Route, Url,RequestType,Method, Milliseconds, StatusCode, IP,Port,LocalIP,LocalPort,CreateTime) VALUES {request}", BuildParameters(requestInfos));

                }

                if (requestDetails.Where(x => x != null).Any())
                {
                    string detail = string.Join(",", requestDetails.Select(item =>
                    {

                        int i = requestDetails.IndexOf(item) + 1;

                        return $"(@Id{i},@RequestId{i},@Scheme{i},@QueryString{i},@Header{i},@Cookie{i},@RequestBody{i},@ResponseBody{i},@ErrorMessage{i},@ErrorStack{i},@CreateTime{i}) ";


                    }));

                    await connection.ExecuteAsync($@"Insert into ""{Prefix}RequestDetail"" (Id,RequestId,Scheme,QueryString,Header,Cookie,RequestBody,ResponseBody,ErrorMessage,ErrorStack,CreateTime) VALUES {detail}", BuildParameters(requestDetails));


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




        public async Task<bool> AddMonitorJob(IMonitorJob job)
        {
            job.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into ""{Prefix}MonitorJob"" 
            (Id,Title,Description,CronLike,Emails,WebHook,Mobiles,Status,Service,Instance,PayLoad,CreateTime)
             Values (@Id,@Title,@Description,@CronLike,@Emails,@WebHook, @Mobiles,@Status,@Service,@Instance,@PayLoad,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, job)

            ) > 0);

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
                    await connection.ExecuteAsync($@"INSERT INTO ""{Prefix}RequestInfo"" (Id,ParentId,Node, Route,Url,RequestType, Method, Milliseconds, StatusCode, IP,Port,LocalIP,LocalPort,CreateTime) VALUES (@Id,@ParentId,@Node, @Route, @Url,@RequestType, @Method, @Milliseconds, @StatusCode, @IP,@Port,@LocalIP,@LocalPort,@CreateTime)", bag.RequestInfo);

                    await connection.ExecuteAsync($@"INSERT INTO ""{Prefix}RequestDetail"" (Id,RequestId,Scheme,QueryString,Header,Cookie,RequestBody,ResponseBody,ErrorMessage,ErrorStack,CreateTime)  VALUES (@Id,@RequestId,@Scheme,@QueryString,@Header,@Cookie,@RequestBody,@ResponseBody,@ErrorMessage,@ErrorStack,@CreateTime)", bag.RequestDetail);

                }, "请求数据保存失败");
            }
        }

        public async Task<SysUser> CheckLogin(string Username, string Password)
        {
            string sql = $@" Select * From ""{Prefix}SysUser"" Where UserName = @UserName AND Password = @Password ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<SysUser>(sql, new { Username, Password })

            ));

        }

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From ""{Prefix}MonitorJob"" Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection =>
            (await connection.ExecuteAsync(sql, new { Id })) > 0);
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



        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            string sql = $@"Select * From ""{Prefix}MonitorJob"" ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.QueryAsync<MonitorJob>(sql)

            ).ToList().Select(x => x as IMonitorJob).ToList());
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

        public async Task InitAsync()
        {
            try
            {
                using (var con = ConnectionFactory.GetConnection())
                {
                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}RequestInfo' ") == 0)
                    {
                        await con.ExecuteAsync($@"
                            CREATE TABLE ""{Prefix}RequestInfo"" ( 
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
                        ").ConfigureAwait(false);
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}RequestDetail' ") == 0)
                    {
                        await con.ExecuteAsync($@"
                            CREATE TABLE ""{Prefix}RequestDetail"" ( 
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

                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}Performance' ") == 0)
                    {
                        await con.ExecuteAsync($@" 

                            CREATE TABLE ""{Prefix}Performance"" ( 
                                ID varchar(50) Primary Key,
                                Service varchar(200),
                                Instance varchar(200),
                                GCGen0 Int,
                                GCGen1 Int,
                                GCGen2 Int,
                                HeapMemory Numeric,
                                ProcessCPU Numeric,
                                ProcessMemory Numeric, 
                                ThreadCount Int,
                                PendingThreadCount Int, 
                                CreateTime timestamp(3) without time zone 

                            ); 
                        ");
                    }






                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from information_schema.columns where table_catalog = '{ConnectionFactory.DataBase}' and table_name = '{Prefix}MonitorJob' and column_name = 'nodes' ") > 0)
                    {
                        await con.ExecuteAsync($@"DROP TABLE ""{Prefix}MonitorJob"" ");
                    }


                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}MonitorJob' ") == 0)
                    {
                        await con.ExecuteAsync($@"
                            CREATE TABLE ""{Prefix}MonitorJob"" ( 
                              ID varchar(50) Primary Key,
                              Title varchar(255) ,
                              Description varchar(255),
                              CronLike varchar(255),
                              Emails varchar(1000),
                              WebHook varchar(1000),
                              Mobiles varchar(1000),
                              Status Int,
                              Service varchar(255),
                              Instance varchar(255),
                              PayLoad varchar(3000),  
                              CreateTime timestamp(3) without time zone
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}SysUser' ") == 0)
                    {
                        await con.ExecuteAsync($@"
                            CREATE TABLE ""{Prefix}SysUser"" ( 
                              ID varchar(50) Primary Key,
                              UserName varchar(255) ,
                              Password varchar(255) 
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($"select count(1) from pg_class where relname = '{Prefix}SysConfig' ") == 0)
                    {
                        await con.ExecuteAsync($@"
                            CREATE TABLE ""{Prefix}SysConfig"" ( 
                              ID varchar(50) Primary Key,
                              Key varchar(255) ,
                              Value varchar(255) 
                            ); 
                        ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($@"Select count(1) from ""{Prefix}SysUser"" ") == 0)
                    {
                        await con.ExecuteAsync($@" Insert Into ""{Prefix}SysUser"" (Id,UserName,Password) Values ('{MD5_16(Guid.NewGuid().ToString())}', '{Core.Config.BasicConfig.DefaultUserName}','{Core.Config.BasicConfig.DefaultPassword}') ");
                    }



                    var lang = await con.QueryFirstOrDefaultAsync<string>($@"Select * from ""{Prefix}SysConfig"" Where Key =  '{BasicConfig.Language}' ");

                    if (!lang.IsEmpty())
                    {
                        if (lang.ToLowerInvariant() == "chinese" || lang.ToLowerInvariant() == "english")
                        {
                            await con.ExecuteAsync($@" Delete From ""{Prefix}SysConfig"" Where Key =  '{BasicConfig.Language}'  ");

                            await con.ExecuteAsync($@" Insert Into ""{Prefix}SysConfig"" (Id,Key,Value) Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");

                        }
                    }
                    else
                    {
                        await con.ExecuteAsync($@" Insert Into ""{Prefix}SysConfig"" (Id,Key,Value) Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据库初始化失败：" + ex.Message, ex);
            }
        }

        public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption filterOption)
        {
            var whereBuilder = new StringBuilder(BuildSqlFilter(filterOption), 512);

            var sqlBuilder = new StringBuilder($@"Select * From ""{Prefix}RequestInfo"" ", 512);

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
            sqlBuilder.Append(BuildSqlControl(filterOption));

            var sql = sqlBuilder.ToString();

            TraceLogSql(sql);

            var countSql = $@"Select count(1) From ""{Prefix}RequestInfo"" " + where;
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

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
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

        public async Task<IMonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From ""{Prefix}MonitorJob"" Where Id = @Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

              await connection.QueryFirstOrDefaultAsync<MonitorJob>(sql, new { Id })

            ));
        }

        public async Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            string sql = $@" Select * From ""{Prefix}RequestInfo"" Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            string detailSql = $@" Select * From ""{Prefix}RequestDetail"" Where RequestId = @Id";

            TraceLogSql(detailSql);

            var requestDetail = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestDetail>(detailSql, new { Id })

           ));

            return (requestInfo, requestDetail);
        }

        public async Task<IRequestInfo> GetRequestInfo(string Id)
        {
            string sql = $@" Select * From ""{Prefix}RequestInfo"" Where Id = @Id";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            return requestInfo;
        }

        public async Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            string sql = $@"Select * From ""{Prefix}RequestInfo"" Where ParentId = @ParentId Order By CreateTime ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async connection => (

             await connection.QueryAsync<RequestInfo>(sql, new { ParentId })

           ));

            return requestInfo.Select(x => x as IRequestInfo).ToList();
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

        public async Task SetLanguage(string Language)
        {
            string sql = $@"Update ""{Prefix}SysConfig"" Set Value = @Language Where Key = '{BasicConfig.Language}' ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

             await connection.ExecuteAsync(sql, new { Language })

           ));
        }


        public async Task<string> GetSysConfig(string Key)
        {
            string sql = $@"Select Value From ""{Prefix}SysConfig"" Where Key = @Key ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryFirstOrDefaultAsync<string>(sql, new { Key })

           ));

            return result;
        }

        public async Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        {
            string sql = $@"Select Node,LocalIP,LocalPort from ""{Prefix}RequestInfo"" where CreateTime >= @CreateTime GROUP BY Node,LocalIP,LocalPort ORDER BY LocalIP,LocalPort";

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


            string sql = $@" Select * From ""{Prefix}Performance"" " + where;

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async connection => (

               await connection.QueryAsync<Performance>(sql, option)

           ));

            return result.Select(x => x as IPerformance).ToList();

        }

        public async Task<bool> AddPerformanceAsync(IPerformance performance)
        {
            performance.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into ""{Prefix}Performance"" (Id,Service,Instance,GCGen0,GCGen1,GCGen2,HeapMemory,ProcessCPU,ProcessMemory,ThreadCount,PendingThreadCount,CreateTime)
             Values (@Id,@Service,@Instance,@GCGen0,@GCGen1,@GCGen2,@HeapMemory,@ProcessCPU,@ProcessMemory,@ThreadCount,@PendingThreadCount,@CreateTime)";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async connection => (

            await connection.ExecuteAsync(sql, performance)

            ) > 0);

        }

         
        public async Task<IndexPageData> GetIndexBasicDataAsync(IndexPageDataFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption);

            string sql = $@"
        Select COUNT(1) Total From ""{Prefix}RequestInfo"" {where}; 
        Select COUNT(1) Code500 From ""{Prefix}RequestInfo"" {where} AND StatusCode = 500;
        SELECT Count(DISTINCT(Node)) From ""{Prefix}RequestInfo"" {where} ;  
        Select Count(1) from ( SELECT LocalIP,LocalPort from ""{Prefix}RequestInfo""  {where} GROUP BY LocalIP,LocalPort) Z;";

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

        public async Task<IEnumerable<string>> GetTopServiceLoad(IndexPageDataFilterOption filterOption)
        {
            string sql = $@"Select Node  From ""{Prefix}RequestInfo"" {BuildSqlFilter(filterOption, false, true)} Group by Node  ORDER BY COUNT(1)  Desc Limit {filterOption.Take}  ";

            return await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

        } 

        public async Task<List<List<TopServiceResponse>>> GetGroupData(IndexPageDataFilterOption filterOption,GroupType group)
        {
            string groupName = default;

            if (group == GroupType.Node) groupName = "Node";
            if (group == GroupType.Route) groupName = "Route";
            if (group == GroupType.Instance) groupName = "LocalIP,LocalPort";  

            string where = BuildSqlFilter(filterOption);

            string sql = $@"

            Select {groupName},COUNT(1) From ""{Prefix}RequestInfo"" {where} Group by {groupName}  ORDER BY COUNT(1) Desc Limit {filterOption.Take} ;
            Select {groupName},AVG(Milliseconds) From ""{Prefix}RequestInfo"" {where} Group by {groupName} ORDER BY  Avg(Milliseconds) Desc Limit {filterOption.Take} ; 
            Select {groupName},COUNT(1) From ""{Prefix}RequestInfo"" {where} AND StatusCode = 500 Group by {groupName}  ORDER BY COUNT(1) Desc Limit {filterOption.Take} ; 

            ";

            TraceLogSql(sql);

            List<List<TopServiceResponse>> result = new List<List<TopServiceResponse>>();

            await LoggingSqlOperation(async connection => 
            {
                using (var resultReader = await connection.QueryMultipleAsync(sql))
                {

                    if (group == GroupType.Instance)
                    {
                        result.Add(resultReader.Read<(string localIP,string localPort, double value)>().Select(x => new TopServiceResponse { Service = x.localIP + ":" + x.localPort, Value = x.value.ToInt() }).ToList());
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


        public async Task<List<APPTimeModel>> GetAppStatus(IndexPageDataFilterOption filterOption, List<string> range)
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

            string where = $" where  CreateTime >= '{filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat)}' AND CreateTime < '{filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat)}'  ";

            if (service.Any())
            {
                if (service.Count() == 1)
                {
                    where = where + $" AND Service = '{service.FirstOrDefault()}' ";
                }
                else
                {
                    where = where + $" AND Service In  ({string.Join(",", service.Select(x => $"'{x}'"))}) ";
                }
            }


            if (filterOption.LocalIP.IsEmpty() && filterOption.LocalPort > 0)
            {
                where = where + $" AND Instance = '{filterOption.LocalIP+":"+filterOption.LocalPort}'  ";
            } 

            string sql = $@" SELECT AVG(GcGen0) GcGen0, AVG(GcGen1) GcGen1, AVG(GcGen2) GcGen2,AVG(HeapMemory) HeapMemory,AVG(ThreadCount) ThreadCount,{DateFormat} TimeField From ""Performance"" {where} GROUP BY {DateFormat} ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<APPTimeModel>(sql, new
            {
                Start = filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat),
                End = filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat),
                NodeList = service.ToArray()

            }));

            var model = new List<APPTimeModel>(); 
           
            foreach (var r in range)
            {
                var c = list.Where(x => x.TimeField == r ).FirstOrDefault();

                model.Add(new APPTimeModel
                {
                    TimeField = r,
                    GcGen0 = c == null ? 0 : c.GcGen0.ToString().ToDouble(2),
                    GcGen1 = c == null ? 0 : c.GcGen1.ToString().ToDouble(2),
                    GcGen2 = c == null ? 0 : c.GcGen2.ToString().ToDouble(2),
                    HeapMemory = c == null ? 0 : c.HeapMemory.ToString().ToDouble(2),
                    ThreadCount = c == null ? 0 : c.ThreadCount
                });

            } 

            return model;

        }




        public async Task<List<BaseTimeModel>> GetServiceTrend(IndexPageDataFilterOption filterOption, List<string> range)
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

            string where = $" where  CreateTime >= '{filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat)}' AND CreateTime < '{filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat)}'  ";

            if (service.Any())
            {
                if (service.Count() == 1)
                {
                    where = where + $" AND Node = '{service.FirstOrDefault()}' ";
                }
                else
                {
                    where = where + $" AND Node In  ({string.Join(",",service.Select(x => $"'{x}'"))}) ";
                }
            }

            if (!filterOption.LocalIP.IsEmpty()) where = where + $" AND LocalIP = '{filterOption.LocalIP}' ";
            if (filterOption.LocalPort > 0) where = where + $" AND LocalPort = {filterOption.LocalPort} ";

            string sql = $@" SELECT Node KeyField, {DateFormat} TimeField,COUNT(1) ValueField From ""RequestInfo"" {where} GROUP BY Node,{DateFormat} ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<BaseTimeModel>(sql, new
            { 
                Start = filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat),
                End = filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat),
                NodeList = service.ToArray()

            }));

            var model = new List<BaseTimeModel>();

            foreach (var s in service)
            {
                foreach (var r in range)
                {
                    var c = list.Where(x => x.KeyField == s && x.TimeField == r).FirstOrDefault();

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

        public async Task<List<BaseTimeModel>> GetServiceHeatMap(IndexPageDataFilterOption filterOption, List<string> Time, List<string> Span)
        {
            string where = $" where   CreateTime >= '{filterOption.StartTime.Value.ToString(filterOption.StartTimeFormat)}' AND CreateTime < '{filterOption.EndTime.Value.ToString(filterOption.EndTimeFormat)}' ";

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
                  From ""RequestInfo"" {where} GROUP BY KeyField,TimeField  ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<BaseTimeModel>(sql, new
            {

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

                    model.Add(new BaseTimeModel
                    { 
                        TimeField = t,
                        KeyField = s,
                        ValueField = c == null ? 0 : c.ValueField 
                    });
                }
            }

            return model;
        }

        private class KVClass<TKey, TValue>
        {
            public TKey KeyField { get; set; }
            public TValue ValueField { get; set; }
        }

    }
}
