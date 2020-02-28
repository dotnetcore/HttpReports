using System.Data;
 

using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace HttpReports.Storage.Oracle
{
    public class OracleConnectionFactory
    {
        public OracleStorageOptions Options { get; } 

        public OracleConnectionFactory(IOptions<OracleStorageOptions> options)
        {
            Options = options.Value; 
        }

        public IDbConnection GetConnection() => new OracleConnection(Options.ConnectionString);
    }
}