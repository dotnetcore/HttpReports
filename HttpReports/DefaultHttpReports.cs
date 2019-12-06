using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports
{
    public class DefaultHttpReports : IHttpReports
    {
        private HttpReportsOptions _options;

        public DefaultHttpReports(IOptions<HttpReportsOptions> options)
        {
            _options = options.Value;
        }

        public void Invoke(HttpContext context, TimeSpan ts, IConfiguration config)
        {
            if (string.IsNullOrEmpty(context.Request.Path))
            {
                return;
            }

            // 创建请求对象
            RequestInfo request = new RequestInfo();

            request.IP = context.Connection.RemoteIpAddress.ToString();

            request.Milliseconds = Convert.ToInt32(ts.TotalMilliseconds);

            request.StatusCode = context.Response.StatusCode;

            request.Method = context.Request.Method;

            request.Url = context.Request.Path;

            request.CreateTime = DateTime.Now;

            var path = context.Request.Path.Value ?? string.Empty;

            if (!path.ToLower().Contains(_options.APiPoint))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                { 
                    path = path.ToLower();

                    request.Node = "Default";

                    var arr = path.Substring(1).Split("/");

                    if (arr[1] == _options.APiPoint)
                    {
                        request.Node = arr[0];
                    }

                    var list = path.Split("/");

                    request.Route = path.Substring(path.IndexOf(_options.APiPoint) + _options.APiPoint.Length);

                    if (IsNumber(list.ToList().Last()))
                    {
                        request.Route = request.Route.Substring(0, request.Route.Length - list.ToList().Last().Length - 1);
                    }

                    // 添加到数据库 
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
                catch (Exception ex)
                { 
                     
                } 
            });  
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
