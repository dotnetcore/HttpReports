namespace HttpReports
{
    public class DefaultModelCreator : IModelCreator
    {
        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}