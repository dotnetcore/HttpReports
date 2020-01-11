using Microsoft.Extensions.Options;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorageOptions : IOptions<MySqlStorageOptions>
    {
        public string ConnectionString { get; set; }

        public MySqlStorageOptions Value => this;
    }
}