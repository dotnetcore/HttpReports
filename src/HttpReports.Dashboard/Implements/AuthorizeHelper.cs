using HttpReports.Core.Config;
using HttpReports.Dashboard.Handle;
using HttpReports.Dashboard.Route;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Implements
{
    public static class AuthorizeHelper
    {
        public static async Task<bool> AuthorizeAsync(HttpContext context,IDashboardHandle handle,DashboardRoute route)
        {
            if (handle.GetType().GetMethod(route.Action).GetCustomAttribute<AllowAnonymousAttribute>() != null)
            {
                return await Task.FromResult(true);
            } 

            if (context.Request.Path.HasValue)
            {
                if (!BasicConfig.CurrentControllers.IsEmpty())
                { 
                    if (!BasicConfig.CurrentControllers.Split(',').Select(x => x.ToLowerInvariant()).Contains(route.Handle.ToLowerInvariant()))
                    {
                        return await Task.FromResult(true);
                    }
                }
            }

            string username = context.Request.Cookies[BasicConfig.LoginCookieId];

            if (string.IsNullOrEmpty(username))
            {
                context.Response.Redirect("/HttpReports/UserLogin");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        } 
    }
}
