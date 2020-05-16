using HttpReports.Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Handle
{
    public abstract class DashboardHandleBase : IDashboardHandle
    {
        protected DashboardHandleBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ViewData = new Dictionary<string, object>();
        }

        public DashboardContext Context { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public Dictionary<string, object> ViewData { get; set; }



        public virtual async Task<string> View(object model = null, Type viewType = null)
        {
            Context.HttpContext.Response.ContentType = "text/html"; 
            ViewData["View"] = Context.Route.View; 
           
            var view =  ServiceProvider.GetRequiredService(viewType ?? Context.Route.View) as RazorPage;

            if (view == null)
            {
                throw new ArgumentException("view not found");
            }

            if (model != null)
            {
                ViewData["Model"] = model;
            }

            view.Context = Context;
            view.ViewData = ViewData;
            return await Task.FromResult(view.ToString());
        }


        public virtual string Json(object model)
        {
            Context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
            return JsonConvert.SerializeObject(model, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}
