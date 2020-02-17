using HttpReports.Storage.SQLServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Test
{
    [TestClass]
    public class SQLServerStorageTest : StorageTest<IHttpReportsStorage>
    {
        private SQLServerStorage _storage;

        public override IHttpReportsStorage Storage => _storage;

        [TestInitialize]
        public override async Task Init()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.Configure<SQLServerStorageOptions>(o =>
            {
                o.ConnectionString = "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;";
            });
            services.AddTransient<SQLServerStorage>();
            services.AddSingleton<SQLServerConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<SQLServerStorage>();
            await _storage.InitAsync();
        }


        [TestMethod]
        public async Task Insert()
        {
            for (int i = 0; i < 99; i++)
            {
                RequestInfo request = new RequestInfo
                {
                    CreateTime = new DateTime(2020, 2, 17, 14, 0, 15, DateTimeKind.Local),
                    IP = "192.168.2.1",
                    Method = "GET",
                    Node = "Log",
                    Milliseconds = new Random().Next(1, 9999),
                    Route = "/User/Login",
                    Url = "/User/Login/AAA",
                    StatusCode = 200
                };

                await _storage.AddRequestInfoAsync(request);

            }

            Assert.IsTrue(true);

        }

    }
}
