namespace HttpReports.Storage.SQLServer
{
    internal class ModelCreator : DefaultModelCreator
    {
        public override IRequestInfo CreateRequestInfo() => new RequestInfo();
    }
}