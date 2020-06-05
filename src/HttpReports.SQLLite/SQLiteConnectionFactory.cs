using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace HttpReports.Storage.SQLite
{
    public class SQLiteConnectionFactory
    { 
        public SQLiteStorageOptions Options { get; }

        public string DataBase { get; }

        public SQLiteConnectionFactory(IOptions<SQLiteStorageOptions> options)
        {
            Options = options.Value;
            DataBase = new  SQLiteConnectionStringBuilder(Options.ConnectionString).DataSource;
        }

        public IDbConnection GetConnection() => new SQLiteConnection(Options.ConnectionString);

        public IDbConnection GetConnectionWithoutDefaultDatabase() => new SQLiteConnection(Options.ConnectionString);  

    }
}
