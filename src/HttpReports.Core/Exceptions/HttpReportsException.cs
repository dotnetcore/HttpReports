using System;
using System.Runtime.Serialization;

namespace HttpReports
{
    public class HttpReportsException : Exception
    {
        public HttpReportsException()
        {
        }

        public HttpReportsException(string message) : base(message)
        {
        }

        public HttpReportsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpReportsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}