namespace HttpReports.Storage.MySql
{
    internal class ModelCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}