using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Storage.Abstractions;
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
        public Task InsertTestAsync()
        {
            var startTime = DateTime.Now.AddSeconds(-1);
            var count = 100000;
            var random = new Random();

            string[] Services = { "User", "User", "User", "User", "User", "Order", "Order", "Order", "Order", "Weixin", "Weixin","Payment", "Log", "Log","Log", "DataCenter", "Student", "Student", "Master" };
            string[] LoginUsers = { "Jack", "Blues", "Mark", "Tom", "Cat" };
            string[] ParentServices = { "User", "Order", "Weixin", "Payment", "Log", "DataCenter" };
            string[] LocalIPs = { "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.4", "192.168.1.5", "192.168.1.6" };

            string[] Route = { "Login", "Payment", "Search", "QueryData", "GetCofnig", "LoadData" };

            int[] LocalPort = { 8801, 8802, 8803, 8804, 8805, 8806 };

            Insert();

            return Task.CompletedTask;

            void Insert()
            {
                for (int i = 0; i < 10000000; i++)
                {
                    List<Core.RequestBag> requestBags = new List<Core.RequestBag>();

                    for (int c = 0; c < 100; c++)
                    {
                        var _Service = Services[new Random().Next(0, Services.Length - 1)];
                        var _ParentService = ParentServices[new Random().Next(0, ParentServices.Length - 1)];
                        var _url = Services[new Random().Next(0, Services.Length - 1)] + "/" + Route[new Random().Next(0, Route.Length - 1)];

                        if (_ParentService == _Service) _ParentService = string.Empty;

                        requestBags.Add(new Core.RequestBag(new RequestInfo
                        {
                            Id = MD5_16(Guid.NewGuid().ToString()),
                            ParentId = "",
                            Service = _Service,
                            ParentService = _ParentService,
                            Route = _url,
                            Url = _url,
                            RequestType = "http",
                            Method = "POST",
                            LoginUser = LoginUsers[new Random().Next(0, LoginUsers.Length - 1)],

                            Milliseconds = _Service switch {  

                                "User" => new Random().Next(1400,1600),
                                "Order" => new Random().Next(1200, 1600),
                                "Weixin" => new Random().Next(600, 1600),
                                "Log" => new Random().Next(100, 500), 
                                "Payment" => new Random().Next(100, 800),
                                _ => new Random().Next(1, 1600)

                            },
                            StatusCode = _Service switch {

                                "User" => new Random().Next(1, 10) > 1 ? 200 : 500,
                                "Order" => new Random().Next(1, 10) > 3 ? 200 : 500,
                                "Weixin" => new Random().Next(1, 10) > 7 ? 200 : 500,
                                "Log" => new Random().Next(1, 10) > 6 ? 200 : 500,
                                "Payment" => new Random().Next(1, 10) > 4 ? 200 : 500,
                                _ => new Random().Next(1, 10) > 5 ? 200 : 500 

                            }, 
                            RemoteIP = "192.168.1.1",
                            Instance = LocalIPs[new Random().Next(0, LocalIPs.Length - 1)] + ":" + LocalPort[new Random().Next(0, LocalPort.Length - 1)],
                            CreateTime = DateTime.Now

                        }, null));

                    }

                    Storage.AddRequestInfoAsync(requestBags, System.Threading.CancellationToken.None).Wait();

                    Debug.WriteLine(i * 100);

                    Task.Delay(new Random().Next(1000, 5000)).Wait();

                }

            }

        }


        [TestMethod]
        public void TraceTest()
        {
            while (true)
            {
                int times = new Random().Next(1,10);

                List<RequestBag> bags = new List<RequestBag>();

                for (int i = 0; i < times; i++)
                {
                    var info = GetRandomRequestInfo();

                    bags.Add(new RequestBag(info, null));

                }

                for (int i = 0; i < times; i++)
                {
                    if (i == 0)
                    {
                        bags[0].RequestInfo.ParentService = "";
                        bags[0].RequestInfo.ParentId = "";
                    }
                    else
                    {
                        bags[i].RequestInfo.ParentService = bags[i - 1].RequestInfo.Service;
                        bags[i].RequestInfo.ParentId = bags[i - 1].RequestInfo.Id;
                    }
                }

                int cost = 0;
                bags.Reverse();

                var current = DateTime.Now;

                foreach (var item in bags)
                { 
                    cost = cost + new Random().Next(100, 3000);  
                    item.RequestInfo.Milliseconds = cost;
                    item.RequestInfo.CreateTime = current = current.AddMilliseconds(-cost);  
                }


                Storage.AddRequestInfoAsync(bags, System.Threading.CancellationToken.None).Wait();

                Task.Delay(new Random().Next(5000, 10000)).Wait();

            } 

        }

        private RequestInfo GetRandomRequestInfo()
        {

            string[] Services = { "User", "Order", "Weixin", "Payment", "Log", "DataCenter", "Student", "Master" };
            string[] LoginUsers = { "Jack", "Blues", "Mark", "Tom", "Cat" };
            string[] ParentServices = { "User", "Order", "Weixin", "Payment", "Log", "DataCenter" };
            string[] LocalIPs = { "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.4", "192.168.1.5", "192.168.1.6" };

            string[] Route = { "Login", "Payment", "Search", "QueryData", "GetCofnig", "LoadData" };

            int[] LocalPort = { 8801, 8802, 8803, 8804, 8805, 8806 };

            var _Service = Services[new Random().Next(0, Services.Length - 1)];
            var _ParentService = ParentServices[new Random().Next(0, ParentServices.Length - 1)];

            if (_ParentService == _Service) _ParentService = string.Empty;

            var route = Services[new Random().Next(0, Services.Length - 1)] + "/" + Route[new Random().Next(0, Route.Length - 1)]; 

            RequestInfo info = new RequestInfo
            { 
                Id = MD5_16(Guid.NewGuid().ToString()),
                ParentId = MD5_16(Guid.NewGuid().ToString()),
                Service = _Service,
                ParentService = _ParentService,
                Route = route,
                Url = route,
                RequestType = "http",
                Method = "POST",
                LoginUser = LoginUsers[new Random().Next(0, LoginUsers.Length - 1)],
                Milliseconds = new Random().Next(1, 2000),
                StatusCode = new Random().Next(1, 10) > 3 ? 200 : 500,
                RemoteIP = "192.168.1.1",
                Instance = LocalIPs[new Random().Next(0, LocalIPs.Length - 1)] + ":" + LocalPort[new Random().Next(0, LocalPort.Length - 1)],
                CreateTime = DateTime.Now 
            };

            return info; 

        } 

         
        private static string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }
    }
}