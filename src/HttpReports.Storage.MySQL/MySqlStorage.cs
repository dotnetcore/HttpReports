using System;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : IHttpReportsStorage
    {
        public MySqlConnectionFactory ConnectionFactory { get; }

        public ILogger<MySqlStorage> Logger { get; }

        public MySqlStorage(MySqlConnectionFactory connectionFactory, ILogger<MySqlStorage> logger)
        {
            ConnectionFactory = connectionFactory;
            Logger = logger;
        }

        public async Task InitAsync()
        {
            using (var con = ConnectionFactory.GetConnection())
            {
                if (con.QueryFirstOrDefault<int>("  Select count(1) from information_schema.tables where table_name ='RequestInfo' and table_schema = 'HttpReports'; ") == 0)
                {
                    await con.ExecuteAsync(@"
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
                        ) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;  ").ConfigureAwait(false);
                }

                if (con.QueryFirstOrDefault<int>(" Select count(1) from information_schema.tables where table_name ='Job' and table_schema = 'HttpReports'; ") == 0)
                {
                    await con.ExecuteAsync(@"

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

                     ").ConfigureAwait(false);
                }
            }
        }

        public async Task AddRequestInfoAsync(IRequestInfo request)
        {
            //TODO 实现批量存储
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    await connection.InsertAsync(request as RequestInfo).ConfigureAwait(false);
                    //await connection.ExecuteAsync("insert into RequestInfo(Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime) values(@Node,@Route,@Url,@Method,@Milliseconds,@StatusCode,@IP,@CreateTime)", request).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "请求数据保存失败");
            }
        }
    }
}