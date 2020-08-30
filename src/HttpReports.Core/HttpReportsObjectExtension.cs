using System;

namespace HttpReports
{
    public static class HttpReportsObjectExtension
    {
        public static int ToInt(this string str)
        {
            int.TryParse(str, out var num);
            return num;
        }

        public static int ToInt(this double dou)
        {
            return Convert.ToInt32(dou);
        }

        public static bool IsInt(this string str)
        {
            return int.TryParse(str, out _);
        }

        public static bool IsNumber(this string str)
        {
            return double.TryParse(str, out _);
        }

        public static float ToFloat(this string str, int digits = -1)
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
                    return Math.Round(Convert.ToDouble(str), digits);
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

            if (DateTime.TryParse(str, out var time))
            {
                return time;
            }
            return DateTime.Now;
        }

        public static DateTime ToDateTime(this string str)
        {
            if (DateTime.TryParse(str, out var time))
            {
                return time;
            }
            return DateTime.Now;
        }

        public static DateTime ToDateTime(this object str)
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


        public static int ToInt(this long lo)
        {
            try
            {
                return Convert.ToInt32(lo);
            }
            catch (Exception)
            {
                return 0;
            }
        } 


    }
}