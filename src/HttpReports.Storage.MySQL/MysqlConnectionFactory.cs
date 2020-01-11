using System.Data;

using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace HttpReports.Storage.MySql
{
    internal class MySqlConnectionFactory
    {
        public MySqlStorageOptions Options { get; }

        public MySqlConnectionFactory(IOptions<MySqlStorageOptions> options)
        {
            Options = options.Value;
        }

        public IDbConnection GetConnection() => new MySqlConnection(Options.ConnectionString);
    }
}