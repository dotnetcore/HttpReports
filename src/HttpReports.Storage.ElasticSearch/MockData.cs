using Nest;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;



namespace HttpReports.Storage.ElasticSearch
{
    public class MockData
    { 
        public void Excute(ElasticClient Client)
        {
            return;

            for (int i = 0; i < 3; i++)
            {
                Models.RequestInfo request = new Models.RequestInfo
                {

                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay3",
                    Route = "/tdfdf",
                    Url = "/api/Pay3",
                    Method = "GET",
                    Milliseconds = (i + 1) * 3,
                    StatusCode = 200,
                    IP = "231321",
                    CreateTime = DateTime.Now

                };

                Client.Index<Models.RequestInfo>(request, x => x.Index("httpreports_index_requestinfo"));

            }

            for (int i = 0; i < 5; i++)
            {
                Models.RequestInfo request = new Models.RequestInfo
                {

                    Id = MD5_16(Guid.NewGuid().ToString()),
                    Node = "Pay5",
                    Route = "/tdfdf",
                    Url = "/api/PayTest5",
                    Method = "GET",
                    Milliseconds = (i + 1) * 3,
                    StatusCode = 500,
                    IP = "231321",
                    CreateTime = DateTime.Now

                };

                Client.Index<Models.RequestInfo>(request, x => x.Index("httpreports_index_requestinfo"));

            }

            return;

            for (int i = 0; i < 7; i++)
            {

                RequestInfo request = new RequestInfo();

                request.Url = "/api/Test2";

                request.Id = MD5_16(Guid.NewGuid().ToString());

                Client.Index<RequestInfo>(request as RequestInfo, x => x.Index("httpreports_index_requestinfo"));

            }


            for (int i = 0; i < 6; i++)
            {

                RequestInfo request = new RequestInfo();

                request.Url = "/api/Test2";

                request.Id = MD5_16(Guid.NewGuid().ToString());

                Client.Index<RequestInfo>(request as RequestInfo, x => x.Index("httpreports_index_requestinfo"));

            }


        } 
        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }







    }
}
