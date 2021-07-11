using HttpReports.Core; 
using HttpReports.Core.Models;   
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq; 
using FreeSql;
using System.Linq.Expressions;
using HttpReports.Core.ViewModels;
using HttpReports.Core.StorageFilters;
using Snowflake.Core;

namespace HttpReports.Storage.Abstractions
{
    public abstract class BaseStorage : IHttpReportsStorage
    {
        public IFreeSql freeSql { get; set; }

        public BaseStorageOptions _options { get; set; }

        private readonly IdWorker _idWorker;

        public AsyncCallbackDeferFlushCollection<RequestBag> _deferFlushCollection { get; set; }


        public BaseStorage(BaseStorageOptions options)
        {
            _options = options; 

            _idWorker = new IdWorker(new Random().Next(1,30), new Random().Next(1,30));

            _deferFlushCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(AddRequestInfoAsync, _options.DeferThreshold, _options.DeferSecond);

            freeSql = new FreeSql.FreeSqlBuilder().UseConnectionString(_options.DataType, _options.ConnectionString).UseNoneCommandParameter(true).Build();  
        } 

        public async Task InitAsync()
        {
            try
            { 
                await Task.Run(async () =>
                {
                    DbInitializer.Initialize(freeSql,_options.DataType);

                    freeSql.CodeFirst.SyncStructure<RequestInfo>();
                    freeSql.CodeFirst.SyncStructure<RequestDetail>();
                    freeSql.CodeFirst.SyncStructure<Performance>();
                    freeSql.CodeFirst.SyncStructure<MonitorJob>();
                    freeSql.CodeFirst.SyncStructure<MonitorAlarm>();
                    freeSql.CodeFirst.SyncStructure<SysUser>();
                    freeSql.CodeFirst.SyncStructure<SysConfig>();

                    if (!await freeSql.Select<SysUser>().AnyAsync())
                    {
                        await freeSql.Insert(new SysUser
                        {
                            Id = _idWorker.NextId(),
                            UserName = BasicConfig.DefaultUserName,
                            Password = BasicConfig.DefaultPassword

                        }).ExecuteAffrowsAsync();
                    }

                    if (!await freeSql.Select<SysConfig>().Where(x => x.Key == BasicConfig.Language).AnyAsync())
                    {
                        await freeSql.Insert(new SysConfig
                        {

                            Id = _idWorker.NextId(),
                            Key = BasicConfig.Language,
                            Value = BasicConfig.DefaultLanguage

                        }).ExecuteAffrowsAsync();

                    }

                });

            }
            catch (Exception ex)
            {
                throw new Exception("Database init failed：" + ex.Message, ex);
            }
        }

        public async Task PrintSQLAsync()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine().AppendLine("--  HttpReports Migration Start!  ").AppendLine();

            DbInitializer.Initialize(freeSql, _options.DataType);

            List<Type> dbType = new List<Type> { 
                typeof(RequestInfo), 
                typeof(RequestDetail),
                typeof(Performance),
                typeof(MonitorJob),
                typeof(MonitorAlarm),
                typeof(SysUser),
                typeof(SysConfig),
            };

            dbType.ForEach(x => {

                var sql = freeSql.CodeFirst.GetComparisonDDLStatements(x);

                if (sql != null) sb.AppendLine(sql); 

            });

            if (!await freeSql.Select<SysUser>().AnyAsync())
            {
               var sql = freeSql.Insert(new SysUser
                {
                    Id = _idWorker.NextId(),
                    UserName = BasicConfig.DefaultUserName,
                    Password = BasicConfig.DefaultPassword

                }).ToSql();

                if (!sql.IsEmpty()) sb.AppendLine(sql);
            }

            if (!await freeSql.Select<SysConfig>().Where(x => x.Key == BasicConfig.Language).AnyAsync())
            {
                var sql = freeSql.Insert(new SysConfig
                {

                    Id = _idWorker.NextId(),
                    Key = BasicConfig.Language,
                    Value = BasicConfig.DefaultLanguage

                }).ToSql();

                if (!sql.IsEmpty()) sb.AppendLine(sql); 
            } 

            sb.AppendLine().AppendLine("--  HttpReports Migration End  ").AppendLine();

            System.Console.WriteLine(sb.ToString());  

        }



        public async Task SetLanguage(string Language)
            => await freeSql.Update<SysConfig>().Set(x => x.Value == Language).Where(x => x.Key == BasicConfig.Language).ExecuteAffrowsAsync();


        public async Task<string> GetSysConfig(string Key)
            => await freeSql.Select<SysConfig>().Where(x => x.Key == Key).ToOneAsync(x => x.Value);




        public async Task AddRequestInfoAsync(RequestBag bag) => await Task.Run(() => _deferFlushCollection.Flush(bag));

        public async Task AddRequestInfoAsync(List<RequestBag> list, System.Threading.CancellationToken token)
        {
            List<RequestInfo> requestInfos = list.Select(x => x.RequestInfo).ToList();

            List<RequestDetail> requestDetails = list.Select(x => x.RequestDetail).ToList(); 

            await freeSql.Insert(requestInfos).ExecuteAffrowsAsync();

            await freeSql.Insert(requestDetails).ExecuteAffrowsAsync();
        }


        public async Task<RequestInfoSearchResult> GetSearchRequestInfoAsync(QueryDetailFilter filter)
        {
            List<long> detailId = default;

            if (!filter.RequestBody.IsEmpty() || !filter.ResponseBody.IsEmpty())
            {
                detailId = await freeSql.Select<RequestDetail>()
               .Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
               .WhereIf(!filter.RequestBody.IsEmpty(), x => x.RequestBody.Contains(filter.RequestBody))
               .WhereIf(!filter.ResponseBody.IsEmpty(), x => x.ResponseBody.Contains(filter.RequestBody))
               .Limit(100)
               .ToListAsync(x => x.Id);
            } 

            var list = await freeSql.Select<RequestInfo>()
                .Where(x => x.CreateTime >= filter.StartTime && x.CreateTime <= filter.EndTime)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .WhereIf(filter.RequestId > 0, x => x.Id == filter.RequestId)
                .WhereIf(filter.StatusCode > 0, x => x.StatusCode == filter.StatusCode)
                .WhereIf(!filter.Route.IsEmpty(), x => x.Route.Contains(filter.Route))
                .WhereIf(detailId != null && detailId.Any(), x => detailId.Contains(x.Id))
                .WhereIf(!filter.Method.IsEmpty(),x => x.Method == filter.Method)
                .WhereIf(filter.MinMs > 0, x => x.Milliseconds > filter.MinMs)
                .Count(out var total)
                .Page(filter.PageNumber, filter.PageSize)
                .OrderByDescending(x => x.CreateTime)
                .ToListAsync();

            RequestInfoSearchResult result = new RequestInfoSearchResult()
            { 
                List = list,
                Total = total.ToInt() 
            };

            return result;
        }


        public async Task<bool> AddMonitorAlarm(MonitorAlarm alarm)
        {
            alarm.Id = _idWorker.NextId();
            return await freeSql.Insert<MonitorAlarm>(alarm).ExecuteAffrowsAsync() > 0;
        }

        public async Task<List<MonitorAlarm>> GetMonitorAlarms(BasicFilter filter)
        {
            return await freeSql.Select<MonitorAlarm>().OrderByDescending(x => x.CreateTime).Page(filter.PageNumber, filter.PageSize).ToListAsync();
        }

        public async Task<List<string>> GetEndpoints(BasicFilter filter)
        {
            var Start = DateTime.Now.AddDays(-1);

            return await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= Start)  
             .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
             .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
              .OrderBy(x => x.Route)
              .GroupBy(x => x.Route)
              .ToListAsync(x => x.Key);
        } 

        public async Task<bool> AddPerformanceAsync(Performance performance)
        {
            performance.Id = _idWorker.NextId();

            return await freeSql.Insert<Performance>(performance).ExecuteAffrowsAsync() > 0;

        }


        public async Task<bool> GetPerformanceAsync(DateTime start, DateTime end,string service,string instance)
        { 
            return await freeSql.Select<Performance>() 
                .Where(x => x.Service == service && x.Instance == instance)
                .Where(x => x.CreateTime >= start && x.CreateTime < end)
                .CountAsync() > 0;  
        }



        public async Task<List<MonitorJob>> GetMonitorJobs() => await freeSql.Select<MonitorJob>().OrderByDescending(x => x.CreateTime).ToListAsync();


        public async Task<bool> AddMonitorJob(MonitorJob job)
        {
            job.CreateTime = DateTime.Now;
            job.Id = _idWorker.NextId();

            return await freeSql.Insert<MonitorJob>(job).ExecuteAffrowsAsync() > 0;

        }

        public async Task<bool> UpdateMonitorJob(MonitorJob job) => await freeSql.Update<MonitorJob>().SetSource(job).IgnoreColumns(x => x.CreateTime).ExecuteAffrowsAsync() > 0;
         

        public async Task<SysUser> CheckLogin(string Username, string Password)
            => await freeSql.Select<SysUser>().Where(x => x.UserName == Username && x.Password == Password).ToOneAsync();

        public async Task ClearData(string StartTime)
        {
            await freeSql.Delete<RequestInfo>().Where(x => x.CreateTime <= StartTime.ToDateTime()).ExecuteAffrowsAsync();

            await freeSql.Delete<RequestDetail>().Where(x => x.CreateTime <= StartTime.ToDateTime()).ExecuteAffrowsAsync();

            await freeSql.Delete<Performance>().Where(x => x.CreateTime <= StartTime.ToDateTime()).ExecuteAffrowsAsync();

            await freeSql.Delete<MonitorAlarm>().Where(x => x.CreateTime <= StartTime.ToDateTime()).ExecuteAffrowsAsync();
        }

        public async Task<bool> DeleteMonitorJob(long Id) =>

            await freeSql.Delete<MonitorJob>().Where(x => x.Id == Id).ExecuteAffrowsAsync() > 0;


        public async Task<List<APPTimeModel>> GetAppStatus(BasicFilter filter, List<string> range)
        {
            var format = GetDateFormat(filter);

            var list = await freeSql.Select<Performance>()
                .Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .GroupBy(x => new
                {
                    TimeField = x.CreateTime.ToString(format)

                }).ToListAsync(x => new APPTimeModel
                {

                    TimeField = x.Key.TimeField,
                    GcGen0_Raw = x.Avg(x.Value.GCGen0),
                    GcGen1_Raw = x.Avg(x.Value.GCGen1),
                    GcGen2_Raw = x.Avg(x.Value.GCGen2),
                    HeapMemory_Raw = x.Avg(x.Value.HeapMemory),
                    ProcessMemory_Raw = x.Avg(x.Value.ProcessMemory),
                    ThreadCount_Raw = x.Avg(x.Value.ThreadCount)

                });


            var model = new List<APPTimeModel>();

            foreach (var r in range)
            {
                var c = list.Where(x => x.TimeField == r).FirstOrDefault();

                model.Add(new APPTimeModel
                {
                    TimeField = r,
                    GcGen0 = c == null ? 0 : c.GcGen0_Raw.ToInt(),
                    GcGen1 = c == null ? 0 : c.GcGen1_Raw.ToInt(),
                    GcGen2 = c == null ? 0 : c.GcGen2_Raw.ToInt(),
                    HeapMemory = c == null ? 0 : c.HeapMemory_Raw.ToDouble(2),
                    ProcessMemory = c == null ? 0 : c.ProcessMemory_Raw.ToDouble(2),
                    ThreadCount = c == null ? 0 : c.ThreadCount_Raw.ToInt()

                });

            }

            return model;


        }

        public async Task<List<List<TopServiceResponse>>> GetGroupData(BasicFilter filter, GroupType groupType)
        {
            var expression = GetServiceExpression(filter);

            Expression<Func<RequestInfo, string>> exp = default;
            if (groupType == GroupType.Service) exp = x => x.Service;
            if (groupType == GroupType.Instance) exp = x => x.Instance;
            if (groupType == GroupType.Route) exp = x => x.Route;

            List<List<TopServiceResponse>> result = new List<List<TopServiceResponse>>(); 

            var GroupTotal = await freeSql.Select<RequestInfo>().Where(expression).GroupBy(exp).
                OrderByDescending(x => x.Count()).Limit(filter.Count).ToListAsync(x => new TopServiceResponse
                {
                    Key = x.Key,
                    Value = x.Count()
                }); 
          
            var GroupErrorTotal = await freeSql.Select<RequestInfo>().Where(expression).GroupBy(exp).
                OrderByDescending(x => x.Avg(x.Value.Milliseconds * 1.00m)).Limit(filter.Count).ToListAsync(x => new TopServiceResponse
                {
                    Key = x.Key,
                    Value =  Convert.ToInt32( x.Avg(x.Value.Milliseconds * 1.00m))
                }); 

            var GroupAvg = await freeSql.Select<RequestInfo>().Where(expression).Where(x => x.StatusCode == 500).GroupBy(exp)
                .OrderByDescending(x => x.Count()).Limit(filter.Count).ToListAsync(x => new TopServiceResponse
                {
                    Key = x.Key,
                    Value = x.Count()
                });


            result.Add(GroupTotal);
            result.Add(GroupErrorTotal);
            result.Add(GroupAvg);

            return result;
        }


        public async Task<List<BaseNode>> GetTopologyData(BasicFilter filter)
        { 
            var result = await freeSql.Select<RequestInfo>()
                .Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .Where(x => !string.IsNullOrEmpty(x.ParentService))
                .Where(x => x.Service != x.ParentService)
                .GroupBy(x => new
                {
                    x.Service,
                    x.ParentService

                }).ToListAsync(x => new BaseNode
                {

                    Key = x.Key.Service,
                    StringValue = x.Key.ParentService

                });

            return result;
        }

        public async Task<IndexPageData> GetIndexBasicDataAsync(BasicFilter filter)
        {
            IndexPageData result = new IndexPageData();

            var expression = GetServiceExpression(filter);

            result.Total = (await freeSql.Select<RequestInfo>().Where(expression).CountAsync()).ToInt();  
            result.ServerError = (await freeSql.Select<RequestInfo>().Where(expression).Where(x => x.StatusCode == 500).CountAsync()).ToInt();
            result.Service = (await freeSql.Select<RequestInfo>().Where(expression).GroupBy(x => x.Service).CountAsync()).ToInt(); 

            result.Instance = (await freeSql.Select<RequestInfo>().Where(expression).GroupBy(x => x.Instance).CountAsync()).ToInt();

            return result;
        }


        private Expression<Func<RequestInfo, bool>> GetServiceExpression(BasicFilter filter)
        {
            Expression<Func<RequestInfo, bool>> expression = x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime;
            expression = expression
                .And(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .And(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance);

            return expression;

        }

        public async Task<MonitorJob> GetMonitorJob(long Id)
            => await freeSql.Select<MonitorJob>().Where(x => x.Id == Id).ToOneAsync();

        public async Task<RequestInfo> GetRequestInfo(long Id)
            => await freeSql.Select<RequestInfo>().Where(x => x.Id == Id).ToOneAsync();

        public async Task<List<RequestInfo>> GetRequestInfoByParentId(long ParentId)
          => await freeSql.Select<RequestInfo>().Where(x => x.ParentId == ParentId).ToListAsync();

        public async Task<(RequestInfo, RequestDetail)> GetRequestInfoDetail(long Id)
        {
            var info = await freeSql.Select<RequestInfo>().Where(x => x.Id == Id).ToOneAsync();
            var detail = await freeSql.Select<RequestDetail>().Where(x => x.RequestId == Id).ToOneAsync();

            return (info ?? new RequestInfo(), detail ?? new RequestDetail());
        }


        public async Task<List<BaseTimeModel>> GetServiceHeatMap(BasicFilter filter, List<string> Time)
        {
            var format = GetDateFormat(filter);

            var expression = GetServiceExpression(filter);

            string[] span = { "0-200", "200-400", "400-600", "600-800", "800-1000", "1000-1200", "1200-1400", "1400-1600", "1600+" }; 

            var list = await freeSql.Select<RequestInfo>().Where(expression)

                .GroupBy(x => new
                {
                    KeyField = SqlExt.Case()
                    .When(0 < x.Milliseconds && x.Milliseconds <= 200, "0-200")
                    .When(200 < x.Milliseconds && x.Milliseconds <= 400, "200-400")
                    .When(400 < x.Milliseconds && x.Milliseconds <= 600, "400-600")
                    .When(600 < x.Milliseconds && x.Milliseconds <= 800, "600-800")
                    .When(800 < x.Milliseconds && x.Milliseconds <= 1000, "800-1000")
                    .When(1000 < x.Milliseconds && x.Milliseconds <= 1200, "1000-1200")
                    .When(1200 < x.Milliseconds && x.Milliseconds <= 1400, "1200-1400")
                    .When(1400 < x.Milliseconds && x.Milliseconds <= 1600, "1400-1600")
                    .Else("1600+").End(),

                    TimeField = x.CreateTime.ToString(format)

                }).ToListAsync(x => new BaseTimeModel
                {
                    KeyField = x.Key.KeyField,
                    TimeField = x.Key.TimeField,
                    ValueField = x.Count()

                });

            var model = new List<BaseTimeModel>();

            foreach (var t in Time)
            {
                foreach (var s in span)
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

        public async Task<List<ServiceInstanceInfo>> GetServiceInstance(DateTime startTime)
        {  
            return await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= startTime).GroupBy(x => new
            {
                x.Service,
                x.Instance

            }).OrderBy(x => x.Key.Service).OrderBy(x => x.Key.Instance).ToListAsync(x => new ServiceInstanceInfo
            {

                Service = x.Key.Service,
                Instance = x.Key.Instance

            });

        }

        public async Task<List<BaseTimeModel>> GetServiceTrend(BasicFilter filter, List<string> range)
        {
            IEnumerable<string> service = new List<string>() { filter.Service };

            if (filter.Service.IsEmpty()) service = await GetTopServiceLoad(filter);

            var expression = GetServiceExpression(filter);

            var format = GetDateFormat(filter);

            var list = await freeSql.Select<RequestInfo>().Where(expression)

                  .GroupBy(x => new
                  {

                      KeyField = x.Service,
                      TimeField = x.CreateTime.ToString(format)

                  }).ToListAsync(x => new BaseTimeModel
                  {

                      KeyField = x.Key.KeyField,
                      TimeField = x.Key.TimeField,
                      ValueField = x.Count()

                  });


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

        public string GetDateFormat(BasicFilter filter)
        {
            if ((filter.EndTime - filter.StartTime).TotalHours > 1)
            {
                return "dd-HH";
            }
            else
            {
                return "HH:mm";
            }

        }

        public async Task<SysUser> GetSysUser(string UserName)
            => await freeSql.Select<SysUser>().Where(x => x.UserName == UserName).ToOneAsync();


        public async Task<(int timeout, int total)> GetTimeoutResponeCountAsync(ResponseTimeTaskFilter filter)
        {
            var timeout = await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .Where(x => x.Milliseconds >= filter.TimeoutMS)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .CountAsync();

            var total = await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .CountAsync();

            return (timeout.ToInt(), total.ToInt());
        }


        public async Task<(int error, int total)> GetErrorResponeCountAsync(ResponseErrorTaskFilter filter)
        {
            var error = await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                   .Where(x => x.StatusCode >= 400)
                   .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                   .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                   .CountAsync();

            var total = await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .CountAsync();

            return (error.ToInt(), total.ToInt());

        }


        public async Task<int> GetCallCountAsync(CallCountTaskFilter filter)
        {
            var total = await freeSql.Select<RequestInfo>().Where(x => x.CreateTime >= filter.StartTime && x.CreateTime < filter.EndTime)
                .WhereIf(!filter.Service.IsEmpty(), x => x.Service == filter.Service)
                .WhereIf(!filter.Instance.IsEmpty(), x => x.Instance == filter.Instance)
                .CountAsync();

            return total.ToInt();

        }

        public async Task<IEnumerable<string>> GetTopServiceLoad(BasicFilter filter)
        {
            var expression = GetServiceExpression(filter);

            return await freeSql.Select<RequestInfo>().Where(expression)
                .GroupBy(x => x.Service)
                .OrderByDescending(x => x.Count())
                .Limit(filter.Count)
                .ToListAsync(x => x.Key);
        }


        public async Task<bool> UpdateLoginUser(SysUser model)
            => await freeSql.Update<SysUser>().SetSource(model).Where(x => x.Id == model.Id).ExecuteAffrowsAsync() > 0;


        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }


    }
}
