using System;

namespace HttpReports.Dashboard
{
    /// <summary>
    /// api路由选项
    /// </summary>
    public class DashboardAPIRouteOptions
    {
        /// <summary>
        /// 默认路由
        /// </summary>
        public const string DefaultRoutePrefix = "HttpReportsDashboard/Api";

        /// <summary>
        /// 路由
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// Cors策略名称
        /// </summary>
        public string CorsPolicyName { get; set; }

        /// <summary>
        /// api路由选项
        /// </summary>
        /// <param name="routePrefix"></param>
        /// <param name="corsPolicyName">null for default</param>
        public DashboardAPIRouteOptions(string routePrefix = DefaultRoutePrefix, string corsPolicyName = null)
        {
            RoutePrefix = routePrefix ?? throw new ArgumentNullException(nameof(routePrefix));
            CorsPolicyName = corsPolicyName ?? throw new ArgumentNullException(nameof(corsPolicyName));
        }
    }
}