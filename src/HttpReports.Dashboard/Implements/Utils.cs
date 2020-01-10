namespace HttpReports.Dashboard.Implements
{
    public class Utils
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
    }
}