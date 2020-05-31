using HttpReports.Core.Interface;
using HttpReports.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public PerformanceService(IOptions<HttpReportsOptions> options)
        {
            Options = options.Value;

            var process = Process.GetCurrentProcess();

            MemoryCounter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);

            CPUCounter = new PerformanceCounter("Process","% Processor Time", process.ProcessName);
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

        public double ProcessMemory() => MemoryCounter.NextValue() / 1024 / 1024; 

        private double ProcessCPU() => CPUCounter.NextValue() / Environment.ProcessorCount;

    }
}
