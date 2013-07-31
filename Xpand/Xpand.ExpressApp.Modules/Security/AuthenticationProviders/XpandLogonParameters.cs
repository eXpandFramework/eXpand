using System.ComponentModel;
using System.Diagnostics;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, IXpandLogonParameters {
        SettingsStorage _storage;

        [Index(2)]
        public bool RememberMe { get; set; }
        [Browsable(false)]
        public SettingsStorage Storage {
            get { return _storage; }
        }

        [Browsable(false)]
        public bool AutoAuthentication {
            get { return ApplicationHelper.Instance.Application!=null && ((IModelOptionsAuthentication)ApplicationHelper.Instance.Application.Model.Options).Athentication.AutoAthentication.Enabled; }
        }
        
        void ICustomObjectSerialize.ReadPropertyValues(SettingsStorage storage) {
            ReadPropertyValuesCore(storage);
        }

        protected void ReadPropertyValuesCore(SettingsStorage storage) {
            _storage=storage;
            UserName = storage.LoadOption("", "UserName");
            Password = storage.LoadOption("", "Password");
            RememberMe = storage.LoadBoolOption("", "RememberMe", false);
        }

        void ICustomObjectSerialize.WritePropertyValues(SettingsStorage storage) {
            WritePropertyValuesCore(storage);
        }

        protected void WritePropertyValuesCore(SettingsStorage storage) {
            Debug.Assert(storage.GetType() == _storage.GetType());
            storage.SaveOption("", "UserName", UserName);
            storage.SaveOption("", "Password", Password);
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}