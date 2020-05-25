using System;
using System.Collections.Generic;
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

        [FromArg("c")]
        [BuildConfiguration]
        public string Configuration { get; set; } = "Release";

        public FullPath OutputDir => RootDirectory.CombineWith("output");

        public FullPath SourceDir => RootDirectory.CombineWith("src");

        protected override void ConfigureTargets(ITaskContext context)
        {
            var clean = context.CreateTarget("Clean")
                .SetDescription("Clean's the solution")
                .AddCoreTask(x => x.Clean());
                
           var build = context.CreateTarget("Build")
                .SetDescription("Build's the solution")
                .DependsOn(clean)
                .AddCoreTask(x => x.Build());

            var projectsToPack = context.GetFiles(SourceDir, "*/*.csproj");

            context.CreateTarget("Pack")
                .SetDescription("Packs and publishes nuget package.")
                .DependsOn(build)
                .ForEach(projectsToPack, (project, target) =>
                {
                    target.AddCoreTask(x => x.Pack()
                        .Project(project)
                        .IncludeSymbols()
                        .NoBuild()
                        .OutputDirectory(OutputDir));
                });
        }
    }
}
