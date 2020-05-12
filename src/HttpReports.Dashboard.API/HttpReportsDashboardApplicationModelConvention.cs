using HttpReports.Dashboard.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HttpReports.Dashboard
{
    internal class HttpReportsDashboardApplicationModelConvention : IApplicationModelConvention
    {
        private readonly string _routePrefix;

        public HttpReportsDashboardApplicationModelConvention(string routePrefix)
        {
            _routePrefix = routePrefix;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.ControllerType == typeof(DashboardDataProvideService))
                {
                    controller.ApiExplorer.GroupName = "HttpReportsDashboardAPI";
                    controller.ApiExplorer.IsVisible = true;
                    foreach (var action in controller.Actions)
                    {
                        action.ApiExplorer.IsVisible = true;
                        action.Selectors[0].AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"{_routePrefix}/{action.ActionName}"));
                    }
                }
            }
        }
    }
}