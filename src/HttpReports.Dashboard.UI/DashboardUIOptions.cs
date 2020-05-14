using Microsoft.Extensions.Options;

namespace HttpReports.Dashboard
{
    /// <summary>
    ///
    /// </summary>
    public class DashboardUIOptions : IOptions<DashboardUIOptions>
    {
        private string _routePrefix;

        /// <summary>
        ///
        /// </summary>
        public DashboardUIOptions Value => this;

        /// <summary>
        ///
        /// </summary>
        public string RoutePrefix
        {
            get => _routePrefix;
            set
            {
                _routePrefix = $"/{value.Trim('/')}";
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string APIAddress { get; set; }
    }
}