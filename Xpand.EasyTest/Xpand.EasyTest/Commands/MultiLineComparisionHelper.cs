using System.Text;
using System.Text.RegularExpressions;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class MultiLineComparisionHelper{
        public static string[] GetLines(string actualValue){
            actualValue = actualValue.Replace("\r\n", "\n");
            return actualValue.Split(new[]{
                '\n'
            });
        }

        public string Compare(string commandName, Parameter param, string actualValue, string valueCaption){
            if (param.IsArray){
                string[] lines = GetLines(actualValue);
                if (param.Index < lines.Length)
                    actualValue = lines[param.Index].Trim();
                else
                    return string.Format("{0}: incorrect index (the '{1}' {2} does not contain row {3})",
                        (object) commandName, (object) param.Name, (object) valueCaption, (object) param.Index);
            }
            if (!CompareString(actualValue, param.Value) != param.IsEqual)
                return null;
            if (param.IsEqual)
                return string.Format("{0}: incorrect '{1}' {2} (expected value: '{3}', actual value: '{4}')",
                    (object) commandName, (object) param.Name, (object) valueCaption, (object) param.Value,
                    (object) actualValue);
            return string.Format("{0}: the '{1}' {2} should not be equal to '{3}'", (object) commandName,
                (object) param.Name, (object) valueCaption, (object) param.Value);
        }

        public static bool MultiCodingStringComparer(string strLeft, string strRight){
            return strLeft.Normalize(NormalizationForm.FormKD) == strRight.Normalize(NormalizationForm.FormKD);
        }

        private static string WildcardToRegex(string pattern){
            return "^" +
                   Regex.Escape(pattern)
                       .Replace("\\*", ".*")
                       .Replace("\\\\.*", "\\*")
                       .Replace("\\?", ".")
                       .Replace("\\\\.", "\\?") + "$";
        }

        public static bool CompareString(string actualValue, string paramValue){
            if (actualValue == null || paramValue == null)
                return actualValue == paramValue;
            string input = actualValue.Normalize(NormalizationForm.FormKD);
            string pattern = paramValue.Normalize(NormalizationForm.FormKD);
            bool flag = paramValue.Length > 1 && paramValue.StartsWith("'") && paramValue.EndsWith("'");
            if (input == pattern)
                return true;
            if (flag){
                pattern = pattern.Substring(1, paramValue.Length - 2).Replace("\\'", "'");
                if (input == pattern)
                    return true;
            }
            return new Regex(WildcardToRegex(pattern),RegexOptions.Multiline).Match(input).Success;
        }
    }
}