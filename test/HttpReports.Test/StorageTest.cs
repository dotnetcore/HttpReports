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