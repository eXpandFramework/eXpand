using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Fasterflect;
using Xpand.Persistent.Base.Validation;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Security {
    public abstract class XpandSecurityModuleBase:XpandModuleBase {
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            ((XafApplication) sender).CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }

        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (((IModelOptionsRegistration) Application.Model.Options).Registration.Enabled)
                AddRegistrationControllers(sender, e);
        }

        protected virtual void AddRegistrationControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            var app = (XafApplication) sender;
            var typeInfo = app.TypesInfo.FindTypeInfo(typeof (IPasswordScoreController)).Implementors.FirstOrDefault();
            if (typeInfo!=null)
                e.Controllers.Add(app.CreateController(typeInfo.Type));

            e.Controllers.Add(app.CreateController<ActionAppearanceController>());
            e.Controllers.Add(app.CreateController<AppearanceController>());
            e.Controllers.Add(app.CreateController<DetailViewItemAppearanceController>());
            e.Controllers.Add(app.CreateController<DetailViewLayoutItemAppearanceController>());
            e.Controllers.Add(app.CreateController<RefreshAppearanceController>());
            e.Controllers.Add(app.CreateController<AppearanceCustomizationListenerController>());

            e.Controllers.Add(app.CreateController<ManageUsersOnLogonController>());
            
            e.Controllers.Add(app.CreateController<ActionValidationController>());
            e.Controllers.Add(app.CreateController<PersistenceValidationController>());
            e.Controllers.Add(app.CreateController<ResultsHighlightController>());
            e.Controllers.Add(app.CreateController<RuleSetInitializationController>());

            e.Controllers.Add(app.CreateController<DialogController>());

        }
    }
    
}
