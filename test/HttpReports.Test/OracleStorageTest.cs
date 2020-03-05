using System.Threading.Tasks;

using HttpReports.Storage.Oracle;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}