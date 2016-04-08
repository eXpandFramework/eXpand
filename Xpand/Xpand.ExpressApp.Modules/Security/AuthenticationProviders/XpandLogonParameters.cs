using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        [ValueConverter(typeof(StringObjectToStringConverter))]
        [DataSourceProperty("DBServers")]
        [RuleRequiredField(TargetContextIDs = ChooseDatabaseAtLogonController.DBServer)]
        public StringObject DBServer {
            get{
                return !string.IsNullOrEmpty(((IDBServerParameter) this).DBServer)
                    ? new StringObject(((IDBServerParameter) this).DBServer)
                    : null;
            }
            set{
                var dbServer = value!=null?value.Name:null;
                ((IDBServerParameter)this).DBServer = dbServer;
            }
        }

        [Browsable(false)]
        public IList<StringObject> DBServers {
            get{
                return ChooseDatabaseAtLogonController.GetConnectionStringSettings()
                    .Select(ChooseDatabaseAtLogonController.GetDbServerName).Select(s => new StringObject(s)).ToList();
            }
        }

        string IDBServerParameter.DBServer {
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
            if (storage is EncryptedSettingsStorage||CaptionHelper.ApplicationModel.IsHosted()){
                storage.SaveOption("", "Password", Password);
                storage.SaveOption("", "RememberMe", RememberMe.ToString());
            }
        }
    }

}