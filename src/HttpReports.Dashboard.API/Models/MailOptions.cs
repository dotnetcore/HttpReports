using Microsoft.Extensions.Options;

namespace HttpReports.Dashboard
{
    public class MailOptions : IOptions<MailOptions>
    {
        /// <summary>
        /// 邮件服务地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 邮件服务端口
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 启用加密连接
        /// </summary>
        public bool EnableSsl { get; set; } = false;

        public MailOptions Value => this;
    }
}