using Microsoft.Extensions.Options;

namespace HttpReports.Storage.SQLServer
{
    internal class SQLServerStorageOptions : IOptions<SQLServerStorageOptions>
    {
        public string ConnectionString { get; set; }

        public SQLServerStorageOptions Value => this;
    }
}