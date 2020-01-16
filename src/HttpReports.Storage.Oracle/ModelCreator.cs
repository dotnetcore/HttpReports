namespace HttpReports.Storage.Oracle
{
    internal class ModelCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}