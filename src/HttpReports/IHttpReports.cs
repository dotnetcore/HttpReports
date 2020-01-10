using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    public interface IHttpReports
    {
        void Invoke(HttpContext context, double Milliseconds, IConfiguration configuration);

        void Init(IConfiguration config);
    }
}