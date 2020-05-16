using HttpReports.Dashboard.Route;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard
{
    public class DashboardContext
    { 
        public HttpContext HttpContext { get; }

        public DashboardRoute Route { get; }

        public DashboardOptions Options { get; } 


        public DashboardContext(HttpContext httpContext, DashboardRoute route, DashboardOptions options)
        { 
            Route = route ?? throw new ArgumentNullException(nameof(route));
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            Options = options;
        }
    }
}
