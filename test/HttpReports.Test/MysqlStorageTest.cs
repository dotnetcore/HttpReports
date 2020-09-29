using System;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
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
                o.ConnectionString = "DataBase=HttpReports;Data Source=localhost;User Id=root;Password=123456;";
                o.DeferSecond = 3;
                o.DeferThreshold = 5; 
            });
            services.AddTransient<MySqlStorage>(); 

            _storage = services.BuildServiceProvider().GetRequiredService<MySqlStorage>();
            await _storage.InitAsync();
        }



        [TestMethod]
        public new async Task GetRequestInfoDetail()
        {
            var ids = new[] { "0000329875d9c209", "000301f44e4e9524", "0005735c2c6240d5" };

            var id = ids[new Random().Next(0, ids.Length - 1)];

            var result = await Storage.GetRequestInfo(id);

            Assert.IsNotNull(result);

        }


    }
}