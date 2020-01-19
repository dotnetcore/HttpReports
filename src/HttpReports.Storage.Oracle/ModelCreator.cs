using HttpReports.Monitor;

namespace HttpReports.Storage.Oracle
{
    internal class ModelCreator : IModelCreator
    {
        public IMonitor CreateMonitor(MonitorType type)
        {
            throw new System.NotImplementedException();
        }

        public IMonitorRule CreateMonitorRule()
        {
            throw new System.NotImplementedException();
        }

        public IRequestInfo CreateRequestInfo()
        {
            throw new System.NotImplementedException();
        }

        public IRequestInfo NewRequestInfo() => new RequestInfo();
    }
}