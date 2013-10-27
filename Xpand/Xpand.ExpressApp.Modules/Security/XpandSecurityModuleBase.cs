using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Validation;

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
            e.Controllers.AddRange(CreateRegistrationControllers(app));
            e.Controllers.Add(app.CreateController<RegistrationDialogController>());
        }

        public static IEnumerable<Controller> CreateRegistrationControllers(XafApplication app) {
            var typeInfo = app.TypesInfo.FindTypeInfo(typeof(IPasswordScoreController)).Implementors.FirstOrDefault();
            if (typeInfo != null)
                yield return app.CreateController(typeInfo.Type);

            yield return app.CreateController<ActionAppearanceController>();
            yield return app.CreateController<AppearanceController>();
            yield return app.CreateController<DetailViewItemAppearanceController>();
            yield return app.CreateController<DetailViewLayoutItemAppearanceController>();
            yield return app.CreateController<RefreshAppearanceController>();
            yield return app.CreateController<AppearanceCustomizationListenerController>();

            yield return app.CreateController<ManageUsersOnLogonController>();

            yield return app.CreateController<ActionValidationController>();
            yield return app.CreateController<PersistenceValidationController>();
            yield return app.CreateController<ResultsHighlightController>();
            yield return app.CreateController<RuleSetInitializationController>();
        }
    }

    public class RegistrationDialogController : DialogController {
    }
}
