using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HttpReports.Core;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Models;
using HttpReports.Storage.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 
using MimeKit;
using MimeKit.Text; 

namespace HttpReports.Dashboard.Services
{
  
    public class AlarmService : IAlarmService
    {
        public DashboardOptions Options { get; }

        private ILogger<AlarmService> Logger;

        private ILocalizeService _localizeService;

        private IHttpReportsStorage _storage;

        private readonly JsonSerializerOptions _jsonSetting;

        private readonly IHttpClientFactory _factory;

        private Localize lang => _localizeService.Current;

        public AlarmService(IOptions<DashboardOptions> options, JsonSerializerOptions jsonSetting, ILogger<AlarmService> logger, ILocalizeService localizeService, IHttpReportsStorage storage, IHttpClientFactory factory)
        {
            Options = options.Value;
            Logger = logger;
            _localizeService = localizeService;
            _storage = storage;
            _jsonSetting = jsonSetting;
            _factory = factory;
        }

        private async Task SendMessageAsync(MimeMessage message)
        { 
            try
            {
                if (!Options.Mail.Switch) return; 

                using (var client = new SmtpClient())
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Connect(Options.Mail.Server,Options.Mail.Port, Options.Mail.EnableSsl);
                    client.Authenticate(Options.Mail.Account, Options.Mail.Password);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }  
            }
            catch (System.Exception ex)
            { 
                Logger.LogError("Failed to send alert mail：" + ex.Message, ex);
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

           await NotifyAlarmAsync(option);

        }

        private async Task NotifyWebHookAsync(AlarmOption option)
        {
            try
            {
                if (option.WebHook.IsEmpty())
                {
                    return;
                } 
                
                using (var httpClient = _factory.CreateClient())
                {
                    string Title = $"HttpReports - {lang.Warning_Title}";

                    HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { Title, option.Content },_jsonSetting));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.PostAsync(option.WebHook, content);

                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Logger.LogInformation("WebHook Push Success");
                    }
                }  

            }
            catch (System.Exception ex)
            { 
                Logger.LogInformation("WebHook Push Error：" + ex.Message, ex);
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
                Logger.LogInformation("Mailbox cannot be empty");
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

                message.Subject = $"HttpReports - {lang.Warning_Title}";

                message.Body = new TextPart(option.IsHtml ? TextFormat.Html : TextFormat.Plain) { Text = option.Content };
                await SendMessageAsync(message); 

            } 
        }

        private async Task NotifyAlarmAsync(AlarmOption option)
        {
            if (option.Alarm == null) return;

            await  _storage.AddMonitorAlarm(option.Alarm);

        }
    }
}