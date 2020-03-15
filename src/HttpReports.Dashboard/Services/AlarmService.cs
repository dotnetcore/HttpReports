using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using HttpReports.Dashboard.Models;

using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;

namespace HttpReports.Dashboard.Services
{
    /// <summary>
    /// 告警服务
    /// </summary>
    public class AlarmService : IAlarmService
    {
        public DashboardOptions Options { get; }

        private ILogger<AlarmService> Logger;

        public AlarmService(IOptions<DashboardOptions> options, ILogger<AlarmService> logger)
        {
            Options = options.Value;
            Logger = logger;
        }

        private async Task SendMessageAsync(MimeMessage message)
        { 
            try
            {
                using (var client = new SmtpClient())
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Connect(Options.Mail.Server,Options.Mail.Port, Options.Mail.EnableSsl);
                    client.Authenticate(Options.Mail.Account, Options.Mail.Password);

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

           await NotifyEmailAsync(option);

           await NotifyWebHookAsync(option); 

        }

        private async Task NotifyWebHookAsync(AlarmOption option)
        {
            try
            {
                if (option.WebHook.IsEmpty())
                {
                    return;
                } 
                
                using (var httpClient = new HttpClient())
                {
                    string Title = "HttpReports - 预警触发通知";

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(new { Title, option.Content }));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.PostAsync(option.WebHook, content);

                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Logger.LogInformation("WebHook推送成功");
                    }
                }  

            }
            catch (System.Exception ex)
            { 
                Logger.LogInformation("WebHook推送失败：" + ex.Message, ex);
            } 
        }

        private async Task NotifyEmailAsync(AlarmOption option)
        {
            if (option.Emails == null || option.Emails.Count() == 0)
            {
                return;
            } 
            

            if (Options.Mail == null || Options.Mail.Account.IsEmpty() || Options.Mail.Password.IsEmpty())
            {
                Logger.LogInformation("预警邮件配置不能为空");
                return;
            }

            if (option.Emails?.Any() == true)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("HttpReports", Options.Mail.Account));

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