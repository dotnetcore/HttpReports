using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.IO;
using FlubuCore.Scripting;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        [SolutionFileName]
        public string SolutionFileName { get; set; } = "HttpReports.sln";

        [FromArg("c", "Build configuration")]
        [BuildConfiguration]
        public string Configuration { get; set; } = "Release";

        [FromArg("version", "Build version")]
        public string Version { get; set; }

        [FromArg("key", "nuget key for publishing nuget packages.")]
        public string NugetKey { get; set; } = string.Empty;

        [FromArg("env", "dev or pro")]
        public string Environment { get; set; } = "pro";

        public FullPath OutputDir => RootDirectory.CombineWith("output");

        public FullPath SourceDir => RootDirectory.CombineWith("src");

        protected override void ConfigureTargets(ITaskContext context)
        {
            if (Environment == "dev")
            {
                Version = $"{Version}-preview-" + DateTime.Now.ToString("MMddHHmm");
            }

            context.LogInfo("============================================");
            context.LogInfo($"NugetKey:{NugetKey} Version:{Version}");
            context.LogInfo($"OutputDir:{OutputDir} SourceDir:{SourceDir}");
            context.LogInfo("============================================");


            var clean = context.CreateTarget("Clean")
                .SetDescription("Clean's the solution")
                .AddCoreTask(x => x.Clean()
                    .AddDirectoryToClean(OutputDir, true));

            var restore = context.CreateTarget("Restore")
                .SetDescription("Restore's the solution")
                .DependsOn(clean)
                .AddCoreTask(x => x.Restore());


            var build = context.CreateTarget("Build")
                .SetDescription("Build's the solution")
                .DependsOn(restore)
                .AddCoreTask(x => x.Build().Version(Version).FileVersion(Version).InformationalVersion(Version));

            var projectsToPack = context.GetFiles(SourceDir, "*/*.csproj");

            projectsToPack.AddRange(context.GetFiles("src/Storage", "*/*.csproj"));
            projectsToPack.AddRange(context.GetFiles("src/Diagnostics", "*/*.csproj")
                .Where(x => x.FileName.Contains("HttpReports.Diagnostic.AspNetCore") || x.FileName.Contains("HttpReports.Diagnostic.HttpClient")));


            var pack = context.CreateTarget("Pack")
                .SetDescription("Packs and publishes nuget package.")
                .DependsOn(build)
                .ForEach(projectsToPack, (project, target) =>
                {
                    target.AddCoreTask(x => x.Pack().PackageVersion(Version)
                        .Project(project)
                        .NoBuild()
                        .OutputDirectory(OutputDir));
                });

            var push = context.CreateTarget("push")
                .SetDescription("Publishes nuget package.")
                .DependsOn(pack)
                .Do(NugetPush);

            var push2 = context.CreateTarget("push2")
                .SetDescription("Publishes nuget package.")
                .DependsOn(pack)
                .Do(NugetPush2);

            context.CreateTarget("Default")
             .SetDescription("Runs all targets.")
             .SetAsDefault()
             .DependsOn(clean, restore, build, pack, push,push2);  
        }

        private void NugetPush(ITaskContext context)
        {
            var nugetPackages = context.GetFiles(OutputDir, "*.nupkg").Where(x => x.FileName.Contains(Version));

            foreach (var nugetPackage in nugetPackages)
            {
                context.CoreTasks().NugetPush(nugetPackage)
                    .ServerUrl("http://192.168.1.30:8003/api/v2/package")
                    .ApiKey(NugetKey)
                    .Execute(context);
            }
        }

        private void NugetPush2(ITaskContext context)
        {
            var nugetPackages = context.GetFiles(OutputDir, "*.snupkg").Where(x => x.FileName.Contains(Version));

            foreach (var nugetPackage in nugetPackages)
            {
                context.CoreTasks().NugetPush(nugetPackage)
                   .ServerUrl("http://192.168.1.30:8003")
                    .ApiKey(NugetKey)
                    .Execute(context);

            }

        }
    }
}
