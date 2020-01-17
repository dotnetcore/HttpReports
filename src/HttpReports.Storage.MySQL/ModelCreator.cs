namespace HttpReports.Storage.MySql
{
    internal class ModelCreator : DefaultModelCreator
    {
        public override IRequestInfo CreateRequestInfo() => new RequestInfo();
    }
}