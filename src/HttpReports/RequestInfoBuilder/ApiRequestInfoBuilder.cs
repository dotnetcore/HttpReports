using System.Linq;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    internal class ApiRequestInfoBuilder : BaseRequestInfoBuilder
    {
        public ApiRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options) : base(modelCreator, options)
        { }

        protected override IRequestInfo Build(IRequestInfo request, string path)
        { 
            request.Node = GetNode(path);
            request.Route = GetRouteForAPI(path);

            return request;
        }

     
        protected string GetRouteForAPI(string path)
        {
            var list = path.Split('/').ToList();

            string route = path.Substring(path.IndexOf(Options.ApiPoint) + Options.ApiPoint.Length);

            if (IsNumber(list.Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            return route;
        }
    }
}