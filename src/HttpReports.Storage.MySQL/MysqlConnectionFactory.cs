using System.Data;

using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace HttpReports.Storage.MySql
{
    internal class MySqlConnectionFactory
    {
        public MySqlStorageOptions Options { get; }

        public string DataBase { get; }

        public MySqlConnectionFactory(IOptions<MySqlStorageOptions> options)
        {
            Options = options.Value;
            DataBase = new MySqlConnectionStringBuilder(Options.ConnectionString).Database;
        }

        public IDbConnection GetConnection() => new MySqlConnection(Options.ConnectionString);

        public IDbConnection GetConnectionWithoutDefaultDatabase() => GetConnectionWithoutDefaultDatabase(Options.ConnectionString);

        public IDbConnection GetConnectionWithoutDefaultDatabase(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(Options.ConnectionString)
            {
                Database = null,
            };

            return new MySqlConnection(builder.ToString());
        }
    }
}