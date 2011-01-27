using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp {
    public class XpandModuleBase : ModuleBase {
        static readonly object _lockObject = new object();
        static IValueManager<XafApplication> _instanceXafApplicationManager;
        static IValueManager<ModelApplicationCreatorProperties> _instanceModelApplicationCreatorPropertiesManager;
        static IValueManager<ModelApplicationCreator> _instanceModelApplicationCreatorManager;
        public static object Control;

        public static ModelApplicationCreatorProperties ModelApplicationCreatorProperties {
            get { return _instanceModelApplicationCreatorPropertiesManager.Value; }
        }
        public static ModelApplicationCreator ModelApplicationCreator {
            get {
                return _instanceModelApplicationCreatorManager != null ? _instanceModelApplicationCreatorManager.Value : null;
            }
            set {
                if (_instanceModelApplicationCreatorManager != null)
                    _instanceModelApplicationCreatorManager.Value = value;
            }
        }

        public new static XafApplication Application {
            get {
                return _instanceXafApplicationManager != null ? _instanceXafApplicationManager.Value : null;
            }
        }

        protected XafApplication BaseApplication {
            get { return base.Application; }
        }

        static XPDictionary _dictiorary=XafTypesInfo.XpoTypeInfoSource.XPDictionary;
        protected string ConnectionString;

        public static XPDictionary Dictiorary {
            get { return _dictiorary; }
            set { _dictiorary = value; }
        }

        public BusinessClassesList GetAdditionalClasses(ApplicationModulesManager manager) {
            var moduleList = manager.Modules;
            var businessClassesList = new BusinessClassesList(moduleList.SelectMany(@base => @base.AdditionalBusinessClasses));
            businessClassesList.AddRange(moduleList.SelectMany(moduleBase => moduleBase.BusinessClassAssemblies.GetBusinessClasses()));
            return businessClassesList;
        }
        public BusinessClassesList GetAdditionalClasses(ModuleList moduleList) {
            var businessClassesList = new BusinessClassesList(moduleList.SelectMany(@base => @base.AdditionalBusinessClasses));
            businessClassesList.AddRange(moduleList.SelectMany(moduleBase => moduleBase.BusinessClassAssemblies.GetBusinessClasses()));
            return businessClassesList;
        }

        protected override void CustomizeModelApplicationCreatorProperties(ModelApplicationCreatorProperties creatorProperties) {
            base.CustomizeModelApplicationCreatorProperties(creatorProperties);
            lock (_lockObject) {
                if (_instanceModelApplicationCreatorPropertiesManager == null)
                    _instanceModelApplicationCreatorPropertiesManager = ValueManager.CreateValueManager<ModelApplicationCreatorProperties>();
                if (_instanceModelApplicationCreatorPropertiesManager.Value == null || _instanceModelApplicationCreatorPropertiesManager.Value != creatorProperties)
                    _instanceModelApplicationCreatorPropertiesManager.Value = creatorProperties;
            }
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            InitializeInstanceXafApplicationManager();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            InitializeInstanceXafApplicationManager();
            application.SetupComplete += ApplicationOnSetupComplete;
            application.CreateCustomObjectSpaceProvider +=
                (sender, args) => ConnectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
        }
        string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            InitializeInstanceXafApplicationManager();
            var type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelMember)).FindMember("Type");
            var attribute = type.FindAttribute<ReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            _instanceXafApplicationManager = null;
            _instanceModelApplicationCreatorManager = null;
            _instanceXafApplicationManager = null;
        }
        void InitializeInstanceXafApplicationManager() {
            lock (_lockObject) {
                if (_instanceXafApplicationManager == null)
                    _instanceXafApplicationManager = ValueManager.CreateValueManager<XafApplication>();
                if (_instanceXafApplicationManager.Value == null)
                    _instanceXafApplicationManager.Value = base.Application;
            }
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