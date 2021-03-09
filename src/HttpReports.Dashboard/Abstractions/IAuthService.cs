using HttpReports.Dashboard.Handles;
using HttpReports.Dashboard.Routes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Abstractions
{
    public interface IAuthService
    {
        string BuildToken();

        bool ValidToken(string token); 

        bool ValidToken(HttpContext httpContext, IDashboardHandle handle, DashboardRoute route);

    }
}
