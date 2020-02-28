using HttpReports.Storage.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Test
{
    [TestClass]
    public class PostgreSQLStorageTest : StorageTest<IHttpReportsStorage>
    {
        private PostgreSQLStorage _storage;

        public override IHttpReportsStorage Storage => _storage;

        [TestInitialize]
        public override async Task Init()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.Configure<PostgreStorageOptions>(o =>
            {
                o.ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=HttpReports;"; 
            });
            services.AddTransient<PostgreSQLStorage>();
            services.AddSingleton<PostgreConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<PostgreSQLStorage>();
            await _storage.InitAsync();
        }


        [TestMethod]
        public async Task Insert()
        {
            for (int i = 0; i < 99; i++)
            {
                RequestInfo request = new RequestInfo
                { 
                    CreateTime = new DateTime(2020,2,16,21,12,15,DateTimeKind.Local),
                    IP = "192.168.1.1",
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
