using System.Data;
using System.Data.SqlClient;

using Microsoft.Extensions.Options;

namespace HttpReports.Storage.SQLServer
{
    public class SQLServerConnectionFactory
    {
        public SQLServerStorageOptions Options { get; }

        public string DataBase { get; }

        public SQLServerConnectionFactory(IOptions<SQLServerStorageOptions> options)
        {
            Options = options.Value;
            DataBase = new SqlConnectionStringBuilder(Options.ConnectionString).InitialCatalog;
        }

        public IDbConnection GetConnection() => new SqlConnection(Options.ConnectionString);
    }
}