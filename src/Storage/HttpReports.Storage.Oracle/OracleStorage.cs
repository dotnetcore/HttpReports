using Dapper;
using Dapper.Contrib.Extensions;
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

namespace HttpReports.Storage.Oracle
{  
    public class OracleStorage : IHttpReportsStorage
    {
        public OracleConnectionFactory ConnectionFactory { get; }

        public ILogger<OracleStorage> Logger { get; }

        private string TablePrefix { get; set; } = string.Empty;

        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _deferFlushCollection = null;

        public OracleStorageOptions _options;

        public OracleStorage(IOptions<OracleStorageOptions> options,OracleConnectionFactory connectionFactory, ILogger<OracleStorage> logger)
        {
            _options = options.Value;
            if (!_options.TablePrefix.IsEmpty()) TablePrefix = _options.TablePrefix + ".";
            ConnectionFactory = connectionFactory;
            Logger = logger;

            if (_options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(AddRequestInfoAsync, _options.DeferThreshold, _options.DeferSecond);
            } 
        }

        public async Task InitAsync()
        {
            try
            {
                using (var con = ConnectionFactory.GetConnection())
                {    
                    if (await con.QueryFirstOrDefaultAsync<int>($" Select count(*) from user_tables where table_name = upper('RequestInfo') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                            create table RequestInfo
                            (
	                            Id varchar2(50) ,
                                ParentId varchar2(50),
	                            Node varchar2(50),
	                            Route varchar2(120),
	                            Url varchar2(200),
                                RequestType varchar2(50),
	                            Method varchar2(50),
	                            Milliseconds number(15),
	                            StatusCode number(15),
	                            IP varchar2(50), 
                                Port number(15),
                                LocalIP varchar2(50),
                                LocalPort number(15), 
	                            CreateTime date 
                            )

                     ");  
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($" Select count(*) from user_tables where table_name = upper('RequestDetail') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                            create table RequestDetail
                            (
                                Id varchar2(50),
                                RequestId varchar2(50),
                                Scheme varchar2(10),
                                QueryString CLOB,
                                Header CLOB,
                                Cookie CLOB,
                                RequestBody CLOB,
                                ResponseBody CLOB,
                                ErrorMessage CLOB,
                                ErrorStack CLOB,
                                CreateTime date            
                            )

                     ");
                    }

                    if (await con.QueryFirstOrDefaultAsync<int>($" Select count(*) from user_tables where table_name = upper('Performance') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                            create table Performance
                            ( 
                                Id varchar2(50),
                                Service varchar2(200),
                                Instance varchar2(50), 
                                GCGen0 number(8), 
                                GCGen1 number(8), 
                                GCGen2 number(8), 
                                HeapMemory number,
                                ProcessCPU number,
                                ProcessMemory number,
                                ThreadCount number(8),
                                PendingThreadCount number(8), 
                                CreateTime date            
                            )

                     ");
                    }


                    if (await con.QueryFirstOrDefaultAsync<int>("  select count(*) from user_tab_cols where table_name= upper('MonitorJob') and column_name= upper('nodes')  ") > 0 )
                    {
                        await con.ExecuteAsync("drop table MonitorJob purge");
                    } 

                    if (await con.QueryFirstOrDefaultAsync<int>($" Select count(*) from user_tables where table_name = upper('MonitorJob') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                        create table MonitorJob
                        (
	                        id varchar2(50) ,
	                        Title varchar2(255),
	                        Description varchar2(255),
	                        CronLike varchar2(255),
	                        Emails varchar2(1000),
                            WebHook varchar2(1000),
                            Mobiles varchar2(1000),
	                        Status number(15),
                            Service varchar2(255),
                            Instance varchar2(255),
                            PayLoad varchar2(2000), 
	                        CreateTime date
                        )

                     "); 

                    }

                    if (await con.QueryFirstAsync<int>($" Select count(*) from user_tables where table_name = upper('SysUser') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                        create table SysUser
                        (
	                        Id varchar2(50),
	                        UserName varchar2(100),
	                        Password varchar2(100) 
                        )

                     "); 

                    }


                    if (await con.QueryFirstAsync<int>($" Select count(*) from user_tables where table_name = upper('SysConfig') ") == 0)
                    {
                        await con.ExecuteAsync(@"   

                        create table SysConfig
                        (
	                        Id varchar2(50),
	                        Key varchar2(255),
	                        Value varchar2(255) 
                        )

                     ");

                    }


                    if (await con.QueryFirstOrDefaultAsync<int>($" Select count(1) from SysUser ") == 0)
                    {  
                       await con.ExecuteAsync($" Insert Into SysUser Values ('{MD5_16(Guid.NewGuid().ToString())}','{Core.Config.BasicConfig.DefaultUserName}','{Core.Config.BasicConfig.DefaultPassword}') ");
                    } 


                    var lang = await con.QueryFirstOrDefaultAsync<string>($"Select * from SysConfig Where Key =  '{BasicConfig.Language}' ");

                    if (!lang.IsEmpty())
                    {
                        if (lang.ToLowerInvariant() == "chinese" || lang.ToLowerInvariant() == "english")
                        {
                            await con.ExecuteAsync($@" Delete From SysConfig Where Key =  '{BasicConfig.Language}'  ");

                            await con.ExecuteAsync($@" Insert Into SysConfig Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') "); 
                        }
                    }
                    else
                    {
                        await con.ExecuteAsync($@" Insert Into SysConfig Values ('{MD5_16(Guid.NewGuid().ToString())}','{BasicConfig.Language}','en-us') ");
                    }  
                } 
            }
            catch (Exception ex)
            { 
                throw new Exception("Oracle数据库初始化失败：" + ex.Message,ex);
            } 
         
        } 

        public async Task AddRequestInfoAsync(List<RequestBag> list, CancellationToken token)
        {
            await LoggingSqlOperation(async _ =>
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("begin");

                DynamicParameters Parameters = BuildParameters(list);

                List<IRequestInfo> requestInfos = list.Select(x => x.RequestInfo).ToList();

                List<IRequestDetail> requestDetails = list.Select(x => x.RequestDetail).ToList(); 

                foreach (var request in requestInfos)
                {
                    int m = requestInfos.IndexOf(request) + 1;

                    sb.AppendLine($@" Insert Into RequestInfo  Values (:mId{m},:mParentId{m}, :mNode{m}, :mRoute{m}, :mUrl{m},:mRequestType{m},:mMethod{m}, :mMilliseconds{m}, :mStatusCode{m}, :mIP{m},:mPort{m},:mLocalIP{m},:mLocalPort{m},:mCreateTime{m}) ;");
                }

                foreach (var detail in requestDetails)
                {
                    int n = requestDetails.IndexOf(detail) + 1;

                    sb.AppendLine($@"Insert Into RequestDetail Values  (:nId{n},:nRequestId{n},:nScheme{n},:nQueryString{n},:nHeader{n},:nCookie{n},:nRequestBody{n},:nResponseBody{n},:nErrorMessage{n},:nErrorStack{n},:nCreateTime{n}) ; ");
                }

                sb.AppendLine("end;");  

                await _.ExecuteAsync(sb.ToString(), Parameters);
            }, "请求数据批量保存失败");
        } 

        private DynamicParameters BuildParameters(List<RequestBag> bags)
        {
            DynamicParameters parameters = new DynamicParameters();

            AddParameters(bags.Select(x => x.RequestInfo).ToList(),"m");

            AddParameters(bags.Select(x => x.RequestDetail).ToList(),"n");

            return parameters;

            void AddParameters<T>(List<T> list,string key)
            {
                var props = typeof(T).GetProperties().ToList(); 

                foreach (var item in list)
                {
                    foreach (var p in props)
                    {
                        if (p.CanRead)
                        {
                            parameters.Add(key + p.Name + (list.IndexOf(item) + 1), p.GetValue(item));
                        } 
                    } 
                } 
            }  
        }  

        public async Task AddRequestInfoAsync(RequestBag bag)
        {
            bag.RequestDetail = CutRequestDetail(bag.RequestDetail);

            if (_options.EnableDefer)
            {
                _deferFlushCollection.Flush(bag);
            }
            else
            {
                await LoggingSqlOperation(async _ =>
                {
                    string requestSql = $@"Insert Into RequestInfo  Values (:Id,:ParentId, :Node, :Route, :Url,:RequestType,:Method, :Milliseconds, :StatusCode, :IP,:Port,:LocalIP,:LocalPort,:CreateTime) ";

                    await _.ExecuteAsync(requestSql,bag.RequestInfo);

                    string detailSql = $@"Insert Into RequestDetail Values  (:Id,:RequestId,:Scheme,:QueryString,:Header,:Cookie,:RequestBody,:ResponseBody,:ErrorMessage,:ErrorStack,:CreateTime)  ";

                    await _.ExecuteAsync(detailSql,bag.RequestDetail); 

                }, "请求数据保存失败");

            } 
        }

        private IRequestDetail CutRequestDetail(IRequestDetail detail)
        {
            int OracleMaxClob = 4000;

            if (detail.QueryString.Length > OracleMaxClob)
            {
                detail.QueryString = detail.QueryString.Substring(0, OracleMaxClob);
            }

            if (detail.Header.Length > OracleMaxClob)
            {
                detail.Header = detail.Header.Substring(0, OracleMaxClob);
            }

            if (detail.Cookie.Length > OracleMaxClob)
            {
                detail.Cookie = detail.Cookie.Substring(0, OracleMaxClob);
            }

            if (detail.RequestBody.Length > OracleMaxClob)
            {
                detail.RequestBody = detail.RequestBody.Substring(0, OracleMaxClob);
            }

            if (detail.ResponseBody.Length > OracleMaxClob)
            {
                detail.ResponseBody = detail.ResponseBody.Substring(0, OracleMaxClob);
            }

            if (detail.ErrorMessage.Length > OracleMaxClob)
            {
                detail.ErrorMessage = detail.ErrorMessage.Substring(0, OracleMaxClob);
            }

            if (detail.ErrorStack.Length > OracleMaxClob)
            {
                detail.ErrorStack = detail.ErrorStack.Substring(0, OracleMaxClob);
            } 

            return detail; 
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
            await LoggingSqlOperation(async _ =>
            {
                result = (await _.QueryAsync<RequestAvgResponeTime>(sql)).ToList();
            }, "获取Url的平均请求处理时间统计异常");

            return result;
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string where = BuildSqlFilter(filterOption, true);

            var sql = string.Join(" Union ", filterOption.StatusCodes.Select(m => $"Select '{m}' Code,COUNT(1) Total From RequestInfo {where} AND StatusCode = {m}"));

            TraceLogSql(sql);

            List<StatusCodeCount> result = null;
            await LoggingSqlOperation(async _ =>
            {
                result = (await _.QueryAsync<StatusCodeCount>(sql)).ToList();
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
            await LoggingSqlOperation(async _ =>
            {
                result = (await _.QueryAsync<ResponeTimeGroup>(sql)).ToList();
            }, "获取http状态码分组统计异常");

            return result;
        }



        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            string sql = $@"Select Url,COUNT(1)  Total From RequestInfo {BuildSqlFilter(filterOption)} Group By Url order by Total {BuildSqlControl(filterOption)}";

            sql = BuildTopSql(sql,filterOption.Take);

            TraceLogSql(sql);

            List<UrlRequestCount> result = null;
            await LoggingSqlOperation(async _ =>
            {
                result = (await _.QueryAsync<UrlRequestCount>(sql)).ToList();
            });

            return result;
        } 

         
        protected string BuildTopSql(string sql,int Count)
        {
            return " Select * From ( " + sql + " ) Where RowNum <=  " + Count;  
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

            await LoggingSqlOperation(async _ =>
            {
                var data = await _.QueryAsync<KVClass<string,string>>(sql);

                result.Total = data.Where(x => x.KeyField == "Total").FirstOrDefault().ValueField.ToInt();
                result.NotFound = data.Where(x => x.KeyField == "Code404").FirstOrDefault().ValueField.ToInt();
                result.ServerError = data.Where(x => x.KeyField == "Code500").FirstOrDefault().ValueField.ToInt();
                result.APICount = data.Where(x => x.KeyField == "APICount").FirstOrDefault().ValueField.ToInt();
                result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);
                result.AvgResponseTime = data.Where(x => x.KeyField == "ART").FirstOrDefault().ValueField.ToDouble(); 
               
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

            if (filterOption is INodeFilterOption nodeFilterOption)
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
                    CheckSqlWhere(builder).Append($"CreateTime >= to_date('{timeSpanFilterOption.StartTime.Value.ToString(timeSpanFilterOption.StartTimeFormat)}','YYYY-MM-DD hh24:mi:ss') ");
                }
                if (timeSpanFilterOption.EndTime.HasValue)
                {
                    CheckSqlWhere(builder).Append($"CreateTime < to_date('{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}','YYYY-MM-DD hh24:mi:ss') ");
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

            await LoggingSqlOperation(async _ =>
            {
                result.Items = new Dictionary<string, int>();

                var list = (await _.QueryAsync<KVClass<string, int>>(sql)).ToList(); 

                foreach (var item in list)
                {
                    result.Items.Add(item.KeyField, item.ValueField);
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

            string sql = $"Select {dateFormat} KeyField,Round(AVG(Milliseconds),2) ValueField From RequestInfo {where} Group by {dateFormat}";

            TraceLogSql(sql);

            var result = new ResponseTimeStatisticsResult()
            {
                Type = filterOption.Type,
            };

            await LoggingSqlOperation(async _ =>
            {
                result.Items = new Dictionary<string, int>();
                (await _.QueryAsync<KVClass<string, int>>(sql)).ToList().ForEach(m =>
                {
                    result.Items.Add(m.KeyField, m.ValueField);
                });
            }, "获取响应时间统计异常");

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
                case TimeUnit.Minute:
                    dateFormat = "to_char(CreateTime,'hh24-mm')";
                    break;

                case TimeUnit.Hour:
                    dateFormat = "to_char(CreateTime,'dd-hh24')";
                    break; 

                case TimeUnit.Day:
                default:
                    dateFormat = "to_char(CreateTime,'yyyy-MM-dd')";
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

            var countSql = "Select count(1) From RequestInfo " + where;
            TraceLogSql(countSql);

            var result = new RequestInfoSearchResult()
            {
                SearchOption = filterOption,
            };

            await LoggingSqlOperation(async _ =>
            {
                result.AllItemCount = await _.QueryFirstOrDefaultAsync<int>(countSql); 

                var listSql = GetListSql(sql, "CreateTime Desc",filterOption.Page,filterOption.PageSize);

                result.List.AddRange((await _.QueryAsync<RequestInfo>(listSql)).ToArray());
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
                    where = where + $" And CreateTime >= to_date('{timeSpanFilterOption.StartTime.Value.ToString(timeSpanFilterOption.StartTimeFormat)}','YYYY-MM-DD hh24:mi:ss') ";
                }
                if (timeSpanFilterOption.EndTime.HasValue)
                {
                    where = where + $" And CreateTime < to_date('{timeSpanFilterOption.EndTime.Value.ToString(timeSpanFilterOption.EndTimeFormat)}','YYYY-MM-DD hh24:mi:ss') ";
                }
            }  
 
            string sql = $"Select RequestId From RequestDetail {where}  ";

            var list = await LoggingSqlOperation(async connection => await connection.QueryAsync<string>(sql));

            return list;
        }

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption filterOption)
        {
            var sql = $"SELECT COUNT(1) FROM RequestInfo {BuildSqlFilter(filterOption)}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => await _.QueryFirstOrDefaultAsync<int>(sql));
        }

        /// <summary>
        /// 获取白名单外的获取请求总次数
        /// </summary>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public async Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption filterOption)
        {
            var ipFilter = $"({string.Join(",", filterOption.List.Select(x=> $"'{x}'"))})";
            if (filterOption.InList)
            {
                ipFilter = "IP IN " + ipFilter;
            }
            else
            {
                ipFilter = "IP NOT IN " + ipFilter;
            }

            var sql = $"SELECT COUNT(1) TOTAL FROM RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter} GROUP BY IP ORDER BY TOTAL DESC ";

            sql = BuildTopSql(sql,1);

            TraceLogSql(sql);

            var max = await LoggingSqlOperation(async _ => await _.QueryFirstOrDefaultAsync<int>(sql));
            sql = $"SELECT COUNT(1) TOTAL FROM RequestInfo {BuildSqlFilter(filterOption)} AND {ipFilter}";


            TraceLogSql(sql);
            var all = await LoggingSqlOperation(async _ => await _.QueryFirstOrDefaultAsync<int>(sql));
            return (max, all);
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption filterOption, int timeoutThreshold)
        {
            var where = BuildSqlFilter(filterOption);
            var sql = $"SELECT COUNT(1) FROM RequestInfo {(string.IsNullOrWhiteSpace(where) ? "WHERE" : where)} AND Milliseconds >= {timeoutThreshold}";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => await _.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<bool> AddMonitorJob(IMonitorJob job)
        {
            job.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into MonitorJob 
            (Id,Title,Description,CronLike,Emails,WebHook,Mobiles,Status,Service,Instance,PayLoad,CreateTime)
             Values ('{Guid.NewGuid().ToString()}',:Title,:Description,:CronLike,:Emails,:WebHook,:Mobiles,:Status,:Service,:Instance,:Payload,to_date('{job.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}','YYYY-MM-DD HH24:MI:SS'))";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => ( await _.ExecuteAsync(sql, job)  ) > 0);

        }

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            string sql = $@"Update MonitorJob 

                Set Title = :Title ,Description = :Description,CronLike = :CronLike ,Emails = :Emails ,Mobiles = :Mobiles , WebHook = :WebHook, Status = :Status , Service =:Service,Instance = :Instance ,PayLoad = :Payload' 

                Where Id = :Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

            await _.ExecuteAsync(sql, job)

            ) > 0);
        }

        public async Task<IMonitorJob> GetMonitorJob(string Id)
        {
            string sql = $@"Select * From MonitorJob Where Id = :Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

              await _.QueryFirstOrDefaultAsync<MonitorJob>(sql,new { Id })

            ));
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            string sql = $@"Select * From MonitorJob ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

            await _.QueryAsync<MonitorJob>(sql)

            ).ToList().Select(x => x as IMonitorJob).ToList());
        }

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            string sql = $@"Delete From MonitorJob Where Id = :Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ =>
            (await _.ExecuteAsync(sql,new { Id })) > 0);
        }

        public async Task<SysUser> CheckLogin(string Username, string Password)
        {
            string sql = $" Select * From SysUser Where UserName = :UserName AND Password = :Password ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

              await _.QueryFirstOrDefaultAsync<SysUser>(sql, new { Username, Password })

            ));

        }

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            string sql = $" Update SysUser Set UserName = :UserName , Password = :Password  Where Id = :Id ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

              await _.ExecuteAsync(sql, model)

             ) > 0);

        }


        public async Task<SysUser> GetSysUser(string UserName)
        {
            string sql = $" Select * From SysUser Where UserName = :UserName ";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

              await _.QueryFirstOrDefaultAsync<SysUser>(sql, new { UserName })

            ));
        }

        public async Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            string sql = $" Select * From RequestInfo Where Id = :Id ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async _ => (

             await _.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            string detailSql = $" Select * From RequestDetail Where RequestId = :Id ";

            TraceLogSql(detailSql);

            var requestDetail = await LoggingSqlOperation(async _ => (

             await _.QueryFirstOrDefaultAsync<RequestDetail>(detailSql, new { Id })

           ));

            return (requestInfo, requestDetail);
        }

        public async Task<IRequestInfo> GetRequestInfo(string Id)
        {
            string sql = $" Select * From RequestInfo Where Id = :Id ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async _ => (

             await _.QueryFirstOrDefaultAsync<RequestInfo>(sql, new { Id })

           ));

            return requestInfo;
        }

        public async Task<List<IRequestInfo>> GetRequestInfoByParentId(string ParentId)
        {
            string sql = $" Select * From RequestInfo Where ParentId = :ParentId Order By CreateTime ";

            TraceLogSql(sql);

            var requestInfo = await LoggingSqlOperation(async _ => (

             await _.QueryAsync<RequestInfo>(sql,new { ParentId })

           ));

            return requestInfo.Select(x => x as IRequestInfo).ToList();
        }

        public async Task ClearData(string StartTime)
        {
            string sql = $"Delete From RequestInfo Where CreateTime <= to_date('{StartTime}','YYYY-MM-DD') "; 
            await LoggingSqlOperation(async _ => await _.ExecuteAsync(sql));

            string detailSql = $"Delete From RequestDetail Where CreateTime <= to_date('{StartTime}','YYYY-MM-DD') ";
            await LoggingSqlOperation(async _ => await _.ExecuteAsync(detailSql));

            string performanceSql = $"Delete From Performance Where CreateTime <= to_date('{StartTime}','YYYY-MM-DD') "; 
            await LoggingSqlOperation(async _ =>  await _.ExecuteAsync(performanceSql, new { StartTime }));


        }

        public async Task SetLanguage(string Language)
        {
            string sql = $"Update SysConfig Set Value = :Language  Where Key = '{BasicConfig.Language}' ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async _ => (

             await _.ExecuteAsync(sql,new { Language })

           ));
        }

        public async Task<string> GetSysConfig(string Key)
        {
            string sql = $"Select Value  From SysConfig Where Key = '{Key}' ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async _ => (

               await _.QueryFirstOrDefaultAsync<string>(sql, new { Key })

           ));

            return result;
        }

        public async Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        {
            string sql = "Select Node,LocalIP,LocalPort from RequestInfo where CreateTime >= :CreateTime GROUP BY Node,LocalIP,LocalPort  ORDER BY LocalIP,LocalPort ";

            TraceLogSql(sql);

            var result = await LoggingSqlOperation(async _ => (

               await _.QueryAsync<ServiceInstanceInfoModel>(sql, new { CreateTime = startTime })

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
            string where = $" where  CreateTime >= to_date('{option.Start:yyyy-MM-dd HH:mm:ss}','YYYY-MM-DD hh24:mi:ss') AND CreateTime < to_date('{option.End:yyyy-MM-dd HH:mm:ss}','YYYY-MM-DD hh24:mi:ss')  ";

            if (!option.Service.IsEmpty())
            {
                where = where + " AND Service = :Service ";
            }

            if (!option.Instance.IsEmpty())
            {
                where = where + " AND Instance = :Instance ";
            }


            string sql = " Select * From Performance " + where;

            TraceLogSql(sql); 

            var result = await LoggingSqlOperation(async _ =>  await _.QueryAsync<Performance>(sql, option));

            return result.Select(x => x as IPerformance).ToList();

        }

        public async Task<bool> AddPerformanceAsync(IPerformance performance)
        {
            performance.Id = MD5_16(Guid.NewGuid().ToString());

            string sql = $@"Insert Into Performance 
            (Id,Service,Instance,GCGen0,GCGen1,GCGen2,HeapMemory,ProcessCPU,ProcessMemory,ThreadCount,PendingThreadCount,CreateTime)
             Values (:Id,:Service,:Instance,:GCGen0,:GCGen1,:GCGen2,:HeapMemory,:ProcessCPU,:ProcessMemory,:ThreadCount,:PendingThreadCount, to_date('{performance.CreateTime:yyyy-MM-dd HH:mm:ss}','YYYY-MM-DD hh24:mi:ss') )";

            TraceLogSql(sql);

            return await LoggingSqlOperation(async _ => (

            await _.ExecuteAsync(sql, performance)

            ) > 0);

        }
    }
}