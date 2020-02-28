using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HttpReports.Storage.PostgreSQL
{
    public class PostgreConnectionFactory
    {
        public PostgreStorageOptions Options { get; }

        public string DataBase { get; }

        public PostgreConnectionFactory(IOptions<PostgreStorageOptions> options)
        {
            Options = options.Value;
            DataBase = new NpgsqlConnectionStringBuilder(Options.ConnectionString).Database;
        }

        public IDbConnection GetConnection() => new NpgsqlConnection(Options.ConnectionString);

        public IDbConnection GetConnectionWithoutDefaultDatabase() => GetConnectionWithoutDefaultDatabase(Options.ConnectionString);

        public IDbConnection GetConnectionWithoutDefaultDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(Options.ConnectionString)
            {
                Database = null,
            };

            return new NpgsqlConnection(builder.ToString());
        } 

    }
}
