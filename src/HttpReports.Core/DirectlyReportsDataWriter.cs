namespace HttpReports
{
    public class DirectlyReportsDataWriter : IReportsDataWriter
    {
        public IHttpReportsStorage Storage { get; }

        public DirectlyReportsDataWriter(IHttpReportsStorage storage)
        {
            Storage = storage;
        }

        public void Write(IRequestInfo requestInfo, IRequestDetail requestDetail)
        {
            Storage.AddRequestInfoAsync(requestInfo, requestDetail).ConfigureAwait(false);
        }
    }
}