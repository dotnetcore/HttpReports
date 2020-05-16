using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpReports.Dashboard.Route
{
    public class RouteCollection
    {
        private static readonly List<DashboardRoute> Routes = new List<DashboardRoute>();

        public void AddRoute(DashboardRoute route)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrWhiteSpace(route.Key))
            {
                throw new ArgumentNullException("route key can not be null");
            }

            if (route.HtmlView)
            {
                if (route.View == null)
                {
                    throw new ArgumentNullException("route view can not be null");
                }
            }


            if (string.IsNullOrWhiteSpace(route.Handle) || string.IsNullOrWhiteSpace(route.Action))
            {
                try
                {
                    var routeArray = route.Key.Split('/');
                    route.Handle = routeArray[1];
                    route.Action = routeArray[2];
                }
                catch (Exception ex)
                {
                    if (route.HtmlView)
                    {
                        throw new ArgumentException("route key fotmat handle/action", ex);
                    }

                }


            }

            if (Routes.Exists(x => x.Key == route.Key))
            {
                Routes[Routes.IndexOf(Routes.FirstOrDefault(x => x.Key == route.Key))] = route;
            }
            else
            {
                Routes.Add(route);
            }

        }

        public DashboardRoute FindRoute(string url)
        {
            if (url.Length > 1 && url.Last() == '/')
            {
                url = url.Substring(0, url.Length - 1);
            } 

            return string.IsNullOrWhiteSpace(url) ? Routes.FirstOrDefault(x => x.Key.ToLower() == "/Dashboard/Index".ToLower()) : Routes.FirstOrDefault(x => x.Key.ToLower() == url.ToLower());
        }
    }
}
