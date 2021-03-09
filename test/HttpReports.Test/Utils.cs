using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HttpReports.Test
{
    public static class Utils
    {
        public static string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

    }
}
