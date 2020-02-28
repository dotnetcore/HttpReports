using System;

using Microsoft.AspNetCore.Http;

namespace HttpReports.Dashboard.Implements
{
    public static class CookieExtensions
    {
        public static void SetCookie(this HttpContext context, string key, string value, double minutes)
        {
            context.Response.Cookies.Append(key, value, new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public static void DeleteCookie(this HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public static string GetCookie(this HttpContext context, string key)
        {
            context.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }
    }
}