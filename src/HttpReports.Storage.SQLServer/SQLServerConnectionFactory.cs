using System.Data;
using System.Data.SqlClient;

using Microsoft.Extensions.Options;

namespace HttpReports.Storage.SQLServer
{
    internal class SQLServerConnectionFactory
    {
        public SQLServerStorageOptions Options { get; }

        public SQLServerConnectionFactory(IOptions<SQLServerStorageOptions> options)
        {
            Options = options.Value;
        }

        public IDbConnection GetConnection() => new SqlConnection(Options.ConnectionString);
    }
}