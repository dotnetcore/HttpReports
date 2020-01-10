using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpReports.Dashboard.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
        }
    }
}