namespace HttpReports.Storage.Oracle
{
    internal class ModelCreator : DefaultModelCreator
    {
        public override IRequestInfo CreateRequestInfo() => new RequestInfo();
    }
}