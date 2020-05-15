using System.Linq;
using HttpReports.Dashboard.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HttpReports.Dashboard
{
    internal class HttpReportsDashboardApplicationModelConvention : IApplicationModelConvention
    {
        private readonly DashboardAPIRouteOptions _routeOptions;

        public HttpReportsDashboardApplicationModelConvention(DashboardAPIRouteOptions routeOptions)
        {
            _routeOptions = routeOptions;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.ControllerType == typeof(DashboardDataProvideService))
                {
                    if (!string.IsNullOrEmpty(_routeOptions.CorsPolicyName))
                    {
                        if (controller.Attributes.Cast<EnableCorsAttribute>().FirstOrDefault() is EnableCorsAttribute enableCors)
                        {
                            enableCors.PolicyName = _routeOptions.CorsPolicyName;
                        }
                    }
                    controller.ApiExplorer.GroupName = "HttpReportsDashboardAPI";
                    controller.ApiExplorer.IsVisible = true;
                    foreach (var action in controller.Actions)
                    {
                        action.ApiExplorer.IsVisible = true;
                        action.Selectors[0].AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"{_routeOptions.RoutePrefix}/{action.ActionName}"));
                    }
                }
            }
        }
    }
}