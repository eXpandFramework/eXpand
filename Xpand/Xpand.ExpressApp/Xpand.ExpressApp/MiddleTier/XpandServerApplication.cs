using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication, IXafApplication {
        ApplicationModulesManager _applicationModulesManager;
        

        IDataStore IXafApplicationDataStore.GetDataStore(IDataStore dataStore) {
            return null;
        }

        public XpandServerApplication(ISecurityStrategyBase securityStrategy) {
            Security = securityStrategy;
        }

        protected override void LoadUserDifferences() {
            base.LoadUserDifferences();
            OnUserDifferencesLoaded(EventArgs.Empty);
        }

        protected override void OnSetupComplete() {
            base.OnSetupComplete();
            var modelApplicationBase = ((ModelApplicationBase)Model);
            var afterSetup = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetup.Id = "After Setup";
            ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetup);
            var userDiff = modelApplicationBase.CreatorInstance.CreateModelApplication();
            userDiff.Id = "UserDiff";
            ModelApplicationHelper.AddLayer(modelApplicationBase, userDiff);
            OnUserDifferencesLoaded(EventArgs.Empty);
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }

        protected override ApplicationModulesManager CreateApplicationModulesManager(ControllersManager controllersManager) {
            _applicationModulesManager = base.CreateApplicationModulesManager(controllersManager);
            return _applicationModulesManager;
        }

        ApplicationModulesManager IXafApplication.ApplicationModulesManager {
            get { return _applicationModulesManager; }
        }

        public AutoCreateOption AutoCreateOption {
            get { return AutoCreateOption.DatabaseAndSchema; }

        }

        public event EventHandler UserDifferencesLoaded;

        SettingsStorage ISettingsStorage.CreateLogonParameterStoreCore() {
            throw new NotImplementedException();
        }

        void IXafApplication.WriteLastLogonParameters(DetailView view, object logonObject) {
            throw new NotImplementedException();
        }

        event CancelEventHandler IConfirmationRequired.ConfirmationRequired {
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

        string IXafApplication.ModelAssemblyFilePath {
            get { return GetModelAssemblyFilePath(); }
        }


        protected virtual void OnUserDifferencesLoaded(EventArgs e) {
            EventHandler handler = UserDifferencesLoaded;
            if (handler != null) handler(this, e);
        }

    }
}
