using System.Linq;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    internal class WebRequestInfoBuilder : BaseRequestInfoBuilder
    {
        public WebRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options) : base(modelCreator, options)
        { }

        protected override IRequestInfo Build(IRequestInfo request, string path)
        {
            request.Node = Options.Node.Substring(0, 1).ToUpper() + Options.Node.Substring(1).ToLower();
            request.Route = GetRouteForWeb(path);

            return request;
        }

        /// <summary>
        ///通过请求地址 获取路由
        /// </summary>
        /// <returns></returns>
        private string GetRouteForWeb(string path)
        {
            string route = path;

            var list = path.Split('/');

            if (IsNumber(list.ToList().Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            return route;
        }
    }
}