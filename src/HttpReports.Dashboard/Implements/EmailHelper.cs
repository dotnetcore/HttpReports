using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace HttpReports.Dashboard.Implements
{
    public static class EmailHelper
    {
        public static void Send(IEnumerable<string> tos, string title, string content)
        {
            foreach (var to in tos)
            {
                Send(to, title, content);
            }
        }

        public static void Send(string to, string title, string content)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var url = "http://175.102.11.117:8802/api/email/send";

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new
                {
                    to,
                    title,
                    content
                }), System.Text.Encoding.UTF8, "application/json")).Result;

                string result = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}