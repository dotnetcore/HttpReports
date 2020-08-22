using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using HttpReports.Storage.FilterOptions;
using Microsoft.VisualBasic.CompilerServices;
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
            var count = 100000;
            var random = new Random();

            string[] Services = { "User", "SendOrder", "PostOrder", "Payment", "Log", "DataCenter", "Student","Master" };
            string[] LocalIPs = { "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.4", "192.168.1.5", "192.168.1.6" };
            int[] LocalPort = { 8801,8802,8803,8804,8805,8806};

            for (int i = 0; i < count; i++)
            {
                List<Core.RequestBag> requestBags = new List<Core.RequestBag>();

                for (int c = 0; c < 100; c++)
                {
                    requestBags.Add(new Core.RequestBag(new RequestInfo {

                        Id = MD5_16(Guid.NewGuid().ToString()),
                        ParentId = MD5_16(Guid.NewGuid().ToString()),
                        Node = Services[new Random().Next(0,Services.Length - 1)],
                        Route = "/httpreportsdata/getserviceinstance",
                        Url = "/HttpReportsData/GetServiceInstance",
                        RequestType = "http",
                        Method = "POST",
                        Milliseconds = new Random().Next(1,2000),
                        StatusCode = new Random().Next(1, 10) > 3 ? 200:500,
                        IP = "192.168.1.1",
                        Port = 80,
                        LocalIP = LocalIPs[new Random().Next(0,LocalIPs.Length - 1)],
                        LocalPort = LocalPort[new Random().Next(0, LocalPort.Length - 1)],
                        CreateTime = DateTime.Now 

                    },null)); 

                }   

                await Storage.AddRequestInfoAsync(requestBags,System.Threading.CancellationToken.None);

                await Task.Delay(new Random().Next(1000,5000));

                Debug.WriteLine(i.ToString());
            } 


           
        } 

        [TestMethod]
        public async Task MonitorQueryTestAsync()
        {
            var requestCount = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Service = "Test1"
            });

            Assert.IsTrue(requestCount > 0);

            var requestCountWithCode = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Service = "Test1",
                StatusCodes = new[] { 200, 301, 302 }
            });

            Assert.IsTrue(requestCount > requestCountWithCode);

            var requestCountWithList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Service = "Test1",
                List = new[] { "127.0.0.1" },
                InList = true,
            });

            Assert.IsTrue(requestCountWithList.All > 0);
            Assert.IsTrue(requestCountWithList.All >= requestCountWithList.Max);

            var requestCountWithOutList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Service = "Test1",
                List = new[] { "127.0.0.1" },
                InList = false,
            });

            Assert.IsTrue(requestCountWithOutList.All > 0);
            Assert.IsTrue(requestCountWithOutList.All >= requestCountWithList.Max);
        }


        private static string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }
    }
}