using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace HttpReports.Dashboard.Views
{
    public class HtmlString
    {
        private readonly string _value;

        public HtmlString(string value)
        {
            _value = value;
        }

        public void WriteTo(TextWriter writer,HtmlEncoder encoder)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            writer.Write(this._value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
