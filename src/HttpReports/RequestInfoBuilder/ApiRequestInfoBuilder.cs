using System.Linq;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    internal class ApiRequestInfoBuilder : BaseRequestInfoBuilder
    {
        public ApiRequestInfoBuilder(IOptions<HttpReportsOptions> options) : base(options)
        { }

        protected override RequestInfo Build(RequestInfo request, string path)
        {
            request.Node = GetNode(path);
            request.Route = GetRouteForAPI(path);

            return request;
        }

        /// <summary>
        ///通过请求地址 获取路由
        /// </summary>
        /// <returns></returns>
        protected string GetRouteForAPI(string path)
        {
            var list = path.Split('/');

            string route = path.Substring(path.IndexOf(Options.ApiPoint) + Options.ApiPoint.Length);

            if (IsNumber(list.ToList().Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            return route;
        }
    }
}