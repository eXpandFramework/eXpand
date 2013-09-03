using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security {
    public abstract class XpandSecurityModuleBase:XpandModuleBase {
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            if (IsHosted)
                ((XafApplication) sender).CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }

        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (((IModelOptionsRegistration) Application.Model.Options).Registration.Enabled)
                AddRegistrationControllers(sender, e);
        }

        protected virtual void AddRegistrationControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            var app = (XafApplication) sender;
            e.Controllers.Add(app.CreateController<ManageUsersOnLogonController>());
            e.Controllers.Add(app.CreateController<DevExpress.ExpressApp.Validation.ActionValidationController>());
            e.Controllers.Add(app.CreateController<DevExpress.ExpressApp.SystemModule.DialogController>());
        }
    }
    [ModelAbstractClass]
    public interface IModelOptionsRegistration:IModelOptions {
        IModelRegistration Registration { get;  }
    }

    public interface IModelRegistration:IModelNode {
        bool Enabled { get; set; }
    }
}
