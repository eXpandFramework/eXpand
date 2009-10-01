﻿using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace eXpand.Utils.Helpers
{
    /// <summary>
    /// Summary description for StringHelper.
    /// </summary>
    public static class StringHelper
    {
        private static readonly Regex isGuid =
            new Regex(
                @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
                RegexOptions.Compiled);

        public static string XMLEncode(this string Value)
        {
            return Value.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string XMLDecode(this string Value)
        {
            return Value.Replace("&amp;", "&").Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">");
        }
        public static String RemoveDiacritics(this String s)
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

        public static string MakeFirstCharUpper(this string s)
        {
            if (s.Length > 0)
            {
                string substring1 = s.Substring(0, 1).ToUpper();
                string substring2 = s.Substring(1);
                return substring1 + substring2;
            }
            return s;
        }



        public static string Inject(this string injectToString, int positionToInject, string stringToInject)
        {
            var builder = new StringBuilder();
            builder.Append(injectToString.Substring(0, positionToInject));
            builder.Append(stringToInject);
            builder.Append(injectToString.Substring(positionToInject));
            return builder.ToString();
        }

        public static bool IsGuid(this string candidate)
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