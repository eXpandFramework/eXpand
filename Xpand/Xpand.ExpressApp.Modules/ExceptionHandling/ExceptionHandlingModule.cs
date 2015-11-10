using System;
using System.Configuration;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ExceptionHandling;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExceptionHandling {
    public class ExceptionHandlingModule : XpandModuleBase {
        public const string ExceptionHandling = "ExceptionHandling";

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode)
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ExceptionHandling");
            application.CreateCustomObjectSpaceProvider += ApplicationOnCreateCustomObjectSpaceProvider;
        }

        private void ApplicationOnCreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs createCustomObjectSpaceProviderEventArgs) {
            if (!(createCustomObjectSpaceProviderEventArgs.ObjectSpaceProviders.OfType<IXpandObjectSpaceProvider>().Any()))
                Application.CreateCustomObjectSpaceprovider(createCustomObjectSpaceProviderEventArgs);
        }

        protected void Log(Exception exception) {
            if (IsEnabled()) {
                Type findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<IExceptionObject>();

                var os = Application.CreateObjectSpace(findBussinessObjectType);

                var exceptionObject = ExceptionObjectBuilder.Create(os, exception, Application);

                if (exceptionObject == null)
                {
                    Tracing.Tracer.LogError("Could not create ExceptionObject");
                    return;
        }

                os.CommitChanges();
                
            }
        }

        bool IsEnabled() {
            return (ConfigurationManager.AppSettings[ExceptionHandling] + "").ToLower() == "true";
        }
    }
}