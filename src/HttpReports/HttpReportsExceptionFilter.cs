using HttpReports.Core.Config;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public class HttpReportsExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        { 
            if (context.Exception != null)
            {
               context.HttpContext.Items.Add(BasicConfig.HttpReportsGlobalException, context.Exception);
            } 
        }
    }
}
