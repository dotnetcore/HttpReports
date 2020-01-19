using System.Collections.Generic;

namespace HttpReports.Dashboard.Models
{
    /// <summary>
    /// 告警选项
    /// </summary>
    public class AlarmOption
    {
        /// <summary>
        /// 邮箱列表
        /// </summary>
        public IEnumerable<string> Emails { get; set; }

        /// <summary>
        /// 手机号列表
        /// </summary>
        public IEnumerable<string> Phones { get; set; }

        /// <summary>
        /// 告警内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 内容是否是Html
        /// </summary>
        public bool IsHtml { get; set; }
    }
}