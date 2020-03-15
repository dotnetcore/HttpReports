using System;
using System.Threading.Tasks;

using HttpReports.Storage.FilterOptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpReports.Test
{
    [TestClass]
    public abstract class StorageTest<T> where T : IHttpReportsStorage
    {
        public abstract T Storage { get; }

        protected virtual TimeSpan? DeferTime { get; set; } = null;

        [TestInitialize]
        public abstract Task Init();

        [TestMethod]
        public async Task InsertTestAsync()
        {
            var startTime = DateTime.Now.AddSeconds(-1);
            var count = 500;
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                RequestInfo request = new RequestInfo
                {
                    CreateTime = DateTime.Now,
                    IP = "192.168.1.1",
                    Method = "GET",
                    Node = "Log",
                    Milliseconds = new Random().Next(1, 9999),
                    Route = "/User/Login",
                    Url = "/User/Login/AAA",
                    StatusCode = 200
                };

                await Storage.AddRequestInfoAsync(request,new RequestDetail());

                await Task.Delay(random.Next(1, 5));
            }

            if (DeferTime.HasValue)
            {
                await Task.Delay(DeferTime.Value + TimeSpan.FromSeconds(1));
            }

            var requests = await Storage.SearchRequestInfoAsync(new RequestInfoSearchFilterOption()
            {
                StartTime = startTime,
                Take = count,
                EndTime = DateTime.Now.AddSeconds(1),
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss.fff",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss.fff",
                IsOrderByField = true,
                Field = RequestInfoFields.Id,
                IsAscend = true,
            });

            Assert.AreEqual(count, requests.AllItemCount);

            for (int i = 0; i < requests.AllItemCount - 1; i++)
            {
                if (requests.List[i].CreateTime >= requests.List[i + 1].CreateTime)
                {
                    Assert.Fail($"Time Error:Index-{i} Id-{requests.List[i].Id}");
                }
            }
        }

        [TestMethod]
        public async Task InsertRequestInfoTestAsync()
        {
            var count = 0;
            for (int day = 30; day >= 0; day = day > 3 ? day - 7 : day - 1)
            {
                var createTime = DateTime.Now.AddDays(-day);
                if (day > 3 && day < 7)
                {
                    day = 9;
                }
                for (int codeIndex = 0; codeIndex < 15; codeIndex++)
                {
                    var statusCode = 200;

                    switch (codeIndex % 13)
                    {
                        case 1: statusCode = 301; break;
                        case 2: statusCode = 500; break;
                        case 3: statusCode = 302; break;
                        case 4: statusCode = 404; break;
                        default: statusCode = 200; break;
                    }

                    for (int timeIndex = 0; timeIndex < 8; timeIndex++)
                    {
                        var milliseconds = 20 + timeIndex * 100;

                        for (int ipIndex = 1; ipIndex < 5; ipIndex++)
                        {
                            var ip = "127.0.0." + ipIndex;
                            for (int nodeIndex = 1; nodeIndex < 4; nodeIndex++)
                            {
                                var node = "Test" + nodeIndex;

                                for (int urlIndex = 1; urlIndex < 7; urlIndex++)
                                {
                                    var url = "api/test";

                                    switch (urlIndex % 5)
                                    {
                                        case 1: url = "api/test1"; break;
                                        case 2: url = "api/test2"; break;
                                        case 3: url = "api/test3"; break;
                                        case 4: url = "api/test4"; break;
                                        default: url = "api/test"; break;
                                    }

                                    count++;

                                    await Storage.AddRequestInfoAsync(new RequestInfo()
                                    {
                                        CreateTime = createTime,
                                        IP = ip,
                                        Method = "GET",
                                        Milliseconds = milliseconds,
                                        StatusCode = statusCode,
                                        Node = node,
                                        Route = "API",
                                        Url = url
                                    });
                                }
                            }
                        }
                    }
                }
            }

            await Task.Delay(3_500);

            Console.WriteLine(count);
        }

        [TestMethod]
        public async Task MonitorQueryTestAsync()
        {
            var requestCount = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
            });

            Assert.IsTrue(requestCount > 0);

            var requestCountWithCode = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                StatusCodes = new[] { 200, 301, 302 }
            });

            Assert.IsTrue(requestCount > requestCountWithCode);

            var requestCountWithList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                List = new[] { "127.0.0.1" },
                InList = true,
            });

            Assert.IsTrue(requestCountWithList.All > 0);
            Assert.IsTrue(requestCountWithList.All >= requestCountWithList.Max);

            var requestCountWithOutList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                List = new[] { "127.0.0.1" },
                InList = false,
            });

            Assert.IsTrue(requestCountWithOutList.All > 0);
            Assert.IsTrue(requestCountWithOutList.All >= requestCountWithList.Max);
        }
    }
}