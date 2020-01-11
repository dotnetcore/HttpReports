namespace HttpReports
{
    internal class DefaultModelCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}