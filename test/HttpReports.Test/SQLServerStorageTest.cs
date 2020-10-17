using System;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.SQLServer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                o.ConnectionString = "Max Pool Size = 512;server=localhost;uid=sa;pwd=123456;database=HttpReports;Connection Timeout=900;";
                o.DeferSecond = 5;
                o.DeferThreshold = 5;
            });
            services.AddSingleton<SQLServerStorage>(); 

            _storage = services.BuildServiceProvider().GetRequiredService<SQLServerStorage>();
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