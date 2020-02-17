using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpReports.RequestInfoBuilder
{
    internal class DefaultRequestInfoBuilder : BaseRequestInfoBuilder
    {
        public DefaultRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options) : base(modelCreator, options)
        { 
        
        }


        protected override IRequestInfo Build(IRequestInfo request, string path)
        {
            if (Options.Node.IsEmpty())
            {
                Options.Node = "Default";
            }

            request.Node = Options.Node.Substring(0, 1).ToUpper() + Options.Node.Substring(1).ToLower();
            request.Route = GetRoute(path);

            return request;
        }

       
        private string GetRoute(string path)
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
