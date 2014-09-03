using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    [Serializable]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, IXpandLogonParameters, ICustomLogonParameter {
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
            storage.SaveOption("", "UserName", UserName);
            storage.SaveOption("", "Password", Password);
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}