using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static int ToInt(this string str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public static int ToInt(this double dou)
        {
            return Convert.ToInt32(dou); 
        }


        public static bool IsNumber(this string str)
        {
            double i = 0;

            try
            {
                i = Convert.ToDouble(str);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        } 
         
        public static string MD5(this string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "").ToLower();
        }

        public static double ToDouble(this string str)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool IsEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return true;
            }

            return false;
        }

        public static DateTime ToDateTime(this string str)
        {
            try
            {
                return Convert.ToDateTime(str);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        [DebuggerStepThrough]
        public static DateTime ToDateTimeOrNow(this string value) => DateTime.TryParse(value, out var time) ? time : DateTime.Now;

        [DebuggerStepThrough]
        public static DateTime ToDateTimeOrDefault(this string value, Func<DateTime> func) => DateTime.TryParse(value, out var time) ? time : func();
    }
}