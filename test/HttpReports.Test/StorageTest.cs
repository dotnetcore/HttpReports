using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HttpReports.Core;
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
        public Task InsertTestAsync()
        {
            var startTime = DateTime.Now.AddSeconds(-1);
            var count = 100000;
            var random = new Random();

            string[] Services = { "User", "SendOrder", "PostOrder", "Payment", "Log", "DataCenter", "Student","Master" };
            string[] LocalIPs = { "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.4", "192.168.1.5", "192.168.1.6" };

            string[] Route = { "Login","Payment","Search","QueryData","GetCofnig","LoadData" };

            int[] LocalPort = { 8801,8802,8803,8804,8805,8806};

            Insert();

            return Task.CompletedTask;

            void Insert()
            { 
                for (int i = 0; i < 10000000; i++)
                {
                    List<Core.RequestBag> requestBags = new List<Core.RequestBag>();

                    for (int c = 0; c < 100; c++)
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

                    Debug.WriteLine(i * 100);

                    Task.Delay(new Random().Next(1000,5000)).Wait();

                } 
               
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