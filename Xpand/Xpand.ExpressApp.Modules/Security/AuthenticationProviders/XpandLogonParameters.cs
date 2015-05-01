using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    [Serializable]
    public class XpandLogonParameters : AuthenticationStandardLogonParameters, IXpandLogonParameters, ICustomLogonParameter, IDBServerParameter{
        private string _dbServer;
        SettingsStorage _storage;
        private bool _rememberMe;

        [RuleRequiredField]
        public string DBServer {
            get { return _dbServer; }
            set {
                _dbServer = value;
                RaisePropertyChanged("DBServer");
            }
        }
        
        [Index(2)]
        public bool RememberMe{
            get { return _rememberMe; }
            set{
                _rememberMe = value;
                RaisePropertyChanged("RememberMe");
            }
        }

        [Browsable(false)]
        public bool AutoAuthentication {
            get { return ApplicationHelper.Instance.Application != null && ((IModelOptionsAuthentication)ApplicationHelper.Instance.Application.Model.Options).Athentication.AutoAthentication.Enabled; }
        }

        [Browsable(false)]
        public bool ChooseDatabaseAtLogon {
            get { return ApplicationHelper.Instance.Application != null && ((IModelOptionsChooseDatabaseAtLogon)ApplicationHelper.Instance.Application.Model.Options).ChooseDatabaseAtLogon; }
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
            storage.SaveOption("", "Password", Password);
            storage.SaveOption("", "RememberMe", RememberMe.ToString());
        }
    }
}