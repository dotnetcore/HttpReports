
using HttpReports.Dashboard.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HttpReports.Dashboard.Handles
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


        public virtual string Json(bool state)
        {
            BaseResult baseResult = new BaseResult
            {
                Code = state ? 1 : -1,
                Msg = state ? "ok" : "error",
                Data = null
            };

            Context.HttpContext.Response.ContentType = "application/json;charset=utf-8";

            var setting = ServiceProvider.GetRequiredService<JsonSerializerOptions>();
            return System.Text.Json.JsonSerializer.Serialize(baseResult,setting);  
            
        }

        public virtual string Json(bool state,object data)
        {
            BaseResult baseResult = new BaseResult
            {
                Code = state ? 1 : -1,
                Msg = state ? "ok" : "error",
                Data = data
            };

            Context.HttpContext.Response.ContentType = "application/json;charset=utf-8";

            var setting = ServiceProvider.GetRequiredService<JsonSerializerOptions>(); 
            return System.Text.Json.JsonSerializer.Serialize(baseResult, setting);  
        } 

        public virtual string Json(bool state, string msg,object data)
        {
            BaseResult baseResult = new BaseResult
            {
                Code = state ? 1 : -1,
                Msg = state ? "ok" : msg,
                Data = data
            };

            var setting = ServiceProvider.GetRequiredService<JsonSerializerOptions>(); 
            return System.Text.Json.JsonSerializer.Serialize(baseResult, setting); 
        }  
    } 
    

    public class SnowFlakeIdConverter : System.Text.Json.Serialization.JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = 0L;
            return reader.TryGetInt64(out val) ? val : 0L;
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
