using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Route
{
    public class DashboardRoute
    {
        public DashboardRoute(string key,Type view)
        {
            HtmlView = view != null;
            Key = key;
            View = view;

        }

        public DashboardRoute()
        {

        }

        public string Key { get; set; }

        public Type View { get; set; }

        public string Handle { get; set; }

        public string Action { get; set; }

        public bool HtmlView { get; set; }
    }
}
