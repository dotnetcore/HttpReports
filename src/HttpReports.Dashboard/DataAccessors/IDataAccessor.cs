using System.Collections.Generic;

using HttpReports.Dashboard.Models;

namespace HttpReports.Dashboard.DataAccessors
{
    public interface IDataAccessor
    {
        string BuildSqlWhere(GetIndexDataRequest request);

        List<EchartPineDataModel> GetStatusCode(GetIndexDataRequest request);

        List<EchartPineDataModel> GetResponseTimePie(GetIndexDataRequest request);

        List<EchartPineDataModel> GetDayRequestTimes(GetIndexDataRequest request);

        List<EchartPineDataModel> GetDayResponseTime(GetIndexDataRequest request);

        List<string> GetNodes();

        GetIndexDataResponse GetIndexData(GetIndexDataRequest request);

        List<EchartPineDataModel> GetLatelyDayData(GetIndexDataRequest request);

        List<GetTopResponse> GetTopRequest(GetTopRequest request);

        string BuildTopWhere(GetTopRequest request);

        List<GetTopResponse> GetCode500Response(GetTopRequest request);

        List<EchartPineDataModel> GetTOPART(GetTopRequest request);

        List<RequestInfo> GetRequestList(GetRequestListRequest request, out int totalCount);

        List<EchartPineDataModel> GetMonthDataByYear(GetIndexDataRequest request);

        void AddJob(Models.Job job);

        void UpdateJob(Models.Job job);

        void DeleteJob(Models.Job job);

        List<Models.Job> GetJobs();

        Models.Job GetJob(int Id);

        CheckModel CheckRt(Models.Job job, int minute);

        CheckModel CheckHttp(Models.Job job, int minute);

        CheckModel CheckIP(Models.Job job, int minute);

        CheckModel CheckRequestCount(Models.Job job, int minute);
    }
}