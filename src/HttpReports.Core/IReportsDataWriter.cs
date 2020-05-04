namespace HttpReports
{
    public interface IReportsDataWriter
    {
        void Write(IRequestInfo requestInfo, IRequestDetail requestDetail);
    }
}