using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string MD5(this string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "").ToLower();
        }

        [DebuggerStepThrough]
        public static DateTime ToDateTimeOrNow(this string value) => DateTime.TryParse(value, out var time) ? time : DateTime.Now;

        [DebuggerStepThrough]
        public static DateTime ToDateTimeOrDefault(this string value, Func<DateTime> func) => DateTime.TryParse(value, out var time) ? time : func();
    }
}