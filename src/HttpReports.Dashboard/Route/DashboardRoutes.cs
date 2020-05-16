using HttpReports.Dashboard.Handle;
using HttpReports.Dashboard.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HttpReports.Dashboard.Route
{
    public static class DashboardRoutes
    {
        public static RouteCollection Routes { get; } 

        static DashboardRoutes()
        {
            Routes = new RouteCollection();

            var views = Assembly.GetAssembly(typeof(DashboardRoute)).GetTypes().Where(x => typeof(RazorPage).IsAssignableFrom(x) && x != typeof(RazorPage));

            foreach (var item in views)
            {
                Routes.AddRoute(new DashboardRoute("/HttpReports/"+item.Name,item));
            }  
          

            typeof(DashboardDataHandle).GetMethods().Select(x => x.Name).ToList().ForEach(action => {

                Routes.AddRoute(new DashboardRoute("/HttpReportsData/" + action, null));

            });  

        }

    }
}
