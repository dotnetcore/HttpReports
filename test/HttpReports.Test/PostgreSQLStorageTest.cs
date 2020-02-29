using System.Threading.Tasks;

using HttpReports.Storage.PostgreSQL;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}