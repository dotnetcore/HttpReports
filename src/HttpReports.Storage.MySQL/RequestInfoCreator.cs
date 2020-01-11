namespace HttpReports.Storage.MySql
{
    internal class RequestInfoCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}