using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcUserServer.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcUserServer
{
    public class Startup
    {
         
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddHttpReports().UseSQLServerStorage().UseGrpc();
            services.AddControllers();
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpReports();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcUserService>();

                endpoints.MapControllers();
            });
        }
    }
}
