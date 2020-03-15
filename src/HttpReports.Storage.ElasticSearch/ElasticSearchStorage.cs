using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.ElasticSearch.WhereExtension;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeInfo = HttpReports.Models.NodeInfo;

namespace HttpReports.Storage.ElasticSearch
{
    public class ElasticSearchStorage : IHttpReportsStorage
    {
        public ElasticSearchStorageOptions Options { get; }

        public ElasticClient Client { get; }

        public ILogger<ElasticSearchStorage> Logger { get; }

        private readonly AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail> _deferFlushCollection = null;

        public ElasticSearchStorage(IOptions<ElasticSearchStorageOptions> options, ILogger<ElasticSearchStorage> logger, ElasticSearchConnectionFactory connectionFactory)
        {
            Options = options.Value;
            Logger = logger;
            Client = connectionFactory.Client;

            if (Options.EnableDefer)
            {
                _deferFlushCollection = new AsyncCallbackDeferFlushCollection<IRequestInfo, IRequestDetail>(AddRequestInfoAsync, Options.DeferThreshold, Options.DeferSecond);
            }
        }

        public async Task<bool> AddMonitorJob(IMonitorJob job)
        {
            var model = job as MonitorJob;

            model.Id = MD5_16(Guid.NewGuid().ToString());

            var response = await Client.IndexAsync<MonitorJob>(model, x => x.Index(GetIndexName<MonitorJob>())).ConfigureAwait(false);

            return response.IsValid;
        }

        private async Task AddRequestInfoAsync(Dictionary<IRequestInfo, IRequestDetail> list, CancellationToken token)
        {
            BulkDescriptor requestDescriptor = new BulkDescriptor(GetIndexName<RequestInfo>());

            foreach (var item in list.Select(x=>x.Key))
            {
                requestDescriptor.Create<IRequestInfo>(op => op.Document(item));
            }

            await Client.BulkAsync(requestDescriptor).ConfigureAwait(false);



            BulkDescriptor detailDescriptor = new BulkDescriptor(GetIndexName<RequestDetail>());

            foreach (var item in list.Select(x => x.Value))
            {
                detailDescriptor.Create<IRequestDetail>(op => op.Document(item));
            }

            await Client.BulkAsync(detailDescriptor).ConfigureAwait(false);  

        }

        public async Task AddRequestInfoAsync(IRequestInfo request, IRequestDetail detail)
        {
            if (Options.EnableDefer)
            {
                _deferFlushCollection.Push(request, detail);
            }
            else
            { 

                await Client.IndexAsync<RequestInfo>(request as RequestInfo, x => x.Index(GetIndexName<RequestInfo>())).ConfigureAwait(false);

                await Client.IndexAsync<RequestDetail>(detail as RequestDetail, x => x.Index(GetIndexName<RequestDetail>())).ConfigureAwait(false); 

            }
        }

        public async Task<SysUser> CheckLogin(string Username, string Password)
        {
            var response = await Client.SearchAsync<SysUser>(a => a.Index(GetIndexName<SysUser>()).Query(b =>

            b.Term(c => c.UserName, Username) && b.Term(c => c.Password, Password)

            )).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return response.Documents.FirstOrDefault();
            }

            return new SysUser();
        }

        public async Task<bool> DeleteMonitorJob(string Id)
        {
            var response = await Client.DeleteAsync<MonitorJob>(Id, a => a.Index(GetIndexName<MonitorJob>())).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Func<AggregationRangeDescriptor, IAggregationRange>[] GetTimeRangeFunc(int[,] group)
        {
            List<Func<AggregationRangeDescriptor, IAggregationRange>> funcs = new List<Func<AggregationRangeDescriptor, IAggregationRange>>();

            var groupCount = group.Length / group.Rank;

            for (int i = 0; i < groupCount; i++)
            {
                var min = group[i, 0];
                var max = group[i, 1];

                if (min < max)
                {
                    funcs.Add(t => t.From(min).To(max).Key($"{min}-{max}"));
                }
                else
                {
                    funcs.Add(t => t.From(min).Key($"{min}以上"));
                }
            }

            return funcs.ToArray();
        }


        public async Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption option)
        {
            List<ResponeTimeGroup> responeTimeGroups = new List<ResponeTimeGroup>();

            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

            .Query(c =>

             c.HasDateWhere(option.StartTime, option.EndTime)

             && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

             && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

            ).Aggregations(a => a.Range("timeRange", b => b.Field(c => c.Milliseconds).Ranges(

                 GetTimeRangeFunc(option.TimeGroup)

              ))).Size(0)

            ).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.RangeBucket;

                        responeTimeGroups.Add(new ResponeTimeGroup
                        {

                            Name = model.Key,
                            Total = Convert.ToInt32(model.DocCount)

                        });

                    }

                }
            }

            return responeTimeGroups;
        }

        public async Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption option)
        {
            IndexPageData result = new IndexPageData();

            var Total = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
            .Query(c => c.HasDateWhere(option.StartTime, option.EndTime) && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes)))
             .Aggregations(c => c.ValueCount("Id", d => d.Field(e => e.Id))).Size(0)
            ).ConfigureAwait(false);

            if (Total != null && Total.IsValid)
            {
                result.Total = Convert.ToInt32(Total.Total);
            }

            var NotFound = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
           .Query(c => c.HasDateWhere(option.StartTime, option.EndTime)
             && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
             && c.Term(f => f.StatusCode, 404)
           )
            .Aggregations(c => c.ValueCount("Id", d => d.Field(e => e.Id))).Size(0)
           ).ConfigureAwait(false);

            if (NotFound != null && NotFound.IsValid)
            {
                result.NotFound = Convert.ToInt32(NotFound.Total);
            }

            var ServerError = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
          .Query(c => c.HasDateWhere(option.StartTime, option.EndTime)
            && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
            && c.Term(f => f.StatusCode, 500)
          )
           .Aggregations(c => c.ValueCount("Id", d => d.Field(e => e.Id))).Size(0)
          ).ConfigureAwait(false);

            if (ServerError != null && ServerError.IsValid)
            {
                result.ServerError = Convert.ToInt32(ServerError.Total);
            }

            var APICount = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
           .Query(c => c.HasDateWhere(option.StartTime, option.EndTime) && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes)))
            .Aggregations(c => c.Cardinality("url", t => t.Field(e => e.Url)))
           .Size(0)
           ).ConfigureAwait(false);

            if (APICount != null && APICount.IsValid)
            {
                if (APICount.Aggregations.Count > 0)
                {
                    var model = (APICount.Aggregations.FirstOrDefault().Value as Nest.ValueAggregate);

                    result.APICount = model.Value.HasValue ? model.Value.Value.ToInt() : 0;
                }
            }

            var Avg = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
           .Query(c => c.HasDateWhere(option.StartTime, option.EndTime) && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes)))
            .Aggregations(c => c.Average("average", d => d.Field(e => e.Milliseconds)))
            .Size(0)
           ).ConfigureAwait(false);

            if (Avg != null && Avg.IsValid)
            {
                if (Avg.Aggregations.Count > 0)
                {
                    var model = (Avg.Aggregations.FirstOrDefault().Value as Nest.ValueAggregate);

                    result.AvgResponseTime = model.Value.HasValue ? model.Value.Value.ToInt() : 0;
                }
            }

            result.ErrorPercent = result.Total == 0 ? 0 : Convert.ToDouble(result.ServerError) / Convert.ToDouble(result.Total);

            return result;
        }

        public async Task<IMonitorJob> GetMonitorJob(string Id)
        {
            var response = await Client.SearchAsync<MonitorJob>(x => x.Index(GetIndexName<MonitorJob>()).Query(a => a.Term(b => b.Id, Id))).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return response.Documents.FirstOrDefault();
            }
            else
            {
                return new MonitorJob();
            }
        }

        public async Task<List<IMonitorJob>> GetMonitorJobs()
        {
            List<IMonitorJob> jobs = new List<IMonitorJob>();

            var response = await Client.SearchAsync<MonitorJob>(s => s.Index(GetIndexName<MonitorJob>()).MatchAll().Sort(a => a.Descending(b => b.CreateTime))).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                jobs = response.Documents.Select(x => x as IMonitorJob).ToList();
            }

            return jobs;
        }

        public async Task<List<NodeInfo>> GetNodesAsync()
        {
            List<NodeInfo> nodes = new List<NodeInfo>();

            var response = await Client.SearchAsync<RequestInfo>(x =>

              x.Index(GetIndexName<RequestInfo>()).Collapse(c =>

                c.Field(g => g.Node).MaxConcurrentGroupSearches(1000)

              )

            ).ConfigureAwait(false);

            if (response != null && response.Documents != null)
            {
                nodes = response.Documents.ToList().Select(x => new NodeInfo { Name = x.Node }).ToList();
            }

            return nodes;
        }

        public async Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption option)
        {
            List<RequestAvgResponeTime> requestAvgs = new List<RequestAvgResponeTime>();

            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>()).Query(c =>

            c.HasDateWhere(option.StartTime, option.EndTime)

            && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

            && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

            ).Aggregations(c => c.Terms("url", cc => cc.Field("url").Order(d => option.IsAscend ? d.Ascending("Milliseconds") : d.Descending("Milliseconds")).Aggregations(h =>

              h.Average("Milliseconds", d => d.Field(e => e.Milliseconds))

              ).Size(option.Take))).Size(0)

            ).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.KeyedBucket<object>;

                        requestAvgs.Add(new RequestAvgResponeTime
                        {

                            Url = model.Key.ToString(),
                            Time = Convert.ToSingle(model.ValueCount("Milliseconds").Value.Value)

                        });

                    }

                }

            }

            return requestAvgs;

        }

        public async Task<int> GetRequestCountAsync(RequestCountFilterOption option)
        {
            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

           .Query(c =>

            c.HasDateWhere(option.StartTime, option.EndTime)

           && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

           && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

           ).Size(0)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return Convert.ToInt32(response.Total);
            }

            return 0;
        }

        public async Task<(int Max, int All)> GetRequestCountWithWhiteListAsync(RequestCountWithListFilterOption option)
        {
            int All = 0; int Max = 0; 

            var TotalResponse = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

           .Query(c => c.HasDateWhere(option.StartTime, option.EndTime)

           && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

           && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

           && c.Bool(f => f.MustNot( 

               e => e.Terms(m => m.Field(n => n.IP).Terms(option.List))

             ))

           ).Size(0)).ConfigureAwait(false);

            if (TotalResponse != null && TotalResponse.IsValid)
            {
                All = Convert.ToInt32(TotalResponse.Total);
            }

            var MaxResponse = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
           .Query(d =>

               d.HasDateWhere(option.StartTime, option.EndTime)
               && d.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
               && d.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))
               && d.Bool(f => f.MustNot(  e => e.Terms(m => m.Field(n => n.IP).Terms(option.List))  )) 

             ).Aggregations(b =>

              b.Terms("ip", c => c.Field(e => e.IP).Order(e => e.CountDescending()).Size(1))

           ).Size(0)).ConfigureAwait(false);

            if (MaxResponse != null && MaxResponse.IsValid)
            {
                if (MaxResponse.Aggregations.Count > 0)
                {
                    var bucket = (MaxResponse.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.FirstOrDefault() as Nest.KeyedBucket<object>;

                    Max = Convert.ToInt32(bucket.DocCount.Value);

                }
            }

            return (Max, All);
        }

        public async Task<RequestTimesStatisticsResult> GetRequestTimesStatisticsAsync(TimeSpanStatisticsFilterOption option)
        {
            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

            .Query(c =>

             c.HasDateWhere(option.StartTime, option.EndTime)

            && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

            && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

            )

            .Aggregations(a => a.DateHistogram("date", b => b.Field(c => c.CreateTime).AutoFormatTime(option.Type).MinimumDocumentCount(0)
            .ExtendedBounds(option.StartTime.Value, option.EndTime.Value)
             .TimeZone("+08:00").Order(HistogramOrder.KeyAscending)
            )).Size(0)

            ).ConfigureAwait(false);

            var result = new RequestTimesStatisticsResult()
            {
                Type = option.Type,
                Items = new Dictionary<string, int>()
            };

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.DateHistogramBucket;

                        if (!result.Items.ContainsKey(model.KeyAsString.ToInt().ToString()))
                        {
                            result.Items.Add(model.KeyAsString.ToInt().ToString(), Convert.ToInt32(model.DocCount.Value));
                        }
                    }
                }

            }

            return result;
        }

        public async Task<ResponseTimeStatisticsResult> GetResponseTimeStatisticsAsync(TimeSpanStatisticsFilterOption option)
        {
            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

           .Query(c =>

            c.HasDateWhere(option.StartTime, option.EndTime)

           && c.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

           && c.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))

           )

           .Aggregations(a => a.DateHistogram("date", b => b.Field(c => c.CreateTime).AutoFormatTime(option.Type).MinimumDocumentCount(0)
           .ExtendedBounds(option.StartTime.Value, option.EndTime.Value)
               .TimeZone("+08:00").Order(HistogramOrder.KeyAscending)
               .Aggregations(m => m.Average("Average", n => n.Field(k => k.Milliseconds)))
           )).Size(0)

           ).ConfigureAwait(false);

            var result = new ResponseTimeStatisticsResult
            {

                Type = option.Type,
                Items = new Dictionary<string, int>()

            };

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.DateHistogramBucket;

                        if (!result.Items.ContainsKey(model.KeyAsString.ToInt().ToString()))
                        {
                            result.Items.Add(model.KeyAsString.ToInt().ToString(), Convert.ToInt32(model.ValueCount("Average").Value == null ? 0 : model.ValueCount("Average").Value.Value));
                        }

                    }
                }

            }

            return result;
        }

        public async Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption option)
        {
            List<StatusCodeCount> statusCodes = new List<StatusCodeCount>();

            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
           .Query(d =>

               d.HasDateWhere(option.StartTime, option.EndTime)
               && d.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
               && d.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

             ).Aggregations(b =>

              b.Terms("statusCode", c => c.Field("statusCode"))

           ).Size(0)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.KeyedBucket<object>;

                        statusCodes.Add(new StatusCodeCount
                        {

                            Code = model.Key.ToString().ToInt(),
                            Total = Convert.ToInt32(model.DocCount.Value)

                        });

                    }

                }

            }

            foreach (var item in option.StatusCodes)
            {
                var k = statusCodes.Where(x => x.Code == item).FirstOrDefault();

                if (k == null)
                {
                    statusCodes.Add(new StatusCodeCount
                    {
                        Code = item,
                        Total = 0
                    });
                }
            }

            return statusCodes;
        }

        public async Task<SysUser> GetSysUser(string UserName)
        {
            var response = await Client.SearchAsync<SysUser>(x => x.Index(GetIndexName<SysUser>()).Query(d => d.Term(e => e.UserName, UserName))).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return response.Documents.FirstOrDefault();
            }

            return new SysUser();
        }

        public async Task<int> GetTimeoutResponeCountAsync(RequestCountFilterOption option, int timeoutThreshold)
        {
            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
          .Query(d =>

              d.HasDateWhere(option.StartTime, option.EndTime)
              && d.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
              && d.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))
              && d.Range(f => f.Field(e => e.Milliseconds).GreaterThanOrEquals(timeoutThreshold))

            ).Size(0)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return Convert.ToInt32(response.Total);
            }

            return 0;
        }

        public async Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption option)
        {
            List<UrlRequestCount> urlRequests = new List<UrlRequestCount>();

            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())
            .Query(d =>

                d.HasDateWhere(option.StartTime, option.EndTime)
                && d.Terms(f => f.Field(e => e.Node).Terms(option.Nodes))
                && d.Terms(f => f.Field(e => e.StatusCode).Terms(option.StatusCodes))

              ).Aggregations(b =>

               b.Terms("url", c => c.Field("url").Order(d => option.IsAscend ? d.CountAscending() : d.CountDescending()).Size(option.Take))

            ).Size(0)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                if (response.Aggregations.Count > 0)
                {
                    var buckets = (response.Aggregations.FirstOrDefault().Value as Nest.BucketAggregate).Items.ToList();

                    foreach (var item in buckets)
                    {
                        var model = item as Nest.KeyedBucket<object>;

                        urlRequests.Add(new UrlRequestCount
                        {
                            Url = model.Key.ToString(),
                            Total = Convert.ToInt32(model.DocCount.Value)
                        });
                    }

                }

            }

            return urlRequests;
        }

        private string GetIndexName<T>()
        {
            string Index = (BasicConfig.ElasticSearchIndexName + typeof(T).Name).ToLower();
            return Index;
        }


        public async Task InitAsync()
        {
            try
            {
                IIndexState indexState = new IndexState()
                {
                    Settings = new IndexSettings()
                    {
                        NumberOfReplicas = 1,
                        NumberOfShards = 3
                    }
                };


                var RequestInfoIndex = await Client.Indices.ExistsAsync(GetIndexName<RequestInfo>());

                if (!RequestInfoIndex.Exists)
                { 
                    await Client.Indices.CreateAsync(GetIndexName<RequestInfo>(), a => a.InitializeUsing(indexState));

                    await Client.MapAsync<Models.RequestInfo>(c => c.Index(GetIndexName<RequestInfo>()).AutoMap());  
                }


                var RequestDetailIndex = await Client.Indices.ExistsAsync(GetIndexName<RequestDetail>());

                if (!RequestDetailIndex.Exists)
                {
                    await Client.Indices.CreateAsync(GetIndexName<RequestDetail>(), a => a.InitializeUsing(indexState));

                    await Client.MapAsync<Models.RequestDetail>(c => c.Index(GetIndexName<RequestDetail>()).AutoMap());
                } 


                var MonitorJobIndex = await Client.Indices.ExistsAsync(GetIndexName<MonitorJob>());

                if (!MonitorJobIndex.Exists)
                {
                    await Client.Indices.CreateAsync(GetIndexName<MonitorJob>(), a => a.InitializeUsing(indexState));

                    await Client.MapAsync<Models.MonitorJob>(c => c.Index(GetIndexName<MonitorJob>()).AutoMap());
                }


                var SysUserIndex = await Client.Indices.ExistsAsync(GetIndexName<SysUser>());

                if (!SysUserIndex.Exists)
                {
                    await Client.Indices.CreateAsync(GetIndexName<SysUser>(), a => a.InitializeUsing(indexState));

                    await Client.MapAsync<Models.SysUser>(c => c.Index(GetIndexName<SysUser>()).AutoMap());
                } 

                var user = await Client.SearchAsync<SysUser>(s => s.Index(GetIndexName<SysUser>()).Query(q => q.MatchAll())).ConfigureAwait(false);

                if (user.Documents.Count == 0)
                {
                    await Client.IndexAsync<SysUser>(new SysUser
                    {
                        Id = MD5_16(Guid.NewGuid().ToString()),
                        UserName = Core.Config.BasicConfig.DefaultUserName,
                        Password = Core.Config.BasicConfig.DefaultPassword

                    }, x => x.Index(GetIndexName<SysUser>()));
                } 
               
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RequestInfoSearchResult> SearchRequestInfoAsync(RequestInfoSearchFilterOption option)
        {
            var response = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>())

            .Query(d =>

             d.HasDateWhere(option.StartTime, option.EndTime)

             && d.Term(e => e.IP, option.IP)

             && d.Bool(e=>e.Filter(c=>c.Wildcard("url", $"*{option.Url}*")))

            ).From(option.Skip).Size(option.Take).Sort(c => c.Descending(e => e.CreateTime))

            ).ConfigureAwait(false);

            RequestInfoSearchResult result = new RequestInfoSearchResult();

            if (response != null && response.IsValid)
            {
                result.List = response.Documents.Select(x => x as IRequestInfo).ToList();

                result.List.ForEach(x =>
                {

                    var k = x.CreateTime;

                    x.CreateTime = new DateTime(k.Year, k.Month, k.Day, k.Hour, k.Minute, k.Second, DateTimeKind.Unspecified);

                });

                result.AllItemCount = Convert.ToInt32(response.Total);
            }

            return result;
        }

        public async Task<bool> UpdateLoginUser(SysUser model)
        {
            var response = await Client.IndexAsync<SysUser>(model, a => a.Index(GetIndexName<SysUser>()).Id(model.Id)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateMonitorJob(IMonitorJob job)
        {
            var response = await Client.IndexAsync<MonitorJob>(job as MonitorJob, a => a.Index(GetIndexName<MonitorJob>()).Id(job.Id)).ConfigureAwait(false);

            if (response != null && response.IsValid)
            {
                return true;
            }

            return false;
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public async Task<(IRequestInfo, IRequestDetail)> GetRequestInfoDetail(string Id)
        {
            IRequestInfo request = new RequestInfo();
            IRequestDetail detail = new RequestDetail();

            var requestResponse = await Client.SearchAsync<RequestInfo>(x => x.Index(GetIndexName<RequestInfo>()).Query(a => a.Term(b => b.Id, Id))).ConfigureAwait(false);

            if (requestResponse != null && requestResponse.IsValid)
            {
                request = requestResponse.Documents.FirstOrDefault();
            }


            var detailResponse = await Client.SearchAsync<RequestDetail>(x => x.Index(GetIndexName<RequestDetail>()).Query(a => a.Term(b => b.RequestId, Id))).ConfigureAwait(false);

            if (detailResponse != null && detailResponse.IsValid)
            {
                detail = detailResponse.Documents.FirstOrDefault();
            } 

            return (request,detail); 

        }
    }
}
