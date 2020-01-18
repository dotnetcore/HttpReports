namespace HttpReports.Storage.SQLServer
{
    internal class ModelCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}