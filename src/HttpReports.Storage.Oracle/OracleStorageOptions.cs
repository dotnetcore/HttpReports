using Microsoft.Extensions.Options;

namespace HttpReports.Storage.Oracle
{
    internal class OracleStorageOptions : IOptions<OracleStorageOptions>
    {
        public string ConnectionString { get; set; }

        public OracleStorageOptions Value => this;
    }
}