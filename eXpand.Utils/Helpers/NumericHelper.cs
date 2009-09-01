using System.Globalization;
using System.Text.RegularExpressions;

namespace eXpand.Utils.Helpers
{
    /// <summary>
    /// Summary description for NumericHelper.
    /// </summary>
    public class NumericHelper
    {
        public static int ValInt32(string x)
        {
            var regex = new Regex("[-+]?\\b\\d+\\b", RegexOptions.Compiled);
            Match match = regex.Match(StringHelper.Trim(StringHelper.Trim(x)));
            if (match.Success)
                return int.Parse(match.Value, NumberFormatInfo.InvariantInfo);
            return 0;
        }

        public static double ValDouble(string x)
        {
            var regex = new Regex("[-+]?\\b(?:[0-9]*\\.)?[0-9]+\\b", RegexOptions.Compiled);
            Match match = regex.Match(StringHelper.Trim(StringHelper.Trim(x)));
            if (match.Success)
                return double.Parse(match.Value, NumberFormatInfo.InvariantInfo);
            return 0;
        }


        public static bool IsNumeric(string strString)
        {
            return Regex.IsMatch(strString, "\\A\\b\\d+\\b\\z");
        }
    }
}