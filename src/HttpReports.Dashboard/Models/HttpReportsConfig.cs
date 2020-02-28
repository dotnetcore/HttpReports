using Microsoft.Extensions.Configuration;

namespace HttpReports.Dashboard.Models
{
    public class HttpReportsConfig
    {
        public HttpReportsConfig(IConfiguration configuration)
        {
            this.UserName = configuration["HttpReportsConfig:UserName"];
            this.Password = configuration["HttpReportsConfig:Password"];
        }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}