using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");

            var a = list.AsEnumerable().SingleOrDefault();

            Assert.IsTrue(a != null);
        }
    }
}
