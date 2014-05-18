using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.MiddleTier {
    public class XpandServerApplication : ServerApplication, IXafApplication {
        public XpandServerApplication(ISecurityStrategyBase securityStrategy) {
            Security = securityStrategy;
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
            base.LoadUserDifferences();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }

        void IXafApplication.WriteLastLogonParameters(DetailView view, object logonObject) {
            throw new NotImplementedException();
        }

        event CancelEventHandler IConfirmationRequired.ConfirmationRequired {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        string IXafApplication.ModelAssemblyFilePath {
            get { return GetModelAssemblyFilePath(); }
        }

        public event EventHandler<WindowCreatingEventArgs> WindowCreating;

        protected virtual void OnWindowCreating(WindowCreatingEventArgs e) {
            EventHandler<WindowCreatingEventArgs> handler = WindowCreating;
            if (handler != null) handler(this, e);
        }
    }
}
