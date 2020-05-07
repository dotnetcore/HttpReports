using System;

namespace HttpReports.Collector.Grpc
{
    public partial class RequestInfo : IRequestInfo
    {
        private DateTime? _createTime = null;

        public DateTime CreateTime
        {
            get
            {
                if (_createTime == null)
                {
                    _createTime = new DateTime(CreateTimeStamp);
                }
                return _createTime.Value;
            }
            set
            {
                _createTime = value;
                CreateTimeStamp = value.Ticks;
            }
        }
    }
}