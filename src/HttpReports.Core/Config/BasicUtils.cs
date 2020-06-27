using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HttpReports.Core.Config
{
    public static class BasicUtils
    {
        public static string GetUniqueId()
        { 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes( Guid.NewGuid().ToString() )), 4, 8).Replace("-", "").ToLower();
            return val; 
        }


        public static string GetUniqueId(this HttpContext context)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(Guid.NewGuid().ToString())), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public static string GetTraceId(this HttpContext context)
        {  
            if (context.Items.ContainsKey(BasicConfig.ActiveTraceId))
            {
                return context.Items[BasicConfig.ActiveTraceId].ToString();
            }
            else
            {
                return string.Empty;
            } 
        }

        public static string GetTraceParentId(this HttpContext context)
        {
            if (context.Items.ContainsKey(BasicConfig.ActiveTraceParentId))
            {
                return context.Items[BasicConfig.ActiveTraceParentId].ToString();
            }
            else
            {
                return string.Empty;
            }
        }   

    }
}
