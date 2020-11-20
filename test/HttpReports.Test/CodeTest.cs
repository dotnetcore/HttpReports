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

            var str1 = System.Text.Json.JsonSerializer.Serialize(user,new System.Text.Json.JsonSerializerOptions { 
            
                 PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                 Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)

            }); 
           
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
