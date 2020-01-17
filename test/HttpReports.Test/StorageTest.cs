using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HttpReports.Monitor;
using HttpReports.Storage.FilterOptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MySqlRequestInfo = HttpReports.Storage.MySql.RequestInfo;

namespace HttpReports.Test
{
    [TestClass]
    public abstract class StorageTest<T> where T : IHttpReportsStorage
    {
        public abstract T Storage { get; }

        [TestInitialize]
        public abstract Task Init();

        [TestMethod]
        public async Task MonitorRuleOperateTestAsync()
        {
            var allRule = await Storage.GetAllMonitorRulesAsync();

            foreach (var rule in allRule)
            {
                await Storage.DeleteMonitorRuleAsync(rule.Id);
            }

            allRule = await Storage.GetAllMonitorRulesAsync();

            //清空规则
            Assert.AreEqual(0, allRule.Count);

            //添加规则
            for (int i = 0; i < 10; i++)
            {
                var monitors = new List<IMonitor>()
                {
                    new ResponseTimeOutMonitor()
                    {
                        TimeoutThreshold = 1000 + i * 100,
                        WarningPercentage = 10,
                        Description = "ResponseTimeOutMonitor" + i,
                        CronExpression = "CronExpression" + i
                    },
                    new RemoteAddressRequestTimesMonitor()
                    {
                        WhileList = new string[] { "127.0.0.1", "192.168.0.120" },
                        WarningPercentage = 10,
                        Description = "RemoteAddressRequestTimesMonitor" + i,
                        CronExpression = "CronExpression" + i
                    }
                };

                var nodes = new string[] { "Node1", "Node2", "Node" };

                var addRule = new MonitorRule()
                {
                    Title = "测试规则" + i,
                    Description = "测试规则描述" + i,
                    Nodes = nodes,
                    Monitors = monitors,
                };

                await Storage.AddMonitorRuleAsync(addRule);
            }

            allRule = await Storage.GetAllMonitorRulesAsync();

            Assert.AreEqual(10, allRule.Count);

            var tasks = allRule.Select(async m =>
            {
                Assert.AreEqual(3, m.Nodes.Count);
                Assert.AreEqual(2, m.Monitors.Count);

                var rule = await Storage.GetMonitorRuleAsync(m.Id);

                Assert.AreEqual(m.Description, rule.Description);
                Assert.AreEqual(m.Id, rule.Id);
                Assert.AreEqual(m.Title, rule.Title);
                Assert.IsTrue(rule.Monitors.Count > 0);
                Assert.AreEqual(m.Monitors.Count, rule.Monitors.Count);
                Assert.AreEqual(m.Nodes.Count, rule.Nodes.Count);
                Assert.AreEqual(0, rule.Nodes.Except(m.Nodes).Count());
                //

                //移除监控
                rule.Monitors.Where(m => m is ResponseTimeOutMonitor).Select(m => m as ResponseTimeOutMonitor).ToList().ForEach(m => rule.Monitors.Remove(m));

                var newMonitor = new ResponseTimeOutMonitor()
                {
                    TimeoutThreshold = 1000,
                    WarningPercentage = 21,
                    Description = "ResponseTimeOutMonitor",
                    CronExpression = "CronExpression"
                };

                //新增监控
                rule.Monitors.Add(newMonitor);

                await Storage.UpdateMonitorRuleAsync(rule);
                rule = await Storage.GetMonitorRuleAsync(m.Id);

                var addedMonitor = rule.Monitors.Where(m => m is ResponseTimeOutMonitor).Select(m => m as ResponseTimeOutMonitor).First();

                Assert.AreEqual(newMonitor.CronExpression, addedMonitor.CronExpression);
                Assert.AreEqual(newMonitor.Description, addedMonitor.Description);
                Assert.AreEqual(newMonitor.TimeoutThreshold, addedMonitor.TimeoutThreshold);
                Assert.AreEqual(newMonitor.WarningPercentage, addedMonitor.WarningPercentage);
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        [TestMethod]
        public async Task InsertRequestInfoTestAsync()
        {
            for (int day = 30; day >= 0; day = day > 3 ? day - 7 : day - 1)
            {
                var createTime = DateTime.Now.AddDays(-day);
                if (day > 3 && day < 7)
                {
                    day = 9;
                }
                for (int codeIndex = 0; codeIndex < 15; codeIndex++)
                {
                    var statusCode = (codeIndex % 13) switch
                    {
                        1 => 301,
                        2 => 500,
                        3 => 302,
                        4 => 404,
                        _ => 200,
                    };

                    for (int timeIndex = 0; timeIndex < 8; timeIndex++)
                    {
                        var milliseconds = 20 + timeIndex * 100;

                        for (int ipIndex = 1; ipIndex < 5; ipIndex++)
                        {
                            var ip = "127.0.0." + ipIndex;
                            for (int nodeIndex = 1; nodeIndex < 4; nodeIndex++)
                            {
                                var node = "Test" + nodeIndex;

                                for (int urlIndex = 1; urlIndex < 7; urlIndex++)
                                {
                                    var url = (urlIndex % 5) switch
                                    {
                                        1 => "api/test1",
                                        2 => "api/test2",
                                        3 => "api/test3",
                                        4 => "api/test4",
                                        _ => "api/test",
                                    };
                                    await Storage.AddRequestInfoAsync(new MySqlRequestInfo()
                                    {
                                        CreateTime = createTime,
                                        IP = ip,
                                        Method = "GET",
                                        Milliseconds = milliseconds,
                                        StatusCode = statusCode,
                                        Node = node,
                                        Route = "API",
                                        Url = url
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task MonitorQueryTestAsync()
        {
            var requestCount = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
            });

            Assert.IsTrue(requestCount > 0);

            var requestCountWithCode = await Storage.GetRequestCountAsync(new RequestCountFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                StatusCodes = new[] { 200, 301, 302 }
            });

            Assert.IsTrue(requestCount > requestCountWithCode);

            var requestCountWithList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                List = new[] { "127.0.0.1" },
                InList = true,
            });

            Assert.IsTrue(requestCountWithList.All > 0);
            Assert.IsTrue(requestCountWithList.All >= requestCountWithList.Max);

            var requestCountWithOutList = await Storage.GetRequestCountWithWhiteListAsync(new RequestCountWithListFilterOption()
            {
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                Nodes = new string[] { "Test1", "Test2" },
                List = new[] { "127.0.0.1" },
                InList = false,
            });

            Assert.IsTrue(requestCountWithOutList.All > 0);
            Assert.IsTrue(requestCountWithOutList.All >= requestCountWithList.Max);
        }
    }
}