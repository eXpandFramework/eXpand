using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    [Serializable]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, IXpandLogonParameters, ICustomLogonParameter {
        SettingsStorage _storage;
        private bool _rememberMe;

        [Index(2)]
        public bool RememberMe{
            get { return _rememberMe; }
            set{
                _rememberMe = value;
                RaisePropertyChanged("RememberMe");
            }
        }

        [Browsable(false)]
        public SettingsStorage Storage {
            get { return _storage; }
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
            storage.SaveOption("", "UserName", UserName);
            if (!XpandModuleBase.IsHosted)
                storage.SaveOption("", "Password", Password);
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}