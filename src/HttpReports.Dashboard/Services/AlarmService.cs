using System.Linq;
using System.Threading.Tasks;

using HttpReports.Dashboard.Models;

using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;
using MimeKit.Text;

namespace HttpReports.Dashboard.Services
{
    /// <summary>
    /// 告警服务
    /// </summary>
    public class AlarmService : IAlarmService
    {
        public MailOptions MailOptions { get; }

        private ILogger<AlarmService> Logger;

        public AlarmService(IOptions<MailOptions> mailOptions, ILogger<AlarmService> logger)
        {
            MailOptions = mailOptions.Value;
            Logger = logger;
        }

        private async Task SendMessageAsync(MimeMessage message)
        { 
            try
            {
                using (var client = new SmtpClient())
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Connect(MailOptions.Server, MailOptions.Port, MailOptions.EnableSsl);
                    client.Authenticate(MailOptions.Account, MailOptions.Password);

                    await client.SendAsync(message).ConfigureAwait(false);

                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }  
            }
            catch (System.Exception ex)
            { 
                Logger.LogInformation("预警邮件发送失败：" + ex.Message, ex);
            } 
        }

        public async Task AlarmAsync(AlarmOption option)
        {
            if (string.IsNullOrWhiteSpace(option.Content))
            {
                return;
            }

            if (MailOptions.Account.IsEmpty() || MailOptions.Password.IsEmpty())
            {
                Logger.LogInformation("预警邮件配置不能为空");
                return;
            }

            if (option.Emails?.Any() == true)
            { 
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("HttpReports", MailOptions.Account));

                foreach (var to in option.Emails)
                {
                    message.To.Add(new MailboxAddress(to));
                }

                message.Subject = "HttpReports - 预警触发通知";

                message.Body = new TextPart(option.IsHtml ? TextFormat.Html : TextFormat.Plain) { Text = option.Content };
                await SendMessageAsync(message).ConfigureAwait(false);
                 
            }
        }
    }
}