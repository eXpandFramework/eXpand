using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Microsoft.Win32;

namespace Xpand.Persistent.Base.Security{
    public class EncryptedSettingsStorage : SettingsStorage{
        public static char KeyValueDelimiter = '\t';
        public static char PairDelimiter = '\n';
        private readonly NameValueCollection _values = new NameValueCollection();
        private readonly string _key;

        public EncryptedSettingsStorage(){
            string wow=Environment.Is64BitOperatingSystem?"Wow6432Node":null;
            _key = @"Software\" + wow + @"\" + CaptionHelper.ApplicationModel.Title + @"\Entropy";
            Registry.CurrentUser.SetValue(_key, new byte[20], RegistryValueKind.Binary);
        }

        public NameValueCollection Values{
            get { return _values; }
        }

        public override bool IsPathExist(string optionPath){
            return string.IsNullOrEmpty(_values[optionPath]);
        }

        public override void SaveOption(string optionPath, string optionName, string optionValue){
            _values[optionPath + "\\" + optionName] = optionValue;
        }

        public override string LoadOption(string optionPath, string optionName){
            return _values[optionPath + "\\" + optionName];
        }

        public byte[] GetContent(){
            var sb = new StringBuilder();
            foreach (string key in _values.Keys){
                sb.Append(key);
                sb.Append(KeyValueDelimiter);
                sb.Append(_values[key]);
                sb.Append(PairDelimiter);
            }
            if (sb.Length > 0){
                sb.Remove(sb.Length - 1, 1);
            }
            return Encrypt(sb);
        }

        private byte[] Encrypt(StringBuilder sb){
            byte[] plaintext = Encoding.UTF8.GetBytes(sb.ToString());
            var entropy = (byte[]) Registry.CurrentUser.GetValue(_key);
            return ProtectedData.Protect(plaintext, entropy,DataProtectionScope.CurrentUser);
        }

        public void SetContents(byte[] bytes){
            var entropy = (byte[])Registry.CurrentUser.GetValue(_key, new byte[0]);
            byte[] plaintext = ProtectedData.Unprotect(bytes, entropy, DataProtectionScope.CurrentUser);
            var str = Encoding.UTF8.GetString(plaintext);
            try{
                _values.Clear();
                string[] pairs = str.Split(PairDelimiter);
                foreach (string pair in pairs){
                    if (!string.IsNullOrEmpty(pair)){
                        try{
                            string[] keyValue = pair.Split(KeyValueDelimiter);
                            _values.Add(keyValue[0], keyValue[1]);
                        }
                        catch (Exception e){
                            Tracing.Tracer.LogSubSeparator("Error occurs on parsing key-value string: '" + pair + "'");
                            Tracing.Tracer.LogError(e);
                        }
                    }
                }
            }
            catch (Exception e){
                Tracing.Tracer.LogSubSeparator("Exception occurs on parsing string:");
                Tracing.Tracer.LogValue("value", str);
                Tracing.Tracer.LogError(e);
            }
        }
    }
}