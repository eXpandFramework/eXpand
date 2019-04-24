using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Xpand.EasyTest{
    public static class ButtonLocalizations{
        private static readonly IDictionary<string, IDictionary<string, string>> _localizations =
            new Dictionary<string, IDictionary<string, string>>{
                {"de", new Dictionary<string, string>{{"Save", "Speichern"}, {"Open", "Öffnen"}}}
            };

        public static ushort UserDefaultUILanguage{
            get { return GetUserDefaultUILanguage(); }
        }

        public static string UserDefaultUILanguageTwoLetterISOName{
            get { return CultureInfo.GetCultureInfo(GetUserDefaultUILanguage()).TwoLetterISOLanguageName; }
        }

        [DllImport("kernel32.dll")]
        private static extern ushort GetUserDefaultUILanguage();

        public static string GetLocalizedButtonCaption(string name){
            IDictionary<string, string> localizations;
            if (!_localizations.TryGetValue(UserDefaultUILanguageTwoLetterISOName, out localizations)){
                return name;
            }
            string localizedName;
            return !localizations.TryGetValue(name, out localizedName) ? name : localizedName;
        }
    }
}