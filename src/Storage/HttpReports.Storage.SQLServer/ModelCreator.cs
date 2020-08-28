namespace HttpReports.Storage.SQLServer
{
    internal class ModelCreator : DefaultModelCreator
    {
        public override RequestInfo CreateRequestInfo() => new RequestInfo();
    }
}