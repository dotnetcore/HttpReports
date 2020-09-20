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

    }
}
