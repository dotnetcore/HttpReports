using HttpReports.Core.Config;

using Microsoft.Extensions.Options;

namespace HttpReports.Dashboard
{
    public class DashboardAPIOptions : IOptions<DashboardAPIOptions>
    {
        public MailOptions Mail { get; set; } = new MailOptions();

        public int ExpireDay { get; set; } = BasicConfig.ExpireDay;

        public DashboardAPIOptions Value => this;
    }
}