using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication, IXafApplication {
        ApplicationModulesManager _applicationModulesManager;
        string IXafApplication.ConnectionString { get; set; }

        IDataStore IXafApplication.GetDataStore(IDataStore dataStore) {
            return null;
        }


        public new string ConnectionString {
            get { return base.ConnectionString; }
            set {
                base.ConnectionString = value;
                ((IXafApplication)this).ConnectionString = value;
            }
        }

        string IXafApplication.RaiseEstablishingConnection() {
            return this.GetConnectionString();
        }

        protected override ApplicationModulesManager CreateApplicationModulesManager(ControllersManager controllersManager) {
            _applicationModulesManager = base.CreateApplicationModulesManager(controllersManager);
            return _applicationModulesManager;
        }

        ApplicationModulesManager IXafApplication.ApplicationModulesManager {
            get { return _applicationModulesManager; }
        }
        public event EventHandler UserDifferencesLoaded;

        SettingsStorage IXafApplication.CreateLogonParameterStoreCore() {
            throw new NotImplementedException();
        }

        void IXafApplication.WriteLastLogonParameters(DetailView view, object logonObject) {
            throw new NotImplementedException();
        }

        event CancelEventHandler IXafApplication.ConfirmationRequired {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<ViewShownEventArgs> IXafApplication.AfterViewShown {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public void OnAfterViewShown(Frame frame, Frame sourceFrame) {
            throw new NotImplementedException();
        }

        string IXafApplication.GetModelAssemblyFilePath() {
            return GetModelAssemblyFilePath();
        }


        protected virtual void OnUserDifferencesLoaded(EventArgs e) {
            EventHandler handler = UserDifferencesLoaded;
            if (handler != null) handler(this, e);
        }

    }
}
