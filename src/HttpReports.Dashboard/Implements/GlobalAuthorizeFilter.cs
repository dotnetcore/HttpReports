using HttpReports.Core.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpReports.Dashboard.Implements
{
    public class GlobalAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {  
            if (context.Filters.Any(x => x is IAllowAnonymousFilter))
            {
                return;
            }

            var ActionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;

            if (ActionDescriptor != null)
            {
                if (ActionDescriptor.MethodInfo.CustomAttributes != null )
                {
                    foreach (var item in ActionDescriptor.MethodInfo.CustomAttributes)
                    {
                        if (item.AttributeType.Name == "AllowAnonymousAttribute")
                        {
                            return;
                        }
                       
                    } 

                } 
                
            } 
          
           

            if (context.HttpContext.Request.Path.HasValue)
            { 
                if (!BasicConfig.CurrentControllers.IsEmpty())
                {
                    var controller = (context.ActionDescriptor.RouteValues["controller"] ?? string.Empty).ToLower();

                    if (!BasicConfig.CurrentControllers.Split(',').Select(x=>x.ToLower()).Contains(controller))
                    {
                        return;
                    }  
                }   
            } 
 

            string username = context.HttpContext.Request.Cookies[BasicConfig.LoginCookieId];

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectResult("/HttpReports/UserLogin");
                return;
            }
        } 

    }
}
