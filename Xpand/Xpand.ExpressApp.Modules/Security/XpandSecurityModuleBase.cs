using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Validation;
using ChooseDatabaseAtLogonController = Xpand.ExpressApp.Security.Controllers.ChooseDatabaseAtLogonController;

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
            var controllers=new Dictionary<Type,Controller>();
            if (((IModelOptionsRegistration) Application.Model.Options).Registration.Enabled)
                AddRegistrationControllers((XafApplication)sender, controllers);
            var dbServerParameter = SecuritySystem.LogonParameters as IDBServerParameter;
            if (dbServerParameter != null){
                AddControllers(controllers,Application.CreateAppearenceControllers());
            }
            if (((IModelOptionsChooseDatabaseAtLogon) Application.Model.Options).ChooseDatabaseAtLogon){
                AddControllers(controllers, Application.CreateValidationControllers().Concat(new[] { Application.CreateController<ChooseDatabaseAtLogonController>() }));
            }
            e.Controllers.AddRange(controllers.Select(pair => pair.Value));
        }

        private void AddControllers(Dictionary<Type, Controller> controllers, IEnumerable<Controller> controllersToAdd){
            foreach (var appearenceController in controllersToAdd) {
                if (!controllers.ContainsKey(appearenceController.GetType())){
                    controllers.Add(appearenceController.GetType(),appearenceController);
                }
            }
        }

        protected virtual void AddRegistrationControllers(XafApplication application, Dictionary<Type, Controller> controllers) {
            var registrationControllers = CreateRegistrationControllers(application).ToArray();
            var appearenceControllers = application.CreateAppearenceControllers();
            var validationControllers = application.CreateValidationControllers();
            foreach (var registrationController in registrationControllers.Concat(appearenceControllers).Concat(validationControllers)){
                controllers.Add(registrationController.GetType(),registrationController);
            }
        }

        public static IEnumerable<Controller> CreateRegistrationControllers(XafApplication app) {
            var typeInfo = app.TypesInfo.FindTypeInfo(typeof(IPasswordScoreController)).Implementors.FirstOrDefault();
            if (typeInfo != null)
                yield return app.CreateController(typeInfo.Type);

            yield return app.CreateController<ManageUsersOnLogonController>();
        }
    }
}
