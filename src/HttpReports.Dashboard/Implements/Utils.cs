using System;

namespace HttpReports.Dashboard.Implements
{
    public static class Utils
    {
        public static bool ObjToBool(object expression, bool defValue = false)
        {
            if (expression != null)
            {
                bool result = false;
                bool.TryParse(expression.ToString(), out result);
                return result;
            }
            return defValue;
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToStandardTime(this DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss");
    }
}