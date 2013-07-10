using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, IXpandLogonParameters {

        [Index(2)]
        public bool RememberMe { get; set; }

        void ICustomObjectSerialize.ReadPropertyValues(SettingsStorage storage) {
            ReadPropertyValuesCore(storage);
        }

        protected void ReadPropertyValuesCore(SettingsStorage storage) {
            UserName = storage.LoadOption("", "UserName");
            Password = storage.LoadOption("", "Password");
            RememberMe = storage.LoadBoolOption("", "RememberMe", false);
        }

        void ICustomObjectSerialize.WritePropertyValues(SettingsStorage storage) {
            WritePropertyValuesCore(storage);
        }

        protected void WritePropertyValuesCore(SettingsStorage storage) {
            storage.SaveOption("", "UserName", UserName);
            storage.SaveOption("", "Password", Password);
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}