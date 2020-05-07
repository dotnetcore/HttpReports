namespace HttpReports
{
    public interface IReportsTransport
    {
        void Write(IRequestInfo requestInfo, IRequestDetail requestDetail);
    }
}