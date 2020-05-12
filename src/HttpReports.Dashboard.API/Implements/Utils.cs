using System;

namespace HttpReports.Dashboard.Implements
{
    public static class Utils
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToStandardTime(this DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss");
    }
}