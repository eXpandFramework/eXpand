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
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Model.RuntimeMembers;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp {
    [ToolboxItem(false)]
    public abstract class XpandModuleBase : ModuleBase {
        static List<object> _storeManagers;
        readonly static HashSet<string> _callMonitor=new HashSet<string>();
        public static string ManifestModuleName;
        static readonly object _lockObject = new object();
        static IValueManager<ModelApplicationCreator> _instanceModelApplicationCreatorManager;
        public static object Control;
        static Assembly _baseImplAssembly;
        static XPDictionary _dictiorary;
        static string _connectionString;
        protected Type DefaultXafAppType = typeof (XafApplication);
        static bool _compatibilityChecked;

        static XpandModuleBase() {
            TypesInfo = XafTypesInfo.Instance;
        }

        public static HashSet<string> CallMonitor {
            get { return _callMonitor; }
        }

        public static XPDictionary Dictiorary {
            get {
                if (!InterfaceBuilder.RuntimeMode && _dictiorary == null)
                    _dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
                return _dictiorary;
            }
            set { _dictiorary = value; }
        }

        public override void CustomizeLogics(CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            if (Executed("CustomizeLogics"))
                return;
            customLogics.RegisterLogic(typeof(ITypesInfoProvider), typeof(TypesInfoProviderDomainLogic));
        }

        public bool Executed(string name) {
            if (_callMonitor.Contains(name))
                return true;
            _callMonitor.Add(name);
            return false;
        }

        public static ITypesInfo TypesInfo { get; set; }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            if (Executed("ExtendModelInterfaces"))
                return;
            extenders.Add<IModelColumn, IModelColumnDetailViews>();
            extenders.Add<IModelMember, IModelMemberDataStoreForeignKeyCreated>();
            extenders.Add<IModelApplication, ITypesInfoProvider>();
        }
        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            if (Executed("AddGeneratorUpdaters"))
                return;
            updaters.Add(new ModelViewClonerUpdater());
        }

        protected bool RuntimeMode {
            get { return InterfaceBuilder.RuntimeMode; }
        }

        public Assembly BaseImplAssembly {
            get {
                if (_baseImplAssembly == null)
                    LoadBaseImplAssembly();
                return _baseImplAssembly;
            }
        }

        public static ModelApplicationCreator ModelApplicationCreator {
            get {
                return _instanceModelApplicationCreatorManager != null
                           ? _instanceModelApplicationCreatorManager.Value
                           : null;
            }
            set {
                if (_instanceModelApplicationCreatorManager != null)
                    _instanceModelApplicationCreatorManager.Value = value;
            }
        }

        public static XpoTypeInfoSource XpoTypeInfoSource {
            get { return XpoTypesInfoHelper.GetXpoTypeInfoSource(); }
        }

        public static string ConnectionString {
            get {
                if (_connectionString != null) return _connectionString;
                throw new NullReferenceException("ConnectionString");
            }
            protected set { _connectionString = value; }
        }

        void LoadBaseImplAssembly() {
            string assemblyString = string.Format("Xpand.Persistent.BaseImpl, Version={0}, Culture=neutral, PublicKeyToken={1}", XpandAssemblyInfo .FileVersion, XpandAssemblyInfo.Token);
            string baseImplName = ConfigurationManager.AppSettings["Baseimpl"];
            if (!String.IsNullOrEmpty(baseImplName)) {
                assemblyString = baseImplName;
            }
            try {
                _baseImplAssembly = Assembly.Load(assemblyString);
            }
            catch (FileNotFoundException) {
            }
            if (_baseImplAssembly == null)
                throw new NullReferenceException(
                    "BaseImpl not found please reference it in your front end project and set its Copy Local=true");
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            if (IsLoadingExternalModel()) {
                var declaredExportedTypes = new List<Type>();
                IEnumerable<Type> collectExportedTypesFromAssembly =
                    CollectExportedTypesFromAssembly(GetType().Assembly).Where(IsExportedType);
                foreach (Type type in collectExportedTypesFromAssembly) {
                    ITypeInfo typeInfo = TypesInfo.FindTypeInfo(type);
                    declaredExportedTypes.Add(type);
                    foreach (Type type1 in CollectRequiredTypes(typeInfo)) {
                        if (!declaredExportedTypes.Contains(type1))
                            declaredExportedTypes.Add(type1);
                    }
                }
                return declaredExportedTypes;
            }
            IEnumerable<Type> exportedTypes = base.GetDeclaredExportedTypes();
            return !exportedTypes.Any() ? AdditionalExportedTypes : exportedTypes;
        }

        void AssignSecurityEntities() {
            if (Application != null) {
                var roleTypeProvider = Application.Security as IRoleTypeProvider;
                if (roleTypeProvider != null) {
                    RoleType =
                        XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == roleTypeProvider.RoleType)
                                    .Type;
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

        public static IEnumerable<Type> CollectExportedTypesFromAssembly(Assembly assembly) {
            var typesList = new ExportedTypeCollection();
            try {
                TypesInfo.LoadTypes(assembly);
                if (Equals(assembly, typeof (XPObject).Assembly)) {
                    typesList.AddRange(XpoTypeInfoSource.XpoBaseClasses);
                }
                else {
                    typesList.AddRange(assembly.GetTypes());
                }
            }
            catch (Exception e) {
                throw new InvalidOperationException(
                    String.Format("Exception occurs while ensure classes from assembly {0}\r\n{1}", assembly.FullName,
                                  e.Message), e);
            }
            return typesList;
        }

        public Type LoadFromBaseImpl(string typeName) {
            if (BaseImplAssembly != null) {
                ITypeInfo typeInfo = TypesInfo.FindTypeInfo(typeName);
                return typeInfo != null ? typeInfo.Type : null;
            }
            return null;
        }

        protected void AddToAdditionalExportedTypes(string[] types) {
            if (RuntimeMode) {
                IEnumerable<Type> types1 = BaseImplAssembly.GetTypes().Where(type1 => types.Contains(type1.FullName));
                AdditionalExportedTypes.AddRange(types1);
            }
        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName, Assembly assembly) {
            if (RuntimeMode) {
                IEnumerable<Type> types =
                    assembly.GetTypes()
                            .Where(type1 => string.Join("", new[]{type1.Namespace}).StartsWith(nameSpaceName));
                AdditionalExportedTypes.AddRange(types);
            }
        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName) {
            AddToAdditionalExportedTypes(nameSpaceName, BaseImplAssembly);
        }

        protected void CreateDesignTimeCollection(ITypesInfo typesInfo, Type classType, string propertyName) {
            XPClassInfo info = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.GetClassInfo(classType);
            if (info.FindMember(propertyName) == null) {
                info.CreateMember(propertyName, typeof (XPCollection), true);
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
            if (Executed("Setup2"))
                return;
            SetConnectionString();
            OnApplicationInitialized(Application);
        }

        void SetConnectionString() {
            if (!RuntimeMode)
                return;
            if (Application.ObjectSpaceProviders.Count == 0) {
                ConnectionString = Application.ConnectionString;
            }
            else {
                var provider = Application.ObjectSpaceProvider as IXpandObjectSpaceProvider;
                if (provider != null) {
                    ConnectionString =provider.DataStoreProvider.ConnectionString;
                }
                else if (Application.ObjectSpaceProvider is XPObjectSpaceProvider) {
                    var fieldInfo = typeof(XPObjectSpaceProvider).GetField("dataStoreProvider", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo == null) throw new NullReferenceException("dataStoreProvider fieldInfo");
                    var xpoDataStoreProvider = ((IXpoDataStoreProvider) fieldInfo.GetValue(Application.ObjectSpaceProvider));
                    ConnectionString = xpoDataStoreProvider.ConnectionString;
                }
                else {
                    throw new NotImplementedException();
                }
            }
        }


        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode)
                ApplicationHelper.Instance.Initialize(application);
            CheckApplicationTypes();
            OnApplicationInitialized(application);
            if (Executed("Setup"))
                return;
            Dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
            
            if (ManifestModuleName == null)
                ManifestModuleName = application.GetType().Assembly.ManifestModule.Name;
            
            application.SetupComplete += ApplicationOnSetupComplete;
            application.SettingUp += ApplicationOnSettingUp;
            application.CustomCheckCompatibility+=ApplicationOnCustomCheckCompatibility;
        }

        void ApplicationOnCustomCheckCompatibility(object sender, CustomCheckCompatibilityEventArgs customCheckCompatibilityEventArgs) {
            ((XafApplication) sender).CustomCheckCompatibility-=ApplicationOnCustomCheckCompatibility;
            _compatibilityChecked = true;
        }

        public static bool CompatibilityChecked {
            get { return _compatibilityChecked; }
        }


        void CheckApplicationTypes() {
            if (RuntimeMode) {
                foreach (var applicationType in ApplicationTypes()) {
                    if (!applicationType.IsInstanceOfType(Application)) {
                        throw new CannotLoadInvalidTypeException(Application.GetType().FullName + " from " + GetType().FullName + ". "+Environment.NewLine + Application.GetType().FullName + " must implement/derive from " +
                                                                 applicationType.FullName + Environment.NewLine +
                                                                 "Please check folder Demos/Modules/" +
                                                                 GetType().Name.Replace("Module", null) + " to see how to install correctly this module. As an quick workaround you can derive your " +Application.GetType().FullName+ " from XpandWinApplication or from XpandWebApplication");
                    }
                }
            }
        }

        void ApplicationOnSettingUp(object sender, SetupEventArgs setupEventArgs) {
            AssignSecurityEntities();
        }

        protected virtual Type[] ApplicationTypes() {
            return new[] { DefaultXafAppType };
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Executed("CustomizeTypesInfo"))
                return;
            AssignSecurityEntities();
            OnApplicationInitialized(Application);
            ITypeInfo findTypeInfo = typesInfo.FindTypeInfo(typeof (IModelMember));
            var type = (BaseInfo) findTypeInfo.FindMember("Type");

            var attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);

            type = (BaseInfo) typesInfo.FindTypeInfo(typeof (IModelBOModelClassMembers));
            attribute = type.FindAttribute<ModelReadOnlyAttribute>();
            if (attribute != null)
                type.RemoveAttribute(attribute);
        }

        void ModifySequenceObjectWhenMySqlDatalayer(ITypesInfo typesInfo) {
            ITypeInfo typeInfo = typesInfo.FindTypeInfo(typeof (ISequenceObject));
            IEnumerable<ITypeInfo> descendants = typeInfo.Implementors.Where(IsMySql);
            foreach (ITypeInfo descendant in descendants) {
                var memberInfo = (XafMemberInfo) descendant.KeyMember;
                if (memberInfo != null) {
                    memberInfo.RemoveAttributes<SizeAttribute>();
                    memberInfo.AddAttribute(new SizeAttribute(255));
                }
            }
        }

        bool IsMySql(ITypeInfo typeInfo) {
            var sequenceObjectObjectSpaceProvider = GetSequenceObjectObjectSpaceProvider(typeInfo.Type);
            if (sequenceObjectObjectSpaceProvider != null) {
                var helper = new ConnectionStringParser(sequenceObjectObjectSpaceProvider.ConnectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                return providerType == MySqlConnectionProvider.XpoProviderTypeString;
            }
            return false;
        }

        IObjectSpaceProvider GetSequenceObjectObjectSpaceProvider(Type type) {
            return (from objectSpaceProvider in Application.ObjectSpaceProviders let originalObjectType = objectSpaceProvider.EntityStore.GetOriginalType(type) where (originalObjectType != null) && objectSpaceProvider.EntityStore.RegisteredEntities.Contains(originalObjectType) select objectSpaceProvider).FirstOrDefault();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            DisposeManagers();
        }

        public static void ReStoreManagers() {
            _instanceModelApplicationCreatorManager.Value = (ModelApplicationCreator) _storeManagers[0];
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
                    _instanceModelApplicationCreatorManager =
                        ValueManager.GetValueManager<ModelApplicationCreator>("instanceModelApplicationCreatorManager");
                if (_instanceModelApplicationCreatorManager.Value == null)
                    _instanceModelApplicationCreatorManager.Value =
                        ((ModelApplicationBase) Application.Model).CreatorInstance;
            }
        }
    }
}