using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Xpand.Utils.Helpers {
    /// <summary>
    /// Summary description for NumericHelper.
    /// </summary>
    public static class NumericHelper {
        public static bool GreaterOrEqual(this double value1, double value2, double unimportantDifference = 0.0001) {
            return value1.NearlyEquals(value2) || value1 > value2;
        }

        public static bool NearlyEquals(this double value1, double value2, double unimportantDifference = 0.0001) {
            return Math.Abs(value1 - value2) < unimportantDifference;
        }
        public static double Round(this double d, int decimals = 0) {
            return Math.Round(d, decimals);
        }

        public static decimal RoundNumber(this decimal d, int decimals = 0) {
            return Math.Round(d, decimals);
        }

        public static int ValInt32(this string x) {
            var regex = new Regex("[-+]?\\b\\d+\\b", RegexOptions.Compiled);
            Match match = regex.Match(x + "");
            if (match.Success)
                return int.Parse(match.Value, NumberFormatInfo.InvariantInfo);
            return 0;
        }

        public static double ValDouble(this string x) {
            var regex = new Regex("[-+]?\\b(?:[0-9]*\\.)?[0-9]+\\b", RegexOptions.Compiled);
            Match match = regex.Match(x + "");
            if (match.Success)
                return double.Parse(match.Value, NumberFormatInfo.InvariantInfo);
            return 0;
        }


        public static bool IsNumeric(this string strString) {
            return Regex.IsMatch(strString, "\\A\\b\\d+\\b\\z");
        }
    }
}