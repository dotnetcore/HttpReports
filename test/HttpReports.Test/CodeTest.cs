using HttpReports.Collector.Grpc;
using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace HttpReports.Test
{
    [TestClass]
    public class CodeTest
    {

        [TestMethod]
        public void Test1()
        {
            string rule = "/httpreports%";

            var result = rule.Where(x => x == '%').ToList();

            Assert.IsTrue(result.Count() > 0);
        }


        [TestMethod]
        public void Test2()
        {
            string url = "http://dx.dabansuan.com.cn/click.htm?zid=3228&od=0";
            int str = url.IndexOf("od");
            url = url.Insert(str, "_au");
        } 
        

        [TestMethod]
        public void JsonTest()
        {
            var user = new { 
             
                FirstName = "张三",
                CreateTime = DateTime.Now

            }; 

            var model1 = new Core.Models.Performance() {

                Id = 1222222222243243232L,
                Instance = "",
                Service = "测试",
                CreateTime =DateTime.Now 
            };

            var model3 = new Collector.Grpc.Performance(); 

            var str2 = JsonSerializer.Serialize(model1);

            var model2 = JsonSerializer.Deserialize<Collector.Grpc.Performance>(str2);  

        }


        [TestMethod]
        public void JsonTest3()
        {
            List<RequestBag> bags = new List<RequestBag>();

            bags.Add(new RequestBag(new Core.Models.RequestInfo
            {

                Service = "11",
                Instance = "1111",
                CreateTime = DateTime.Now


            }, new Core.Models.RequestDetail
            {

                QueryString = "11111111",
                CreateTime = DateTime.Now

            }));

            bags.Add(new RequestBag(new Core.Models.RequestInfo
            {

                Service = "22",
                Instance = "2222",
                CreateTime = DateTime.Now


            }, new Core.Models.RequestDetail
            { 
                QueryString = "2222222",
                CreateTime = DateTime.Now

            })); 

            RequestInfoPack ccc = new RequestInfoPack(); 
          
            var str3 = JsonSerializer.Serialize(bags); 

            var pack = JsonSerializer.Deserialize<RequestInfoPack>(str3); 
        } 


        public delegate string GetResultDelegate();   

        public HttpConfig GetConfig(Action<HttpConfig> action)
        {
            HttpConfig con = new HttpConfig();

            action(con);

            return con; 

        } 
    } 
  

    public class HttpConfig
    {
        public string Name { get; set; } 
    }

}
