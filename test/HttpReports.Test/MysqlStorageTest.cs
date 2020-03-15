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
                o.ConnectionString = "Data Source=127.0.0.1;Initial Catalog=HttpReports;User ID=test;Password=test;charset=utf8;SslMode=none;";
                o.DeferSecond = 3;
                o.DeferThreshold = 5;
                o.EnableDefer = true;
            });
            services.AddTransient<MySqlStorage>();
            services.AddSingleton<MySqlConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<MySqlStorage>();
            await _storage.InitAsync();
        }
    }
}