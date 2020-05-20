using System.Collections.Generic;

namespace HttpReports.Dashboard.Models
{
    public class AlarmOption
    {
        public string WebHook { get; set; }

        public IEnumerable<string> Emails { get; set; }

        public IEnumerable<string> Phones { get; set; }

        public string Content { get; set; }

        public bool IsHtml { get; set; }
    }
}