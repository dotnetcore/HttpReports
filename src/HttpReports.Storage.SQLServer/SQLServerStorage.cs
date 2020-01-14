using Dapper;
using HttpReports.Models;
using HttpReports.Storage.FilterOptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace HttpReports.Storage.SQLServer
{  
    internal class SQLServerStorage : IHttpReportsStorage
    {
        public SQLServerConnectionFactory ConnectionFactory { get; }

        public ILogger<SQLServerStorage> Logger { get; }

        public SQLServerStorage(SQLServerConnectionFactory connectionFactory, ILogger<SQLServerStorage> logger)
        {
            ConnectionFactory = connectionFactory;
            Logger = logger;
        }

        public async Task InitAsync()
        {
            using (var con = ConnectionFactory.GetConnection())
            {
                // 检查RequestInfo表
                if (con.QueryFirstOrDefault<int>(" Select Count(*) from sysobjects where id = object_id('HttpReports.dbo.RequestInfo') ") == 0)
                {
                   await con.ExecuteAsync(@"  
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
                    ").ConfigureAwait(false); 
                }

                // 检查Job表
                if (con.QueryFirstOrDefault<int>("Select Count(*) from sysobjects where id = object_id('HttpReports.dbo.Job')") == 0)
                {
                   await con.ExecuteAsync(@"
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
                      ").ConfigureAwait(false);
                } 

            }
        }

        public Task AddRequestInfoAsync(IRequestInfo request)
        {
            throw new NotImplementedException();
        }

        public Task<List<NodeInfo>> GetNodesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<UrlRequestCount>> GetUrlRequestStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<RequestAvgResponeTime>> GetRequestAvgResponeTimeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatusCodeCount>> GetStatusCodeStatisticsAsync(RequestInfoFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResponeTimeGroup>> GetGroupedResponeTimeStatisticsAsync(GroupResponeTimeFilterOption filterOption)
        {
            throw new NotImplementedException();
        }

        public Task<IndexPageData> GetIndexPageDataAsync(IndexPageDataFilterOption filterOption)
        {
            throw new NotImplementedException();
        }
    }
}