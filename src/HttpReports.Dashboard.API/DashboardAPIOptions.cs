using HttpReports.Core.Config;

using Microsoft.Extensions.Options;

namespace HttpReports.Dashboard
{
    public class DashboardAPIOptions : IOptions<DashboardAPIOptions>
    {
        public MailOptions Mail { get; set; } = new MailOptions();

        public int ExpireDay { get; set; } = BasicConfig.ExpireDay;

        /// <summary>
        /// 本地化文件附加路径
        /// </summary>
        public string LocalizeAddOnDirectory { get; set; }

        public DashboardAPIOptions Value => this;
    }
}