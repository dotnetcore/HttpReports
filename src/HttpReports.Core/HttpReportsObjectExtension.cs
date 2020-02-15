using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public static class HttpReportsObjectExtension
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

        public static bool IsInt(this string str)
        {
            int i = 0;

            try
            {
                i = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
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


        public static float ToFloat(this string str,int digits = -1)
        {
            try
            {
                if (digits == -1)
                {
                    return Convert.ToSingle(str);
                }
                else
                {
                    return Convert.ToSingle(Math.Round(Convert.ToDouble(str), digits));
                } 
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static double ToDouble(this string str, int digits = -1)
        {
            try
            {
                if (digits == -1)
                {
                    return Convert.ToDouble(str);
                }
                else
                {
                    return Math.Round(Convert.ToDouble(str),digits);
                }
               
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

        public static DateTime? TryToDateTime(this string str)
        {
            if (str.IsEmpty())
            {
                return null;
            }

            try
            {
                return Convert.ToDateTime(str);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
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

    }
}
