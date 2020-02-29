using System.Threading.Tasks;

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
                o.ConnectionString = "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;";
            });
            services.AddTransient<SQLServerStorage>();
            services.AddSingleton<SQLServerConnectionFactory>();

            _storage = services.BuildServiceProvider().GetRequiredService<SQLServerStorage>();
            await _storage.InitAsync();
        }
    }
}