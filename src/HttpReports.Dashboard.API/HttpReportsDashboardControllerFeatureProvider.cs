using System.Reflection;

using HttpReports.Dashboard.Services;

using Microsoft.AspNetCore.Mvc.Controllers;

namespace HttpReports.Dashboard
{
    internal class HttpReportsDashboardControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (typeInfo == typeof(DashboardDataProvideService))
            {
                return true;
            }
            return base.IsController(typeInfo);
        }
    }
}