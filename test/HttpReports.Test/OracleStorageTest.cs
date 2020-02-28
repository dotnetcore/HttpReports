using HttpReports.Storage.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Test
{
    [TestClass]
    public class OracleStorageTest : StorageTest<IHttpReportsStorage>
    {
        private OracleStorage _storage;

        public override IHttpReportsStorage Storage => _storage;

        [TestInitialize]
        public override async Task Init()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.Configure<OracleStorageOptions>(o =>
            {
                o.ConnectionString = "Password=Mm2717965346;User ID=sa;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));";
            });
            services.AddTransient<OracleStorage>();
            services.AddSingleton<OracleConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<OracleStorage>();
            await _storage.InitAsync();
        }


        [TestMethod]
        public async Task Insert()
        {
            for (int i = 0; i < 99; i++)
            {
                RequestInfo request = new RequestInfo
                {
                    CreateTime = new DateTime(2020, 2, 17, 11, 52, 15, DateTimeKind.Local),
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
