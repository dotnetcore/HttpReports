using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.IO;
using FlubuCore.Scripting;
using Newtonsoft.Json;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        [SolutionFileName]
        public string SolutionFileName { get; set; } = "HttpReports.sln";

        [FromArg("c")]
        [BuildConfiguration]
        public string Configuration { get; set; } = "Release";

        public string Version { get; set; } 

        public string NugetKey = "";

        public FullPath OutputDir => RootDirectory.CombineWith("output");

        public FullPath SourceDir => RootDirectory.CombineWith("src");

        protected override void ConfigureTargets(ITaskContext context)
        { 
            if (context.ScriptArgs.ContainsKey("version"))
            {
                Version = context.ScriptArgs["version"];
            }

            if (context.ScriptArgs.ContainsKey("key"))
            {
                NugetKey = context.ScriptArgs["key"];
            }  

            context.LogInfo("============================================"); 
            context.LogInfo($"NugetKey:{NugetKey} Version:{Version}");
            context.LogInfo("============================================"); 

            var clean = context.CreateTarget("Clean")
                .SetDescription("Clean's the solution") 
                
                .AddCoreTask(x => { 
                    
                        context.GetFiles(OutputDir, "*.*").Where(c => !c.FileName.Contains(Version)).ToList().ForEach(t => System.IO.File.Delete(t)); 
                    
                    return x.Clean(); 
                
                });

            var restore = context.CreateTarget("Restore")
                .SetDescription("Restore's the solution")
                .DependsOn(clean)
                .AddCoreTask(x => x.Restore());


            var build = context.CreateTarget("Build")
                .SetDescription("Build's the solution")
                .DependsOn(restore)
                .AddCoreTask(x => x.Build());

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


            var branch = context.BuildSystems().Travis().BranchName;

            var packs = context.GetFiles(OutputDir,"*.*").Where(x => x.FileName.Contains(Version));  



            var push = context.CreateTarget("push") 
               .SetDescription("Publishes nuget package.") 
               .DependsOn(pack)
               .ForEach(packs, (project, tagget) =>
               { 
                   tagget.AddCoreTask(x => x.NugetPush($"{OutputDir}/{project.FileName}")
                   .ServerUrl("https://www.nuget.org/api/v2/package").ApiKey(NugetKey));

               }); 


            var push2 = context.CreateTarget("push2")
              .SetDescription("Publishes nuget package.")
              .DependsOn(pack)
              .ForEach(packs, (project, tagget) =>
              {
                  tagget.AddCoreTask(x => x.NugetPush($"{OutputDir}/{project.FileName.Replace(".nupkg", ".snupkg")}")
                  .ServerUrl("https://www.nuget.org/api/v3/package").ApiKey(NugetKey));

              });


            context.CreateTarget("Default")
             .SetDescription("Runs all targets.")
             .SetAsDefault()
             .DependsOn(clean, restore, build, pack,push, push2); 

        }
    }
}
