using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.TranslatorProviders {
    public class GoogleTranslatorProvider : TranslatorProviderBase {
        private static readonly string[] GoogleLanguages = 
        {"sq", "ar", "bg", "zh", "ca", "hr", "cs", "da", "nl", "en", 
         "et", "fi", "fr", "gl", "de", "el", "he", "hi", "hu", "id", 
         "it", "ja", "co", "lv", "lt", "mt", "nn", "fa", "pl", "pt", 
         "ro", "ru", "es", "sr", "sk", "sl", "sl", "sv", "th", "tr", 
         "uk", "vi"};

        public GoogleTranslatorProvider() : base("<br />", 5000) { }
        internal static IEnumerable<string> CalculatePairSeparatorsBlocks(string text) {
            string[] leftSeparators = new string[] { "\"", "^'", " '", "('", "{" };
            string[] rightSeparators = new string[] { "\"", "'", "'", "'", "}" };
            int index = 0;
            int leftSeparatorIndex = 0;
            int rightSeparatorIndex = 0;
            int iSeparator = 0;
            int leftSeparatorSize = 0;
            while (index < text.Length) {
                leftSeparatorIndex = -1;
                for (int i = 0; i < leftSeparators.Length; i++) {
                    int separatorIndex = -1;
                    if (leftSeparators[i][0] == '^') {
                        separatorIndex = text.IndexOf(leftSeparators[i].Substring(1), index);
                        if (separatorIndex == 0) {
                            iSeparator = i;
                            leftSeparatorIndex = separatorIndex;
                            leftSeparatorSize = leftSeparators[i].Length - 1;
                        }
                    } else {
                        separatorIndex = text.IndexOf(leftSeparators[i], index);
                        if (separatorIndex >= 0 &&
                            (leftSeparatorIndex < 0 || separatorIndex < leftSeparatorIndex)) {
                            iSeparator = i;
                            leftSeparatorIndex = separatorIndex;
                            leftSeparatorSize = leftSeparators[i].Length;
                        }
                    }
                }
                if (leftSeparatorIndex >= 0) {
                    rightSeparatorIndex = text.IndexOf(
                        rightSeparators[iSeparator], leftSeparatorIndex + leftSeparatorSize);
                    if (rightSeparatorIndex >= 0) {
                        string result = text.Substring(index, leftSeparatorIndex - index).Trim();
                        if (result.Length > 0) {
                            yield return result;
                        }
                        index = rightSeparatorIndex + rightSeparators[iSeparator].Length;
                        continue;
                    }
                }
                yield return text.Substring(index, text.Length - index).Trim();
                index = text.Length;
            }
        }
        protected string TranslateCore(
            string text, string sourceLanguageCode, string desinationLanguageCode) {
            Uri serviceUri = new Uri("http://ajax.googleapis.com/ajax/services/language/translate");
            UTF8Encoding encoding = new UTF8Encoding();
            string postData = "v=1.0";
            postData += ("&q=" + HttpUtility.UrlEncode(text));
            postData += ("&langpair=" + sourceLanguageCode + "|" + desinationLanguageCode);
            byte[] data = encoding.GetBytes(postData);
            System.Net.HttpWebRequest httpRequest =
                (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
            httpRequest.Timeout = 15000;
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            httpRequest.ContentLength = data.Length;
            using (Stream requestStream = httpRequest.GetRequestStream()) {
                requestStream.Write(data, 0, data.Length);
            }
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)httpRequest.GetResponse();
            String resp = null;
            using (StreamReader sReader = new StreamReader(response.GetResponseStream(), encoding)) {
                resp = sReader.ReadToEnd();
            }
            return resp;
        }
        internal static string GetPropertyValueFromJson(string text, string propertyName) {
            int index = text.IndexOf(propertyName);
            if (index < 0) {
                return null;
            }
            int indexColon = text.IndexOf(":", index);
            int indexValueStart = indexColon + 1;
            while (text[indexValueStart] == ' ') {
                indexValueStart++;
            }
            if (text[indexValueStart] == '"') {
                indexValueStart++;
                int indexValueEnd = text.IndexOf("\"", indexValueStart);
                return text.Substring(indexValueStart, indexValueEnd - indexValueStart);
            } else if (text[indexValueStart] >= '0' && text[indexValueStart] <= '9') {
                index = indexValueStart;
                string number = string.Empty;
                while (text[index] >= '0' && text[index] <= '9' && index < text.Length) {
                    number += text[index];
                    index++;
                }
                return number;
            }
            return null;
        }
        internal static string DecodeASCIIToUnicode(string text) {
            string result = text;
            int unicodeSymbolIndex = 0;
            while ((unicodeSymbolIndex = result.IndexOf("\\u")) >= 0) {
                string unicodeCharString = result.Substring(unicodeSymbolIndex, 6);
                string unicodeCharStringCode = result.Substring(unicodeSymbolIndex + 2, 4);
                char unicodeChar = (char)Convert.ToInt32(unicodeCharStringCode, 16);
                result = result.Replace(unicodeCharString, new string(unicodeChar, 1));
            }
            return result;
        }
        public override IEnumerable<string> CalculateSentences(string text) {
            List<string> sentences = new List<string>();
            IEnumerable<string> lineBlocks =
                text.Split(new string[] { Environment.NewLine, "\\r\\n", "\\n" },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string lineBlock in lineBlocks) {
                sentences.AddRange(CalculatePairSeparatorsBlocks(lineBlock));
            }
            return sentences;
        }
        #region ITranslatorProvider Members
        public override string Caption {
            get { return "Google Translate"; }
        }
        public override string Description {
            get { return "Powered by <b>Google Translate</b>"; }
        }
        public override string[] GetLanguages() {
            return GoogleLanguages;
        }
        public override string Translate(string text, string sourceLanguageCode,
            string desinationLanguageCode) {
            string jsonText = TranslateCore(text, sourceLanguageCode, desinationLanguageCode);
            string responseStatus = GetPropertyValueFromJson(jsonText, "responseStatus");
            if (responseStatus == "200") {
                string translatedText = GetPropertyValueFromJson(jsonText, "translatedText");
                string unicodeDecodedText = DecodeASCIIToUnicode(translatedText);
                return HttpUtility.HtmlDecode(unicodeDecodedText);
            } else return text;
        }
        #endregion
    }
}
