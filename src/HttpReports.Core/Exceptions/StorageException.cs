using System;
using System.Runtime.Serialization;

namespace HttpReports
{
    public class StorageException : HttpReportsException
    {
        public StorageException()
        {
        }

        public StorageException(string message) : base(message)
        {
        }

        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}