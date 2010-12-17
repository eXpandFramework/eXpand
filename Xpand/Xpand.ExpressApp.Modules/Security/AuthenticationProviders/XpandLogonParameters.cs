using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, ICustomObjectSerialize {
        [Index(2)]
        public bool RememberMe { get; set; }

        public void ReadPropertyValues(SettingsStorage storage) {
            UserName = storage.LoadOption("", "UserName");
            Password = storage.LoadOption("", "Password");
            RememberMe = storage.LoadBoolOption("", "RememberMe", false);
        }
        public void WritePropertyValues(SettingsStorage storage) {
            storage.SaveOption("", "UserName", UserName);
            storage.SaveOption("", "Password", RememberMe ? Password : "");
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}