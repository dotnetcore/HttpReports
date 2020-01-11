using System.Linq;
using System.Threading.Tasks;

using Dapper;

using MySql.Data.MySqlClient;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : IHttpReportsStorage
    {
        public MySqlConnectionFactory ConnectionFactory { get; }

        public MySqlStorage(MySqlConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        public Task InitAsync()
        {
            using (var con = ConnectionFactory.GetConnection())
            {
                //CreateDataBaseMySql(ConnectionFactory.Options.ConnectionString);

                if (con.QueryFirstOrDefault<int>("  Select count(1) from information_schema.tables where table_name ='RequestInfo' and table_schema = 'HttpReports'; ") == 0)
                {
                    con.Execute(@"
                        CREATE TABLE `RequestInfo` (
                          `Id` int(11) NOT NULL auto_increment,
                          `Node` varchar(50) default NULL,
                          `Route` varchar(50) default NULL,
                          `Url` varchar(200) default NULL,
                          `Method` varchar(50) default NULL,
                          `Milliseconds` int(11) default NULL,
                          `StatusCode` int(11) default NULL,
                          `IP` varchar(50) default NULL,
                          `CreateTime` datetime default NULL,
                          PRIMARY KEY  (`Id`)
                        ) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;  ");
                }

                if (con.QueryFirstOrDefault<int>(" Select count(1) from information_schema.tables where table_name ='Job' and table_schema = 'HttpReports'; ") == 0)
                {
                    con.Execute(@"

                            CREATE TABLE `Job` (
                              `Id` int(11) NOT NULL AUTO_INCREMENT,
                              `Title` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `CronLike` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Emails` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Mobiles` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `Status` int(2) DEFAULT NULL,
                              `Servers` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `RtStatus` int(2) DEFAULT NULL,
                              `RtTime` int(11) DEFAULT NULL,
                              `RtRate` double DEFAULT NULL,
                              `HttpStatus` int(11) DEFAULT NULL,
                              `HttpCodes` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `HttpRate` double DEFAULT NULL,
                              `IPStatus` int(11) DEFAULT NULL,
                              `IPWhiteList` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
                              `IPRate` double DEFAULT NULL,
                              `CreateTime` datetime DEFAULT NULL,
                              `RequestStatus` int(2) DEFAULT NULL,
                              `RequestCount` int(11) DEFAULT NULL,
                              PRIMARY KEY (`Id`)
                            ) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

                     ");
                }
            }

            return Task.CompletedTask;
        }

        private void CreateDataBaseMySql(string Constr)
        {
            if (string.IsNullOrEmpty(Constr))
            {
                return;
            }

            string newStr = string.Empty;

            Constr.ToLower().Split(';').ToList().ForEach(x =>
            {
                if (!x.Contains("database"))
                {
                    newStr = newStr + x + ";";
                }
            });

            using (var TempConn = new MySqlConnection(newStr))
            {
                var DbInfo = TempConn.QueryFirstOrDefault<string>(" show databases like 'HttpReports'; ");

                if (string.IsNullOrEmpty(DbInfo))
                {
                    TempConn.Execute(" create database HttpReports; ");
                }
            }
        }
    }
}