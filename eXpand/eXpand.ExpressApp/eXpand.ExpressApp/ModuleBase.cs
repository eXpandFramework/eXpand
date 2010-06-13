using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp {
    public abstract class ModuleBase : DevExpress.ExpressApp.ModuleBase {
        static readonly object _lockObject = new object();
        static IValueManager<XafApplication> _instanceXafApplicationManager;
        static IValueManager<ModelApplicationCreatorProperties> _instanceModelApplicationCreatorPropertiesManager;
        static IValueManager<ModelApplicationCreator> _instanceModelApplicationCreatorManager;

        public static ModelApplicationCreatorProperties ModelApplicationCreatorProperties{
            get { return _instanceModelApplicationCreatorPropertiesManager.Value; }
        }
        public static ModelApplicationCreator ModelApplicationCreator
        {
            get { return _instanceModelApplicationCreatorManager.Value; }
        }

        public new static XafApplication Application {
            get { return _instanceXafApplicationManager.Value; }
        }
        protected override void CustomizeModelApplicationCreatorProperties(ModelApplicationCreatorProperties creatorProperties)
        {
            base.CustomizeModelApplicationCreatorProperties(creatorProperties);
            lock (_lockObject){
                if (_instanceModelApplicationCreatorPropertiesManager == null)
                    _instanceModelApplicationCreatorPropertiesManager = ValueManager.CreateValueManager<ModelApplicationCreatorProperties>();
                if (_instanceModelApplicationCreatorPropertiesManager.Value == null)
                    _instanceModelApplicationCreatorPropertiesManager.Value = creatorProperties;
            }
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            lock (_lockObject) {
                if (_instanceXafApplicationManager == null)
                    _instanceXafApplicationManager = ValueManager.CreateValueManager<XafApplication>();
                if (_instanceXafApplicationManager.Value == null)
                    _instanceXafApplicationManager.Value = application;
            }
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            lock (_lockObject) {
                if (_instanceModelApplicationCreatorManager == null)
                    _instanceModelApplicationCreatorManager = ValueManager.CreateValueManager<ModelApplicationCreator>();
                if (_instanceModelApplicationCreatorManager.Value == null)
                    _instanceModelApplicationCreatorManager.Value = ((ModelApplicationBase)Application.Model).CreatorInstance;
            }
        }
    }
}