using HttpReports.Dashboard.Handle;
using HttpReports.Dashboard.Route;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Services
{
    public interface IAuthService
    {

        string BuildToken();

        bool ValidToken(string token);


        bool ValidToken(HttpContext httpContext, IDashboardHandle handle, DashboardRoute route);

    }
}
