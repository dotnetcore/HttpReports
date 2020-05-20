namespace HttpReports
{
    public class DirectlyReportsTransport : IReportsTransport
    {
        public IHttpReportsStorage Storage { get; }

        public DirectlyReportsTransport(IHttpReportsStorage storage)
        {
            Storage = storage;
        }

        public void Write(IRequestInfo requestInfo, IRequestDetail requestDetail)
        {
            Storage.AddRequestInfoAsync(requestInfo, requestDetail).ConfigureAwait(false);
        }
    }
}