using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
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

        protected void CreateDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName) {
            XPClassInfo info = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(classType);
            if (info.FindMember(propertyName) == null) {
                info.CreateMember(propertyName, typeof(XPCollection), true);
                typesInfo.RefreshInfo(classType);
            }
        }

        public new static XafApplication Application {
            get {
                return _instanceXafApplicationManager != null ? _instanceXafApplicationManager.Value : null;
            }
            set { _instanceXafApplicationManager.Value = value; }
        }

        protected XafApplication BaseApplication {
            get { return base.Application; }
        }

        static XPDictionary _dictiorary=XafTypesInfo.XpoTypeInfoSource.XPDictionary;
        static List<object> _storeManagers=new List<object>();


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
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            InitializeInstanceXafApplicationManager();
            var type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelMember)).FindMember("Type");
            var attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);

            type = (BaseInfo) typesInfo.FindTypeInfo(typeof(IModelBOModelClassMembers));
            attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            DisposeManagers();
        }

        public static void ReStoreManagers() {
            _instanceXafApplicationManager.Value = (XafApplication) _storeManagers[0];
            _instanceModelApplicationCreatorManager.Value = (ModelApplicationCreator) _storeManagers[2];
            _instanceModelApplicationCreatorPropertiesManager.Value =(ModelApplicationCreatorProperties) _storeManagers[1];
        }


        public static  void DisposeManagers() {
            _storeManagers = new List<object>{_instanceXafApplicationManager.Value};
            if (_instanceModelApplicationCreatorPropertiesManager!=null) {
                _storeManagers.Add(_instanceModelApplicationCreatorPropertiesManager.Value);
                _instanceModelApplicationCreatorPropertiesManager.Value = null;
            }
            if (_instanceModelApplicationCreatorManager!=null) {
                _storeManagers.Add(_instanceModelApplicationCreatorManager.Value);
                _instanceModelApplicationCreatorManager.Value = null;
            }
            _instanceXafApplicationManager.Value = null;
            
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