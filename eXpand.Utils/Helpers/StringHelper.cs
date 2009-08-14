using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace eXpand.Utils.Helpers
{
    /// <summary>
    /// Summary description for StringHelper.
    /// </summary>
    public class StringHelper
    {
        private static readonly Regex isGuid =
            new Regex(
                @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
                RegexOptions.Compiled);

        /// <summary>
        /// it does not give an error for null strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Trim(string s)
        {
            return (s + "").Trim();
        }

        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string MakeFirstCharUpper(string s)
        {
            if (s.Length > 0)
            {
                string substring1 = s.Substring(0, 1).ToUpper();
                string substring2 = s.Substring(1);
                return substring1 + substring2;
            }
            return s;
        }

        public static object Format(object value, string formatString)
        {
            if (isDateTime(value as string))
                return DateTime.Parse((string) value).ToString(formatString);
            return value;
        }

        private static bool isDateTime(string value)
        {
            try
            {
                DateTime.Parse(value);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public static string Inject(string injectToString, int positionToInject, string stringToInject)
        {
            var builder = new StringBuilder();
            builder.Append(injectToString.Substring(0, positionToInject));
            builder.Append(stringToInject);
            builder.Append(injectToString.Substring(positionToInject));
            return builder.ToString();
        }

        public static bool IsGuid(string candidate)
        {
            if (candidate != null)
            {
                if (isGuid.IsMatch(candidate))
                {
                    return true;
                }
            }

            return false;
        }
    }
}