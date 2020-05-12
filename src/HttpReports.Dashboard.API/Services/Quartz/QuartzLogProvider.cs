using System;

using Microsoft.Extensions.Logging;

using ILogLevelQuartz = Quartz.Logging.LogLevel;
using ILogProvider = Quartz.Logging.ILogProvider;
using QuartzLogger = Quartz.Logging.Logger;

namespace HttpReports.Dashboard.Services.Quartz
{
    /// <summary>
    /// QuartzLogProvider，映射QuartzLog到Microsoft.Extensions.Logging
    /// </summary>
    public class QuartzLogProvider : ILogProvider
    {
        public ILoggerFactory LoggerFactory { get; }

        public QuartzLogProvider(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        public QuartzLogger GetLogger(string name) => (level, func, exception, parameters) =>
        {
            if (func is null)
            {
                return true;
            }

            var logger = LoggerFactory.CreateLogger(name);

            var message = string.Format(func(), parameters);
            switch (level)
            {
                case ILogLevelQuartz.Trace:
                    logger.LogTrace(exception, message);
                    break;

                case ILogLevelQuartz.Debug:
                    logger.LogDebug(exception, message);
                    break;

                case ILogLevelQuartz.Warn:
                    logger.LogWarning(exception, message);
                    break;

                case ILogLevelQuartz.Error:
                    logger.LogError(exception, message);
                    break;

                case ILogLevelQuartz.Fatal:
                    logger.LogCritical(exception, message);
                    break;

                case ILogLevelQuartz.Info:
                default:
                    logger.LogInformation(exception, message);
                    break;
            }

            return true;
        };

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}