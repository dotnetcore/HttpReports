using System;
using System.Threading.Tasks;

using HttpReports.Storage.MySql;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpReports.Test
{
    [TestClass]
    public class MysqlStorageTest : StorageTest<IHttpReportsStorage>
    {
        private MySqlStorage _storage;

        public override IHttpReportsStorage Storage => _storage;

        [TestInitialize]
        public override async Task Init()
        { 
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging(); 
          
            services.Configure<MySqlStorageOptions>(o =>
            {
                o.ConnectionString = "DataBase=HttpReports;Data Source=localhost;User Id=root;Password=123456"; 
            });
            services.AddTransient<MySqlStorage>();
            services.AddSingleton<MySqlConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<MySqlStorage>();
            await _storage.InitAsync();
        }


        [TestMethod]
        public async Task Insert()
        {
            for (int i = 0; i < 99; i++)
            {
                RequestInfo request = new RequestInfo
                {
                    CreateTime = new DateTime(2020, 2, 17, 14, 24, 15, DateTimeKind.Local),
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