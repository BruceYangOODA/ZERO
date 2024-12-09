using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZERO.Util
{
    public class ValidMsg
    { 
        public bool isValid { get; set; }
        public string MSG { get; set; }
    }
    public static class UtilDateValidate
    {
        public static ValidMsg CheckDateValidate(string date) 
        {
            ValidMsg result = new();
            result.isValid = true;
            if (date.Length != 8) 
            {
                result.isValid = false;
                result.MSG = "日期格式為YYYYMMDD";
                return result;
            }
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(4, 2));
            int day = int.Parse(date.Substring(6, 2));
            if (year < 1970) 
            {
                result.isValid = false;
                result.MSG = "日期格式為YYYYMMDD";
                return result;
            }
            if (month >12)
            {
                result.isValid = false;
                result.MSG = "日期格式為YYYYMMDD";
                return result;
            }
            if (day > 31)
            {
                result.isValid = false;
                result.MSG = "日期格式為YYYYMMDD";
                return result;
            }
            return result;
        }
    }
}
