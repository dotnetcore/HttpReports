using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpReports.Dashboard.Filters
{
    public class GlobalAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 判断是否跳过授权过滤器
            if (context.Filters.Any(x => x is IAllowAnonymousFilter))
            {
                return;
            }

            string cookie = context.HttpContext.Request.Cookies["login_info"];

            if (string.IsNullOrEmpty(cookie))
            {
                context.Result = new RedirectResult("/User/Login");
                return;
            }
        }
    }
}