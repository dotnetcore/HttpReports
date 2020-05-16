using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Views
{
    public abstract class RazorPage
    {
        private readonly StringBuilder _content = new StringBuilder();
        private string _body;

        public RazorPage Layout { get; protected set; }

        public Dictionary<string, object> ViewData { get; set; }

        public abstract void Execute();

        public Dictionary<string, string> Section { get; set; }


        public DashboardContext Context { get; set; }

        protected RazorPage()
        {
            Section = new Dictionary<string, string>();
            ViewData = new Dictionary<string, object>();

        }

        protected RazorPage(Dictionary<string, object> viewData)
        {
            ViewData = viewData;
        }

        protected void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _content.Append(textToAppend);
        }

        /// <exclude />
        protected virtual void Write(object value)
        {
            if (value == null)
                return;
            var html = value as string;
            WriteLiteral(html);
        }

        protected virtual string RenderBody()
        {
            return _body;
        }

        protected virtual string RenderSection(string sectionName)
        {
            return !Section.TryGetValue(sectionName, out string content) ? null : content;
        }


        protected string Raw(object value)
        {
            var html = value as string;
            return new HtmlString(html).ToString();
        }

        public override string ToString()
        {
            return TransformText(null);
        }

        private string TransformText(string body)
        {
            _body = body;

            Execute();

            return Layout != null ? Layout.TransformText(_content.ToString()) : _content.ToString();
        }
    } 
}
