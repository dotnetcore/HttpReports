using Nest;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Storage.ElasticSearch
{
    public class MockData
    {
        private ElasticClient Client;

        public void Excute(ElasticClient client)
        {  
            System.Threading.Thread.Sleep(3000);

            Client = client;

            var pp = new DateTime(2020, 2, 15, 21, 52, 5, DateTimeKind.Local);

            for (int i = 0; i < 5; i++)
            {

                Add(new RequestInfo
                {
                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay44",
                    Method = "GET",
                    Milliseconds = new Random().Next(5, 4875),
                    StatusCode = 501,
                    IP = "192.168.1.0",
                    CreateTime = pp

                });
            }

            for (int i = 0; i < 5; i++)
            { 

                Add(new RequestInfo
                {
                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay44",
                    Method = "GET",
                    Milliseconds = new Random().Next(5,4875),
                    StatusCode = 501,
                    IP = "192.168.1.1",
                    CreateTime = pp

                });
            }

            for (int i = 0; i < 5; i++)
            {
                var aa = DateTime.Now;
                var bb = new DateTime(2020, 2, 15, 22, 30, 2, DateTimeKind.Local);

                Add(new RequestInfo
                {
                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay44",
                    Method = "GET",
                    Milliseconds = new Random().Next(5, 4875),
                    StatusCode = 501,
                    IP = "192.168.1.1",
                    CreateTime = pp

                });
            }


            for (int i = 0; i < 4; i++)
            {
                var aa = DateTime.Now;
                var bb = new DateTime(2020, 2, 15, 22, 30, 2, DateTimeKind.Local);

                Add(new RequestInfo
                {
                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay44",
                    Method = "GET",
                    Milliseconds = new Random().Next(5, 4875),
                    StatusCode = 502,
                    IP = "192.168.1.2",
                    CreateTime = pp

                });
            }


            for (int i = 0; i < 3; i++)
            {
                var aa = DateTime.Now;
                var bb = new DateTime(2020, 2, 15, 22, 30, 2, DateTimeKind.Local);

                Add(new RequestInfo
                {
                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay44",
                    Method = "GET",
                    Milliseconds = new Random().Next(5, 4875),
                    StatusCode = 200,
                    IP = "192.168.1.3",
                    CreateTime = pp

                });
            }

        }


        private void Add(RequestInfo request)
        {
            Client.Index<RequestInfo>(request, x => x.Index("httpreports_index_requestinfo"));
        }

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

    }
}
