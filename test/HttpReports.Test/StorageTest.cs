using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpReports.Test
{
    [TestClass]
    public abstract class StorageTest<T> where T : IHttpReportsStorage
    {
        public abstract T Storage { get; }

        [TestInitialize]
        public abstract Task Init();  
      

        [TestMethod]
        public async Task InsertRequestInfoTestAsync()
        {
            for (int day = 30; day >= 0; day = day > 3 ? day - 7 : day - 1)
            {
                var createTime = DateTime.Now.AddDays(-day);
                if (day > 3 && day < 7)
                {
                    day = 9;
                }
                for (int codeIndex = 0; codeIndex < 15; codeIndex++)
                {
                    var statusCode = (codeIndex % 13) switch
                    {
                        1 => 301,
                        2 => 500,
                        3 => 302,
                        4 => 404,
                        _ => 200,
                    };

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
                                    var url = (urlIndex % 5) switch
                                    {
                                        1 => "api/test1",
                                        2 => "api/test2",
                                        3 => "api/test3",
                                        4 => "api/test4",
                                        _ => "api/test",
                                    };
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

            await Task.Delay(1_500);
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