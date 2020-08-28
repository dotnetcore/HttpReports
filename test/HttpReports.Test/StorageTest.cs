using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using HttpReports.Storage.FilterOptions;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Asn1.IsisMtt;

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

            string[] Route = { "Login","Payment","Search","QueryData","GetCofnig","LoadData" };

            int[] LocalPort = { 8801,8802,8803,8804,8805,8806};

            _ = Task.Run(()=> { Insert(); });

            _ = Task.Run(() => { Insert(); });

            _ = Task.Run(() => { Insert(); });

            Insert();

            void Insert()
            {
                while (true)
                {
                    List<Core.RequestBag> requestBags = new List<Core.RequestBag>();

                    for (int c = 0; c < 200; c++)
                    {
                        requestBags.Add(new Core.RequestBag(new RequestInfo
                        {
                            Id = MD5_16(Guid.NewGuid().ToString()),
                            ParentId = MD5_16(Guid.NewGuid().ToString()),
                            Service = Services[new Random().Next(0, Services.Length - 1)],
                            Route =  Services[new Random().Next(0, Services.Length - 1)] + "/" + Route[new Random().Next(0, Route.Length - 1)],
                            Url = "/HttpReportsData/GetServiceInstance",
                            RequestType = "http",
                            Method = "POST",
                            Milliseconds = new Random().Next(1, 2000),
                            StatusCode = new Random().Next(1, 10) > 3 ? 200 : 500,
                            RemoteIP = "192.168.1.1", 
                            Instance = LocalIPs[new Random().Next(0, LocalIPs.Length - 1)] + ":" + LocalPort[new Random().Next(0, LocalPort.Length - 1)],
                            CreateTime = DateTime.Now

                        }, null));

                    }

                    Storage.AddRequestInfoAsync(requestBags, System.Threading.CancellationToken.None).Wait();
                } 
               
            }
                 
        }


        [TestMethod]
        public async Task IndexQuery()
        { 
            int times = 3;

            IndexPageDataFilterOption option = new IndexPageDataFilterOption
            {  
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now,
                StartTimeFormat = "yyyy-MM-dd HH:mm:ss",
                EndTimeFormat = "yyyy-MM-dd HH:mm:ss",
                Take = 6 

            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < times; i++)
            { 
                var basic = await Storage.GetIndexBasicDataAsync(option);
            }

            stopwatch.Stop();

            Console.WriteLine($"GetIndexBasicDataAsync:AVG {(stopwatch.ElapsedMilliseconds).ToString()}"); 

            stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < times; i++) {

                var top = await Storage.GetGroupData(option,Core.Storage.FilterOptions.GroupType.Instance); 
            }

            stopwatch.Stop();

            Console.WriteLine($"GetIndexTOPService:AVG {(stopwatch.ElapsedMilliseconds).ToString()}");  


            
            stopwatch = new Stopwatch();
            stopwatch.Start();

            var range =  GetTimeRange(option.StartTime.Value, option.EndTime.Value);

            for (int i = 0; i < times; i++)
            {
                var trend = await Storage.GetServiceTrend(option, range);
            } 

            stopwatch.Stop();
            Console.WriteLine($"GetServiceTrend:AVG {(stopwatch.ElapsedMilliseconds).ToString()}");



            stopwatch = new Stopwatch();
            stopwatch.Start(); 

            string[] span = { "0-200", "200-400", "400-600", "600-800", "800-1000", "1000-1200", "1200-1400", "1400-1600", "1600+" };

            for (int i = 0; i < times; i++)
            {
                var heatmap = await Storage.GetServiceHeatMap(option, range, span.ToList());
            } 

            stopwatch.Stop();
            Console.WriteLine($"GetServiceHeatMap:AVG {(stopwatch.ElapsedMilliseconds).ToString()}"); 
           
            Assert.IsTrue(true); 

            List<string> GetTimeRange(DateTime start, DateTime end)
            {
                List<string> Time = new List<string>();

                if ((end - start).TotalMinutes <= 60)
                {
                    while (start <= end)
                    {
                        Time.Add(start.ToString("HH:mm"));
                        start = start.AddMinutes(1);
                    }

                }
                else
                {
                    while (start <= end)
                    {
                        Time.Add(start.ToString("dd-HH"));
                        start = start.AddHours(1);
                    }
                }
                return Time;
            }

        }  


        private static string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }
    }
}