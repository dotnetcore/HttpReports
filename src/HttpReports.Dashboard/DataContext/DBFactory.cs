using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.DataContext
{
    public class DBFactory
    {  
        public IConfiguration _configuration; 

        public DBFactory(IConfiguration configuration)
        {
            _configuration = configuration; 
        } 
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("HttpReports"));
        } 

        public MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(_configuration.GetConnectionString("HttpReports"));
        }

        public void InitDB()
        { 
            string DBType = _configuration["HttpReportsConfig:DBType"];

            string Constr = _configuration.GetConnectionString("HttpReports");

            if (string.IsNullOrEmpty(DBType) || string.IsNullOrEmpty(Constr) )
            {  
                throw new Exception("数据库类型配置错误!"); 
            }

            try
            { 
                if (DBType.ToLower() == "sqlserver") InitSqlServer(Constr);
                if (DBType.ToLower() == "mysql") InitMySql(Constr);

            }
            catch (Exception ex)
            { 
                throw new Exception("数据库初始化失败！错误信息:" + ex.Message);
            } 

        }

        private void InitSqlServer(string Constr)
        { 
            using (SqlConnection con = new SqlConnection(Constr))
            {
                //string TempConstr = Constr.Replace("httpreports", "master");

                //string DB_id = con.QueryFirstOrDefault<string>(" SELECT DB_ID('HttpReports') ");

                //if (string.IsNullOrEmpty(DB_id))
                //{
                //    int i = con.Execute(" Create Database HttpReports ");
                //}


                // 检查RequestInfo表
                if (con.QueryFirstOrDefault<int>(" Select Count(*) from sysobjects where id = object_id('HttpReports.dbo.RequestInfo') ") == 0)
                {
                    con.Execute(@"  

                        USE [HttpReports];
                        SET ANSI_NULLS ON; 
                        SET QUOTED_IDENTIFIER ON; 
                        CREATE TABLE [dbo].[RequestInfo](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Node] [nvarchar](50) NOT NULL,
	                        [Route] [nvarchar](50) NOT NULL,
	                        [Url] [nvarchar](200) NOT NULL,
	                        [Method] [nvarchar](50) NOT NULL,
	                        [Milliseconds] [int] NOT NULL,
	                        [StatusCode] [int] NOT NULL,
	                        [IP] [nvarchar](50) NOT NULL,
	                        [CreateTime] [datetime] NOT NULL,
                         CONSTRAINT [PK_RequestInfo] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]

                    ");

                    //new MockData().MockSqlServer(Constr); 
                } 

                // 检查Job表
                if (con.QueryFirstOrDefault<int>("Select Count(*) from sysobjects where id = object_id('HttpReports.dbo.Job')") == 0)
                {
                    con.Execute(@"

                            USE [HttpReports];
                            SET ANSI_NULLS ON;
                            SET QUOTED_IDENTIFIER ON; 
                            CREATE TABLE [dbo].[Job](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [Title] [nvarchar](50) NOT NULL,
	                            [CronLike] [nvarchar](50) NOT NULL,
	                            [Emails] [nvarchar](500) NOT NULL,
                                [Mobiles] [nvarchar](500) NOT NULL,
	                            [Status] [int] NOT NULL,
	                            [Servers] [nvarchar](500) NOT NULL,
	                            [RtStatus] [int] NOT NULL,
	                            [RtTime] [int] NOT NULL,
	                            [RtRate] [decimal](18, 4) NOT NULL,
	                            [HttpStatus] [int] NOT NULL,
	                            [HttpCodes] [nvarchar](500) NOT NULL,
	                            [HttpRate] [decimal](18, 4) NOT NULL,
	                            [IPStatus] [int] NOT NULL,
	                            [IPWhiteList] [nvarchar](500) NOT NULL,
	                            [IPRate] [decimal](18, 4) NOT NULL,
	                            [CreateTime] [datetime] NULL,
                                [RequestStatus] [int] NOT NULL,
                                [RequestCount] [int] NOT NULL,
                             CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]



                      ");   
                }  
            } 
        }

        private void InitMySql(string Constr)
        {  
            using (MySqlConnection con = new MySqlConnection(Constr))
            {
                //string TempConstr = Constr.ToLower().Replace("httpreports", "sys");

                //MySqlConnection TempConn = new MySqlConnection(TempConstr);

                //var DbInfo = TempConn.QueryFirstOrDefault<string>("  show databases like 'httpreports'; ");

                //if (string.IsNullOrEmpty(DbInfo))
                //{
                //    TempConn.Execute(" create database HttpReports; ");
                //}

                //TempConn.Close();
                //TempConn.Dispose(); 

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

                   //new MockData().MockMySql(Constr);

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
        }  
    }  
}
