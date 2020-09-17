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

            var restore = context.CreateTarget("Restore")
                .SetDescription("Restore's the solution")
                .DependsOn(clean)
                .AddCoreTask(x => x.Restore());

            var build = context.CreateTarget("Build")
                .SetDescription("Build's the solution")
                .DependsOn(restore)
                .AddCoreTask(x => x.Build());

            var projectsToPack = context.GetFiles(SourceDir, "*/*.csproj");

           var pack =  context.CreateTarget("Pack")
                .SetDescription("Packs and publishes nuget package.")
                .DependsOn(build)
                .ForEach(projectsToPack, (project, target) =>
                {
                    target.AddCoreTask(x => x.Pack()
                        .Project(project) 
                        .NoBuild()
                        .OutputDirectory(OutputDir));
                }); 


            context.CreateTarget("Default")
             .SetDescription("Runs all targets.")
             .SetAsDefault()
             .DependsOn(clean, restore, build, pack);   


        }
    }
}
