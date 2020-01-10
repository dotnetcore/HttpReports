using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace HttpReports
{
    public class DefaultHttpReports : IHttpReports
    {
        private HttpReportsOptions _options;

        private IHostingEnvironment _environment;

        public DefaultHttpReports(IOptions<HttpReportsOptions> options, IHostingEnvironment environment)
        {
            _options = options.Value;
            _environment = environment;
        }

        public void Invoke(HttpContext context, double Milliseconds, IConfiguration config)
        {
            if (string.IsNullOrEmpty(context.Request.Path))
            {
                return;
            }

            // 创建请求对象
            RequestInfo request = new RequestInfo();

            request.IP = context.Connection.RemoteIpAddress.ToString();

            request.Milliseconds = ToInt(Milliseconds);

            request.StatusCode = context.Response.StatusCode;

            request.Method = context.Request.Method;

            request.Url = context.Request.Path;

            request.CreateTime = DateTime.Now;

            var path = (context.Request.Path.Value ?? string.Empty).ToLower();

            // 录入数据库
            if (_options.WebType == WebType.API) SaveWhenUseAPI(request, path, config);

            if (_options.WebType == WebType.MVC) SaveWhenUseMVC(request, path, config);
        }

        /// <summary>
        /// 检查数据库连接是否可用
        /// </summary>
        /// <param name="config"></param>
        public void Init(IConfiguration config)
        {
            try
            {
                string constr = config.GetConnectionString("HttpReports");

                if (string.IsNullOrEmpty(constr))
                {
                    if (_environment.IsDevelopment())
                    {
                        throw new Exception("appsettings.json未找到HttpReports连接字符串");
                    }
                }

                if (_options.DBType == DBType.SqlServer)
                {
                    InitSqlServer(constr);
                }

                if (_options.DBType == DBType.MySql)
                {
                    InitMySql(constr);
                }
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    throw ex;
                }
            }
        }

        private void InitSqlServer(string Constr)
        {
            using (SqlConnection con = new SqlConnection(Constr))
            {
                CreateDataBaseSqlServer(Constr);

                int TableCount = con.QueryFirstOrDefault<int>(" Use HttpReports; Select Count(*) from sysobjects where id = object_id('HttpReports.dbo.RequestInfo') ");

                if (TableCount == 0)
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
	                        [CreateTime] [datetime] NOT NULL
                        ) ON [PRIMARY];

                    ");
                }
            }
        }

        private void InitMySql(string Constr)
        {
            using (MySqlConnection con = new MySqlConnection(Constr))
            {
                CreateDataBaseMySql(Constr);

                var TableInfo = con.QueryFirstOrDefault<int>("  Select count(1) from information_schema.tables where table_name ='RequestInfo' and table_schema = 'HttpReports'; ");

                if (TableInfo == 0)
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
            }
        }


        private void CreateDataBaseSqlServer(string Constr)
        {
            if (string.IsNullOrEmpty(Constr))
            {
                return;
            }

            string newStr = string.Empty;

            Constr.ToLower().Split(';').ToList().ForEach(x => {

                if (!x.Contains("database"))
                {
                    newStr = newStr + x + ";";
                }
            });

            using (SqlConnection connection = new SqlConnection(newStr))
            {
                string DB_id = connection.QueryFirstOrDefault<string>(" SELECT DB_ID('HttpReports') ");

                if (string.IsNullOrEmpty(DB_id))
                {
                    connection.Execute(" Create Database HttpReports; ");
                }
            }
        }

        private void CreateDataBaseMySql(string Constr)
        {
            if (string.IsNullOrEmpty(Constr))
            {
                return;
            }

            string newStr = string.Empty;

            Constr.ToLower().Split(';').ToList().ForEach(x => {

                if (!x.Contains("database"))
                {
                    newStr = newStr + x + ";";
                }
            });

            using (MySqlConnection TempConn = new MySqlConnection(newStr))
            {
                var DbInfo = TempConn.QueryFirstOrDefault<string>(" show databases like 'HttpReports'; ");

                if (string.IsNullOrEmpty(DbInfo))
                {
                    TempConn.Execute(" create database HttpReports; ");
                }
            }
        }


        /// <summary>
        /// 使用API模式
        /// </summary>
        /// <param name="request"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task SaveWhenUseAPI(RequestInfo request, string path, IConfiguration config)
        {
            return Task.Run(() =>
            {
                if (!path.ToLower().Contains(_options.ApiPoint))
                {
                    return;
                }

                try
                {
                    request.Node = GetNode(path);
                    request.Route = GetRouteForAPI(path);

                    SaveInDataBase(request, config);
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                    {
                        throw ex;
                    }
                }
            });
        }

        /// <summary>
        /// 使用API模式
        /// </summary>
        /// <param name="request"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task SaveWhenUseMVC(RequestInfo request, string path, IConfiguration config)
        {
            return Task.Run(() =>
            {
                try
                {
                    request.Node = _options.Node.Substring(0, 1).ToUpper() + _options.Node.Substring(1).ToLower();

                    request.Route = GetRouteForMVC(path);

                    SaveInDataBase(request, config);
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                    {
                        throw ex;
                    }
                }
            });
        }

        private void SaveInDataBase(RequestInfo request, IConfiguration config)
        {
            //添加到数据库
            if (_options.DBType == DBType.SqlServer)
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("HttpReports")))
                {
                    con.Insert<RequestInfo>(request);
                }
            }

            if (_options.DBType == DBType.MySql)
            {
                using (MySqlConnection con = new MySqlConnection(config.GetConnectionString("HttpReports")))
                {
                    con.Insert<RequestInfo>(request);
                }
            }
        }

        /// <summary>
        /// 通过请求地址 获取服务节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        private string GetNode(string path)
        {
            string Default = _options.Node;

            var arr = path.Substring(1).Split('/');

            if (arr.Length > 0 && arr[1] == _options.ApiPoint)
            {
                Default = arr[0];
            }

            Default = Default.Substring(0, 1).ToUpper() + Default.Substring(1).ToLower();

            return Default;
        }

        /// <summary>
        ///通过请求地址 获取路由
        /// </summary>
        /// <returns></returns>
        private string GetRouteForAPI(string path)
        {
            string route = string.Empty;

            var list = path.Split('/');

            route = path.Substring(path.IndexOf(_options.ApiPoint) + _options.ApiPoint.Length);

            if (IsNumber(list.ToList().Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            return route;
        }

        /// <summary>
        ///通过请求地址 获取路由
        /// </summary>
        /// <returns></returns>
        private string GetRouteForMVC(string path)
        {
            string route = path;

            var list = path.Split('/');

            if (IsNumber(list.ToList().Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            return route;
        }

        private int ToInt(double dou)
        {
            try
            {
                return Convert.ToInt32(dou);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private bool IsNumber(string str)
        {
            try
            {
                int i = Convert.ToInt32(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}