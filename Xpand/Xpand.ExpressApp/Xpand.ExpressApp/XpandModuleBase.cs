using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
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
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp {

    [ToolboxItem(false)]
    public abstract class XpandModuleBase : ModuleBase {
        static List<object> _storeManagers;
        public static XPDictionary Dictiorary {
            get {
                if (!InterfaceBuilder.RuntimeMode && _dictiorary == null)
                    _dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
                return _dictiorary;
            }
            set { _dictiorary = value; }
        }

        public static ITypesInfo TypesInfo { get; set; }
        public static string ManifestModuleName;
        static readonly object _lockObject = new object();
        static IValueManager<ModelApplicationCreator> _instanceModelApplicationCreatorManager;
        public static object Control;
        static Assembly _baseImplAssembly;
        static XPDictionary _dictiorary;

        static XpandModuleBase() {
            TypesInfo = XafTypesInfo.Instance;
        }

        void LoadBaseImplAssembly() {
            var assemblyString = "Xpand.Persistent.BaseImpl, Version=*, Culture=neutral, PublicKeyToken=*";
            var baseImplName = ConfigurationManager.AppSettings["Baseimpl"];
            if (!String.IsNullOrEmpty(baseImplName)) {
                assemblyString = baseImplName;
            }
            try {
                _baseImplAssembly = Assembly.Load(assemblyString);
            } catch (FileNotFoundException) {
            }
            if (_baseImplAssembly == null)
                throw new NullReferenceException(
                    "BaseImpl not found please reference it in your front end project and set its Copy Local=true");
        }

        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        protected bool RuntimeMode {
            get {
                return InterfaceBuilder.RuntimeMode;
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
            var exportedTypes = base.GetDeclaredExportedTypes();
            return !exportedTypes.Any() ? AdditionalExportedTypes : exportedTypes;
        }

        void AssignSecurityEntities() {
            if (Application != null) {
                var roleTypeProvider = Application.Security as IRoleTypeProvider;
                if (roleTypeProvider != null) {
                    RoleType = XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == roleTypeProvider.RoleType).Type;
                    if (RoleType.IsInterface)
                        RoleType = XpoTypeInfoSource.GetGeneratedEntityType(RoleType);
                }
                if (Application.Security != null) {
                    UserType = Application.Security.UserType;
                    if (UserType != null && UserType.IsInterface)
                        UserType = XpoTypeInfoSource.GetGeneratedEntityType(UserType);
                }
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
                if (Equals(assembly, typeof(XPObject).Assembly)) {
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

        protected void AddToAdditionalExportedTypes(string nameSpaceName, Assembly assembly) {
            if (RuntimeMode) {
                var types = assembly.GetTypes().Where(type1 => string.Join("", new[] { type1.Namespace }).StartsWith(nameSpaceName));
                AdditionalExportedTypes.AddRange(types);
            }
        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName) {
            AddToAdditionalExportedTypes(nameSpaceName, BaseImplAssembly);
        }

        protected void CreateDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName) {
            XPClassInfo info = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.GetClassInfo(classType);
            if (info.FindMember(propertyName) == null) {
                info.CreateMember(propertyName, typeof(XPCollection), true);
                typesInfo.RefreshInfo(classType);
            }
        }

        public IList<Type> GetAdditionalClasses(ApplicationModulesManager manager) {
            return GetAdditionalClasses(manager.Modules);
        }
        public IList<Type> GetAdditionalClasses(ModuleList moduleList) {
            return new List<Type>(moduleList.SelectMany(@base => @base.AdditionalExportedTypes));
        }

        static bool _setupCalled;
        static bool _setup2Called;
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (_setupCalled)
                return;
            OnApplicationInitialized(Application);
            _setupCalled = true;
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (_setup2Called)
                return;
            ApplicationHelper.Instance.Initialize(application);
            Dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
            Type applicationType = ApplicationType();
            if (!applicationType.IsInstanceOfType(application)) {
                throw new CannotLoadInvalidTypeException(application.GetType().FullName + " must implement/derive from " + applicationType.FullName + Environment.NewLine + "Please check folder Demos/Modules/" + GetType().Name.Replace("Module", null) + " to see how to install correctly this module");
                
            }
            if (ManifestModuleName == null)
                ManifestModuleName = application.GetType().Assembly.ManifestModule.Name;
            OnApplicationInitialized(application);
            application.SetupComplete += ApplicationOnSetupComplete;
            application.SettingUp += ApplicationOnSettingUp;
            application.CreateCustomObjectSpaceProvider+=ApplicationOnCreateCustomObjectSpaceProvider;
            _setup2Called = true;
        }

        void ApplicationOnCreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs createCustomObjectSpaceProviderEventArgs) {
            _connectionString = createCustomObjectSpaceProviderEventArgs.ConnectionString;
        }

        void ApplicationOnSettingUp(object sender, SetupEventArgs setupEventArgs) {
            AssignSecurityEntities();
        }

        protected Type DefaultXafAppType = typeof(XafApplication);
        static string _connectionString;

        public static string ConnectionString {
            get { return _connectionString; }
        }

        protected virtual Type ApplicationType() {
            return DefaultXafAppType;
        }

        static bool _customizeTypesInfoCalled ;
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (_customizeTypesInfoCalled)
                return;
            

            AssignSecurityEntities();
            OnApplicationInitialized(Application);
            var findTypeInfo = typesInfo.FindTypeInfo(typeof(IModelMember));
            var type = (BaseInfo)findTypeInfo.FindMember("Type");

            var attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);

            type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelBOModelClassMembers));
            attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);
            _customizeTypesInfoCalled = true;
        }

        void ModifySequenceObjectWhenMySqlDatalayer(ITypesInfo typesInfo) {
            var typeInfo = typesInfo.FindTypeInfo(typeof (ISequenceObject));
            var descendants = typeInfo.Implementors.Where(IsMySql);
            foreach (var descendant in descendants) {
                var memberInfo = (XafMemberInfo) descendant.KeyMember;
                if (memberInfo != null) {
                    memberInfo.RemoveAttributes<SizeAttribute>();
                    memberInfo.AddAttribute(new SizeAttribute(255));
                }
            }
        }

        bool IsMySql(ITypeInfo typeInfo) {
            var objectSpace = Application.CreateObjectSpace(typeInfo.Type) as XpandObjectSpace;
            return objectSpace != null && ((BaseDataLayer)objectSpace.Session.DataLayer).ConnectionProvider is MySqlConnectionProvider;
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
            lock (_lockObject) {
                ModifySequenceObjectWhenMySqlDatalayer(TypesInfo);
                if (_instanceModelApplicationCreatorManager == null)
                    _instanceModelApplicationCreatorManager = ValueManager.GetValueManager<ModelApplicationCreator>("instanceModelApplicationCreatorManager");
                if (_instanceModelApplicationCreatorManager.Value == null)
                    _instanceModelApplicationCreatorManager.Value = ((ModelApplicationBase)Application.Model).CreatorInstance;
            }
        }
    }
}
