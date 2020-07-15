using HttpReports.Core.Config;
using HttpReports.Core.Interface;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Service
{
    public class PerformanceService : IPerformanceService
    {
        private PerformanceCounter MemoryCounter;

        private PerformanceCounter CPUCounter;

        private HttpReportsOptions Options;

        private bool SupportPerformanceCounter = true; 

        private double prevCpuTime;

        private ILogger<PerformanceService> _logger;

        public PerformanceService(IOptions<HttpReportsOptions> options,ILogger<PerformanceService> logger)
        {
            Options = options.Value;

            _logger = logger;

            var process = Process.GetCurrentProcess(); 

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    MemoryCounter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);

                    CPUCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                }
                catch (Exception ex)
                {
                    SupportPerformanceCounter = false;
                } 
            }
        }

        public Task<IPerformance> GetPerformance(string Instance)
        {
            IPerformance performance = new Performance {  

                 Service = Options.Service.IsEmpty() ? "Default": Options.Service,
                 Instance = Instance,
                 GCGen0 = Gen0CollectCount(),
                 GCGen1 = Gen1CollectCount(),
                 GCGen2 = Gen2CollectCount(),
                 HeapMemory = GCHeapMemory().ToString().ToDouble(2),
                 ThreadCount = GetThreadCount(),
                 PendingThreadCount = GetPendingThreadCount(),
                 ProcessMemory = ProcessMemory().ToString().ToDouble(2),
                 ProcessCPU = ProcessCPU().ToString().ToDouble(2),
                 CreateTime = DateTime.Now 
            }; 

            return Task.FromResult(performance); 
        }

        private int Gen0CollectCount() => GC.CollectionCount(0);

        private int Gen1CollectCount() => GC.CollectionCount(1);

        private int Gen2CollectCount() => GC.CollectionCount(2);

        private double GCHeapMemory() => GC.GetTotalMemory(false) / 1024 / 1024; 

        private int GetThreadCount()
        {
            ThreadPool.GetMaxThreads(out int max,out _);

            ThreadPool.GetAvailableThreads(out int current, out _);  

            return max - current;
        } 

        private int GetPendingThreadCount()
        { 
            return 0;
        }

        public double ProcessMemory()
        { 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (SupportPerformanceCounter)
                {
                    return MemoryCounter.NextValue() / 1024.00 / 1024.00;
                }  
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            { 
                return Process.GetCurrentProcess().WorkingSet64 / 1024.00 / 1024.00; 
            }

            return 0; 
        } 
      

        private double ProcessCPU() 
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (SupportPerformanceCounter)
                {
                    return CPUCounter.NextValue() / Convert.ToDouble(Environment.ProcessorCount);
                }
                else
                { 
                    var process = Process.GetCurrentProcess();  

                    //当前时间   
                    var curTime = process.TotalProcessorTime.TotalMilliseconds;

                    //间隔时间内的CPU运行时间除以逻辑CPU数量 
                    var value = (curTime - prevCpuTime) / Convert.ToDouble( BasicConfig.PerformanceInerval) / Convert.ToDouble(Environment.ProcessorCount) * 100.00;

                    prevCpuTime = curTime;

                    return value;
                } 
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var process = Process.GetCurrentProcess();

                //当前时间   
                var curTime = process.TotalProcessorTime.TotalMilliseconds;

                //间隔时间内的CPU运行时间除以逻辑CPU数量 
                var value = (curTime - prevCpuTime) / Convert.ToDouble(BasicConfig.PerformanceInerval) / Convert.ToDouble(Environment.ProcessorCount) * 100.00;

                prevCpuTime = curTime;

                return value;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return 0;
            } 

            return 0; 
        }

        private long _prevIdleTime;
        private long _prevTotalTime;
        public float CpuUsage;

        private double GetCpuUseLinux()
        {
            var cpuUsageLine = ReadLineStartingWith("/proc/stat", "cpu  ");

            if (string.IsNullOrWhiteSpace(cpuUsageLine))
            {
                _logger.LogWarning($"Couldn't read line from  linux ");
                return 0;
            }

            // Format: "cpu  20546715 4367 11631326 215282964 96602 0 584080 0 0 0"
            var cpuNumberStrings = cpuUsageLine.Split(' ').Skip(2);

            if (cpuNumberStrings.Any(n => !long.TryParse(n, out _)))
            {
                _logger.LogWarning($"Couldn't read line from  linux ");
                return 0;
            }

            var cpuNumbers = cpuNumberStrings.Select(long.Parse).ToArray();
            var idleTime = cpuNumbers[3];
            var iowait = cpuNumbers[4]; // Iowait is not real cpu time
            var totalTime = cpuNumbers.Sum() - iowait;

             
            var deltaIdleTime = idleTime - _prevIdleTime;
            var deltaTotalTime = totalTime - _prevTotalTime;

            var currentCpuUsage = (1.0f - deltaIdleTime / ((float)deltaTotalTime)) * 100f;

            var previousCpuUsage = CpuUsage;
            CpuUsage = (previousCpuUsage + 2 * currentCpuUsage) / 3; 

            return Convert.ToDouble(CpuUsage);
        }

        private double GetMemoryUse()
        { 
            var memTotalLine = ReadLineStartingWith("/proc/meminfo", "MemTotal");

            if (string.IsNullOrWhiteSpace(memTotalLine))
            {
                _logger.LogWarning($"Couldn't read 'MemTotal' line from Linux ");
                return 0;
            }

            // Format: "MemTotal:       16426476 kB"
            if (!long.TryParse(new string(memTotalLine.Where(char.IsDigit).ToArray()), out var totalMemInKb))
            {
                _logger.LogWarning($"Couldn't read 'MemTotal' line from Linux ");
                return 0;
            }

            return totalMemInKb / Convert.ToDouble(1024);  
        } 

        private static string ReadLineStartingWith(string path, string lineStartsWith)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (var r = new StreamReader(fs, Encoding.ASCII))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.StartsWith(lineStartsWith))
                        return line;
                }
            }

            return null;
        }  
    }
}
