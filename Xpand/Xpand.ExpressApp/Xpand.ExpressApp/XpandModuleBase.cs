using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp {
    [ToolboxItem(false)]
    public abstract class XpandModuleBase : ModuleBase {
        static List<object> _storeManagers;
        public static XPDictionary Dictiorary { get; set; }
        public static ITypesInfo TypesInfo { get; set; }
        public static string ManifestModuleName;
        static readonly object _lockObject = new object();
        static IValueManager<ModelApplicationCreator> _instanceModelApplicationCreatorManager;
        public static object Control;
        static Assembly _baseImplAssembly;

        static XpandModuleBase() {
            Dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
            TypesInfo = XafTypesInfo.Instance;
        }

        static void LoadBaseImplAssembly() {
            var assemblyString = "Xpand.Persistent.BaseImpl, Version=*, Culture=neutral, PublicKeyToken=*";
            var baseImplName = ConfigurationManager.AppSettings["Baseimpl"];
            if (!String.IsNullOrEmpty(baseImplName)) {
                assemblyString = baseImplName;
            }
            _baseImplAssembly = Assembly.Load(assemblyString);
            if (_baseImplAssembly == null)
                throw new NullReferenceException(
                    "BaseImpl not found please reference it in your front end project and set its Copy Local=true");
        }

        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        protected bool RuntimeMode {
            get {
                var devProcceses = new[] { "Xpand.ExpressApp.ModelEditor", "devenv" };
                return !devProcceses.Contains(Process.GetCurrentProcess().ProcessName) && LicenseManager.UsageMode != LicenseUsageMode.Designtime;
            }
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            if (IsLoadingExternalModel()) {
                var declaredExportedTypes = new List<Type>();
                var collectExportedTypesFromAssembly = CollectExportedTypesFromAssembly(GetType().Assembly).Where(IsExportedType);
                foreach (var type in collectExportedTypesFromAssembly) {
                    var typeInfo = TypesInfo.FindTypeInfo(type);
                    declaredExportedTypes.Add(type);
                    foreach (Type type1 in CollectRequiredTypes(typeInfo)) {
                        if (!declaredExportedTypes.Contains(type1))
                            declaredExportedTypes.Add(type1);
                    }
                }
                return declaredExportedTypes;
            }
            return base.GetDeclaredExportedTypes();
        }

        void AssignSecurityEntities() {
            var roleTypeProvider = Application.Security as IRoleTypeProvider;
            if (roleTypeProvider != null) {
                RoleType = XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == roleTypeProvider.RoleType).Type;
                if (RoleType.IsInterface)
                    RoleType = XpoTypeInfoSource.GetGeneratedEntityType(RoleType);
            }
            if (Application.Security != null) {
                UserType = Application.Security.UserType;
                if (UserType.IsInterface)
                    UserType = XpoTypeInfoSource.GetGeneratedEntityType(UserType);
            }
        }

        IEnumerable<Type> CollectRequiredTypes(ITypeInfo typeInfo) {
            return (typeInfo.GetRequiredTypes(info => IsExportedType(info.Type)).Select(required => required.Type));
        }

        bool IsLoadingExternalModel() {
            return TypesInfo != XafTypesInfo.Instance;
        }

        public Assembly BaseImplAssembly {
            get {
                if (_baseImplAssembly == null)
                    LoadBaseImplAssembly();
                return _baseImplAssembly;
            }
        }

        public static IEnumerable<Type> CollectExportedTypesFromAssembly(Assembly assembly) {
            var typesList = new ExportedTypeCollection();
            try {
                TypesInfo.LoadTypes(assembly);
                if (assembly == typeof(XPObject).Assembly) {
                    typesList.AddRange(XpoTypeInfoSource.XpoBaseClasses);
                } else {
                    typesList.AddRange(assembly.GetTypes());
                }
            } catch (Exception e) {
                throw new InvalidOperationException(
                    String.Format("Exception occurs while ensure classes from assembly {0}\r\n{1}", assembly.FullName, e.Message), e);
            }
            return typesList;
        }

        public Type LoadFromBaseImpl(string typeName) {
            if (BaseImplAssembly != null) {
                var typeInfo = TypesInfo.FindTypeInfo(typeName);
                return typeInfo != null ? typeInfo.Type : null;
            }
            return null;
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

        public static XpoTypeInfoSource XpoTypeInfoSource {
            get { return XpoTypesInfoHelper.GetXpoTypeInfoSource(); }
        }

        protected void AddToAdditionalExportedTypes(string[] types) {
            if (RuntimeMode) {
                var types1 = BaseImplAssembly.GetTypes().Where(type1 => types.Contains(type1.FullName));
                AdditionalExportedTypes.AddRange(types1);
            }

        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName) {
            if (RuntimeMode) {
                var types = BaseImplAssembly.GetTypes().Where(type1 => (type1.Namespace + "").StartsWith(nameSpaceName));
                AdditionalExportedTypes.AddRange(types);
            }
        }

        protected void CreateDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName) {
            XPClassInfo info = Dictiorary.GetClassInfo(classType);
            if (info.FindMember(propertyName) == null) {
                info.CreateMember(propertyName, typeof(XPCollection), true);
                info.AddAttribute(new VisibleInDetailViewAttribute(false));
                typesInfo.RefreshInfo(classType);
            }
        }

        public IList<Type> GetAdditionalClasses(ApplicationModulesManager manager) {
            return GetAdditionalClasses(manager.Modules);
        }
        public IList<Type> GetAdditionalClasses(ModuleList moduleList) {
            return new List<Type>(moduleList.SelectMany(@base => @base.AdditionalExportedTypes));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            OnApplicationInitialized(Application);
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (!ApplicationType().IsInstanceOfType(application))
                throw new TypeInitializationException(application.GetType().FullName, new Exception("Please derive your " + application.GetType().FullName + " from either XpandWinApplication or XpandWebApplication"));
            if (ManifestModuleName == null)
                ManifestModuleName = application.GetType().Assembly.ManifestModule.Name;
            OnApplicationInitialized(application);
            application.SetupComplete += ApplicationOnSetupComplete;
        }

        protected Type DefaultXafAppType = typeof(XafApplication);
        protected virtual Type ApplicationType() {
            return DefaultXafAppType;
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            OnApplicationInitialized(Application);
            var type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelMember)).FindMember("Type");
            var attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);

            type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelBOModelClassMembers));
            attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            DisposeManagers();
        }

        public static void ReStoreManagers() {
            _instanceModelApplicationCreatorManager.Value = (ModelApplicationCreator)_storeManagers[0];
        }


        public static void DisposeManagers() {
            _storeManagers = new List<object>();
            if (_instanceModelApplicationCreatorManager != null) {
                _storeManagers.Add(_instanceModelApplicationCreatorManager.Value);
                _instanceModelApplicationCreatorManager.Value = null;
            }
        }

        protected virtual void OnApplicationInitialized(XafApplication xafApplication) {
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            AssignSecurityEntities();
            lock (_lockObject) {
                if (_instanceModelApplicationCreatorManager == null)
                    _instanceModelApplicationCreatorManager = ValueManager.GetValueManager<ModelApplicationCreator>("instanceModelApplicationCreatorManager");
                if (_instanceModelApplicationCreatorManager.Value == null)
                    _instanceModelApplicationCreatorManager.Value = ((ModelApplicationBase)Application.Model).CreatorInstance;
            }
        }
    }
}
