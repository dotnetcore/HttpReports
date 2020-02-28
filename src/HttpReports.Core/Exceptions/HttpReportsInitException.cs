using System;
using System.Runtime.Serialization;

namespace HttpReports
{
    public class HttpReportsInitException : HttpReportsException
    {
        public HttpReportsInitException()
        {
        }

        public HttpReportsInitException(string message) : base(message)
        {
        }

        public HttpReportsInitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpReportsInitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}