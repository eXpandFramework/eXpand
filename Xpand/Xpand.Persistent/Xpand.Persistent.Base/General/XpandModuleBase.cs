using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using Microsoft.Win32;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Utils.GeneralDataStructures;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public interface IXpandModuleBase {
        event EventHandler<GeneratorUpdaterEventArgs> CustomAddGeneratorUpdaters;
        event EventHandler ApplicationModulesManagerSetup;
        ModuleTypeList RequiredModuleTypes { get; }
        XafApplication Application { get; }
    }

    [ToolboxItem(false)]
    public class XpandModuleBase : ModuleBase, IModelNodeUpdater<IModelMemberEx>, IModelXmlConverter, IXpandModuleBase {
        private static string _xpandPathInRegistry;
        private static string _dxPathInRegistry;
        public static string ManifestModuleName;
        static readonly object _lockObject = new object();
        public static object Control;
        static Assembly _baseImplAssembly;
        static string _connectionString;
        private static readonly object _syncRoot = new object();
        protected Type DefaultXafAppType = typeof (XafApplication);
        static  bool? _isHosted;
        static string _assemblyString;
        private static volatile IValueManager<MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>> _callMonitor;
        
        public event EventHandler ApplicationModulesManagerSetup;

        protected virtual void OnApplicationModulesManagerSetup(EventArgs e) {
            EventHandler handler = ApplicationModulesManagerSetup;
            if (handler != null) handler(null, e);
        }

        public event CancelEventHandler InitSeqGenerator;
        public event EventHandler<ExtendingModelInterfacesArgs> ExtendingModelInterfaces ;
        public event EventHandler<GeneratorUpdaterEventArgs> CustomAddGeneratorUpdaters ;

        void OnCustomAddGeneratorUpdaters(GeneratorUpdaterEventArgs e) {
            EventHandler<GeneratorUpdaterEventArgs> handler = CustomAddGeneratorUpdaters;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnExtendingModelInterfaces(ExtendingModelInterfacesArgs e) {
            EventHandler<ExtendingModelInterfacesArgs> handler = ExtendingModelInterfaces;
            if (handler != null) handler(this, e);
        }

        public static MultiValueDictionary<KeyValuePair<string,ApplicationModulesManager>, object> CallMonitor {
            get {
                if (_callMonitor == null) {
                    lock (_syncRoot) {
                        if (_callMonitor == null) {
                            _callMonitor = ValueManager.GetValueManager<MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>>("CallMonitor");
                        }
                    }
                }
                if (_callMonitor.CanManageValue) {
                    if (_callMonitor.Value == null) {
                        lock (_syncRoot) {
                            if (_callMonitor.Value == null) {
                                _callMonitor.Value =new MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>();
                            }
                        }
                    }
                    return _callMonitor.Value;
                }
                return new MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>();
            }
        }

        public static bool IsHosted {
            get {
                if (!_isHosted.HasValue) {
                    var xafApplication = ApplicationHelper.Instance.Application;
                    _isHosted = GetIsHosted(xafApplication.Model);
                }
                return _isHosted.Value;
            }
        }

        public static bool GetIsHosted(IModelApplication application) {
            var modelSources = ((IModelSources) application);
            if (modelSources == null)
                return Assembly.GetEntryAssembly() == null;
            return modelSources.Modules.Any(@base => {
                var attribute =((ITypesInfoProvider) application).TypesInfo.FindTypeInfo(@base.GetType()).FindAttribute<ToolboxItemFilterAttribute>();
                if (attribute != null)
                    return attribute.FilterString == "Xaf.Platform.Web";
                return false;
            });
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes() {
            var declaredControllerTypes = base.GetDeclaredControllerTypes();
            if (!Executed<IDashboardUser>("DashboardUser")) {
                declaredControllerTypes =declaredControllerTypes.Concat(new[]
                    {typeof (DashboardInteractionController), typeof (WebDashboardRefreshController)});
            }
            if (!Executed("GetDeclaredControllerTypes")) {
                declaredControllerTypes= declaredControllerTypes.Union(new[]
                {typeof (CreatableItemController), typeof (FilterByColumnController),typeof(CreateExpandAbleMembersViewController)});
            }
            
            return declaredControllerTypes;
        }

        internal void OnInitSeqGenerator(CancelEventArgs e) {
            CancelEventHandler handler = InitSeqGenerator;
            if (handler != null) handler(this, e);
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator) {
            base.AddModelNodeUpdaters(updaterRegistrator);
            updaterRegistrator.AddUpdater(this);
        }

        public static XPDictionary Dictiorary {
            get {
                return XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary ;
            }
        }

        public override void CustomizeLogics(CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            if (Executed("CustomizeLogics"))
                return;
            customLogics.RegisterLogic(typeof(IModelClassEx), typeof(ModelClassExDomainLogic));
            customLogics.RegisterLogic(typeof(ITypesInfoProvider), typeof(TypesInfoProviderDomainLogic));
            customLogics.RegisterLogic(typeof(IModelColumnDetailViews), typeof(ModelColumnDetailViewsDomainLogic));
            customLogics.RegisterLogic(typeof(IModelApplicationListViews), typeof(ModelApplicationListViewsDomainLogic));
        }

        public bool Executed<T>(string name) {
            if (typeof(T).IsAssignableFrom(GetType())) {
                Type value = typeof (T);
                var keyValuePair = new KeyValuePair<string, ApplicationModulesManager>(name, ModuleManager);
                if (CallMonitor.ContainsKey(keyValuePair)) {
                    if (!CallMonitor.GetValues(keyValuePair, true).Contains(value)) {
                        CallMonitor.Add(keyValuePair, value);
                        return false;
                    }
                    return true;
                }
                CallMonitor.Add(keyValuePair, value);
                
                return false;
            }
            return true;
        }

        public bool Executed(string name) {
            return Executed<object>(name);
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            OnExtendingModelInterfaces(new ExtendingModelInterfacesArgs(extenders));
            if (!Executed<IColumnCellFilterUser>("ExtendModelInterfaces")) {
                extenders.Add<IModelMember, IModelMemberCellFilter>();
                extenders.Add<IModelColumn, IModelColumnCellFilter>();   
            }

            if (Executed("ExtendModelInterfaces"))
                return;

            extenders.Add<IModelOptions, IModelOptionsMergedDifferenceStrategy>();
            extenders.Add<IModelClass, IModelClassEx>();
            extenders.Add<IModelColumn, IModelColumnDetailViews>();
            extenders.Add<IModelMember, IModelMemberDataStoreForeignKeyCreated>();
            extenders.Add<IModelApplication, ITypesInfoProvider>();
            extenders.Add<IModelApplication, IModelApplicationModule>();
            extenders.Add<IModelApplication, IModelApplicationReadonlyParameters>();
            extenders.Add<IModelApplication, IModelApplicationListViews>();
            extenders.Add<IModelObjectView, IModelObjectViewMergedDifferences>();
            
        }
        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            OnCustomAddGeneratorUpdaters(new GeneratorUpdaterEventArgs(updaters));
            if (Executed("AddGeneratorUpdaters"))
                return;
            
            updaters.Add(new ModelViewClonerUpdater());
            updaters.Add(new MergedDifferencesUpdater());
        }

        protected internal bool RuntimeMode {
            get { return InterfaceBuilder.RuntimeMode; }
        }

        public Assembly BaseImplAssembly {
            get {
                if (_baseImplAssembly == null)
                    LoadBaseImplAssembly();
                return _baseImplAssembly;
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
            internal set { _connectionString = value; }
        }

        [SecuritySafeCritical]
        void LoadBaseImplAssembly() {
            _assemblyString = String.Format("Xpand.Persistent.BaseImpl, Version={0}, Culture=neutral, PublicKeyToken={1}", XpandAssemblyInfo.FileVersion, XpandAssemblyInfo.Token);
            string baseImplName = ConfigurationManager.AppSettings["Baseimpl"];
            if (!String.IsNullOrEmpty(baseImplName)) {
                _assemblyString = baseImplName;
            }
            try {
                AppDomain.CurrentDomain.AssemblyResolve += XpandAssemblyResolve;
                _baseImplAssembly = Assembly.Load(_assemblyString);
            }
            catch (FileNotFoundException) {
            }
            finally {
                AppDomain.CurrentDomain.AssemblyResolve -= XpandAssemblyResolve;
            }
            if (_baseImplAssembly == null)
                throw new FileNotFoundException(
                    "Xpand.ExpressApp.BaseImpl not found please reference it in your front end project and set its Copy Local=true");
        }

        public static string XpandPathInRegistry {
            get {
                if (_xpandPathInRegistry==null) {
                    _xpandPathInRegistry = "";
                    var softwareNode = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node") ??
                                       Registry.LocalMachine.OpenSubKey("Software");
                    if (softwareNode != null) {
                        var xpandNode = softwareNode.OpenSubKey(@"Microsoft\.NetFramework\AssemblyFolders\Xpand");
                        if (xpandNode != null)
                            _xpandPathInRegistry = xpandNode.GetValue(null) + "";
                    }
                }
                return _xpandPathInRegistry;
            }
        }        

        public static string DXPathInRegistry {
            get {
                if (_dxPathInRegistry == null) {
                    _dxPathInRegistry = "";
                    var softwareNode = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node") ??
                                       Registry.LocalMachine.OpenSubKey("Software");
                    if (softwareNode != null) {
                        var xpandNode = softwareNode.OpenSubKey(@"DevExpress\DXperience\v"+AssemblyInfo.VersionShort);
                        if (xpandNode != null)
                            _dxPathInRegistry = xpandNode.GetValue("RootDirectory") + "";
                    }
                }
                return _dxPathInRegistry;
            }
        }

        public static Assembly XpandAssemblyResolve(object sender, ResolveEventArgs args) {
            if (!string.IsNullOrEmpty(XpandPathInRegistry)) {
                var path = Path.Combine(XpandPathInRegistry,args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) +".dll");
                if (File.Exists(path))
                    return Assembly.LoadFile(path);
            }
            return null;
        }

        public static Assembly DXAssemblyResolve(object sender, ResolveEventArgs args) {
            if (!string.IsNullOrEmpty(DXPathInRegistry)) {
                var path = Path.Combine(DXPathInRegistry,@"bin\framework\"+ args.Name + ".dll");
                if (File.Exists(path))
                    return Assembly.LoadFile(path);
            }
            return null;
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            if (IsLoadingExternalModel()) {
                var declaredExportedTypes = new List<Type>();
                IEnumerable<Type> collectExportedTypesFromAssembly =
                    CollectExportedTypesFromAssembly(GetType().Assembly).Where(IsExportedType);
                foreach (Type type in collectExportedTypesFromAssembly) {
                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
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

        public static bool IsLoadingExternalModel() {
            return XafTypesInfo.Instance.GetType()!=typeof(TypesInfo);
        }

        public static IEnumerable<Type> CollectExportedTypesFromAssembly(Assembly assembly) {
            var typesList = new ExportedTypeCollection();
            try {
                XafTypesInfo.Instance.LoadTypes(assembly);
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
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeName);
                return typeInfo != null ? typeInfo.Type : null;
            }
            return null;
        }

        protected internal void AddToAdditionalExportedTypes(string[] types) {
            if (RuntimeMode) {
                IEnumerable<Type> types1 = BaseImplAssembly.GetTypes().Where(type1 => types.Contains(type1.FullName));
                AdditionalExportedTypes.AddRange(types1);
            }
        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName, Assembly assembly) {
            if (RuntimeMode) {
                IEnumerable<Type> types =
                    assembly.GetTypes()
                            .Where(type1 => String.Join("", new[]{type1.Namespace}).StartsWith(nameSpaceName));
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
            OnApplicationModulesManagerSetup(EventArgs.Empty);
            if (Executed("Setup2"))
                return;
            if (RuntimeMode)
                ConnectionString = this.GetConnectionString();
            XafTypesInfo.Instance.LoadTypes(typeof(XpandModuleBase).Assembly);
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode)
                ApplicationHelper.Instance.Initialize(application);
            CheckApplicationTypes();
            var helper = new ConnectionStringHelper();
            helper.Attach(this);
            var generatorHelper = new SequenceGeneratorHelper();
            generatorHelper.Attach(this,helper);

            if (Executed("Setup"))
                return;

            if (ManifestModuleName == null)
                ManifestModuleName = application.GetType().Assembly.ManifestModule.Name;
            
            application.SetupComplete += ApplicationOnSetupComplete;
            application.SettingUp += ApplicationOnSettingUp;
            application.ObjectSpaceCreated+=ApplicationOnObjectSpaceCreated;
        }

        void ApplicationOnObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs objectSpaceCreatedEventArgs) {
            var xpObjectSpace = objectSpaceCreatedEventArgs.ObjectSpace as XPObjectSpace;
            if (xpObjectSpace!=null) {
                xpObjectSpace.SetFieldValue("objectsToSave", new HashedArrayList());
                
            }
        }

        public static bool ObjectSpaceCreated { get; internal set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type SequenceObjectType { get; set; }

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
            foreach (var memberInfo in typesInfo.PersistentTypes.SelectMany(info => info.Members).Where(info => info.FindAttribute<InvisibleInAllViewsAttribute>() != null)) {
                memberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
                memberInfo.AddAttribute(new VisibleInListViewAttribute(false));
                memberInfo.AddAttribute(new VisibleInLookupListViewAttribute(false));
            }

            AssignSecurityEntities();
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
            if (!RuntimeMode) {
                var keyValuePairs = CallMonitor.Keys.ToList();
                foreach (var pair in keyValuePairs) {
                    CallMonitor[pair].Clear();
                }
            }
            base.Dispose(disposing);
        }

        public void ConvertXml(ConvertXmlParameters parameters) {
            if (typeof(IModelMember).IsAssignableFrom(parameters.NodeType)) {
                if (parameters.Values.ContainsKey("IsRuntimeMember") && parameters.XmlNodeName == "Member" && parameters.Values["IsRuntimeMember"].ToLower() == "true")
                    parameters.NodeType = typeof(IModelMemberPersistent);
            }
            if (parameters.XmlNodeName == "CalculatedRuntimeMember") {
                parameters.NodeType = typeof(IModelMemberCalculated);
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            lock (_lockObject) {
                ModifySequenceObjectWhenMySqlDatalayer(XafTypesInfo.Instance);
            }
        }

        public void UpdateNode(IModelMemberEx node, IModelApplication application) {
            node.IsCustom = false;
        }

        public static void RemoveCall(string name, ApplicationModulesManager applicationModulesManager) {
            if (CallMonitor != null)
                CallMonitor.Remove(new KeyValuePair<string, ApplicationModulesManager>(name, applicationModulesManager));
        }
    }

    public class GeneratorUpdaterEventArgs : EventArgs {
        readonly ModelNodesGeneratorUpdaters _updaters;

        public GeneratorUpdaterEventArgs(ModelNodesGeneratorUpdaters updaters) {
            _updaters = updaters;
        }

        public ModelNodesGeneratorUpdaters Updaters {
            get { return _updaters; }
        }
    }

    public class ExtendingModelInterfacesArgs : EventArgs {
        readonly ModelInterfaceExtenders _extenders;

        public ExtendingModelInterfacesArgs(ModelInterfaceExtenders extenders) {
            _extenders = extenders;
        }

        public ModelInterfaceExtenders Extenders {
            get { return _extenders; }
        }
    }

    public static class ModuleBaseExtensions {
        public static string GetConnectionString(this ModuleBase moduleBase) {
            if (moduleBase.Application.ObjectSpaceProviders.Count == 0) {
                return moduleBase.Application.ConnectionString;
            }
            var provider = moduleBase.Application.ObjectSpaceProvider as IXpandObjectSpaceProvider;
            if (provider != null) {
                return provider.DataStoreProvider.ConnectionString;
            }
            if (moduleBase.Application.ObjectSpaceProvider is XPObjectSpaceProvider) {
                return ((IXpoDataStoreProvider)moduleBase.Application.ObjectSpaceProvider.GetFieldValue("dataStoreProvider")).ConnectionString;
            }
            return moduleBase.Application.ConnectionString;
        }
 
    }

    public class ConnectionStringHelper {
        const string ConnectionStringHelperName = "ConnectionStringHelper";
        static string _currentConnectionString;
        XpandModuleBase _xpandModuleBase;
        public event EventHandler ConnectionStringUpdated;

        void OnConnectionStringUpdated() {
            var handler = ConnectionStringUpdated;
            if (handler != null) handler(this, EventArgs.Empty);
        }
       
        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            XpandModuleBase.ObjectSpaceCreated = false;
            ((XafApplication)sender).LoggedOff -= ApplicationOnLoggedOff;
            if (!XpandModuleBase.IsHosted)
                Application.ObjectSpaceCreated += ApplicationOnObjectSpaceCreated;
            else
                XpandModuleBase.RemoveCall(ConnectionStringHelperName,_xpandModuleBase.ModuleManager);
        }

        void ApplicationOnObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs objectSpaceCreatedEventArgs) {
            XpandModuleBase.ObjectSpaceCreated = true;
            ((XafApplication)sender).ObjectSpaceCreated -= ApplicationOnObjectSpaceCreated;
            if (String.CompareOrdinal(_currentConnectionString, Application.ConnectionString) != 0) {
                _currentConnectionString = Application.ConnectionString;
                XpandModuleBase.ConnectionString=_xpandModuleBase.GetConnectionString();
                OnConnectionStringUpdated();
            }
        }

        protected XafApplication Application {
            get { return _xpandModuleBase.Application; }
        }

        protected bool RuntimeMode {
            get { return _xpandModuleBase.RuntimeMode; }
        }

        public void Attach(XpandModuleBase moduleBase) {
            _xpandModuleBase=moduleBase;
            if (RuntimeMode&&!Executed(ConnectionStringHelperName)) {
                Application.ObjectSpaceCreated += ApplicationOnObjectSpaceCreated;
                Application.LoggedOff += ApplicationOnLoggedOff;
                Application.DatabaseVersionMismatch+=ApplicationOnDatabaseVersionMismatch;
            }
        }

        void ApplicationOnDatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs databaseVersionMismatchEventArgs) {
            ((XafApplication) sender).DatabaseVersionMismatch-=ApplicationOnDatabaseVersionMismatch;
            ApplicationStatusUpdater.UpdateStatus+=ApplicationStatusUpdaterOnUpdateStatus;
        }

        void ApplicationStatusUpdaterOnUpdateStatus(object sender, UpdateStatusEventArgs updateStatusEventArgs) {
            if (updateStatusEventArgs.Context == ApplicationStatusMesssageId.UpdateDatabaseData.ToString()) {
                ApplicationStatusUpdater.UpdateStatus -= ApplicationStatusUpdaterOnUpdateStatus;
                ApplicationOnObjectSpaceCreated(Application, null);
            }
        }

        bool Executed(string name) {
            return _xpandModuleBase.Executed(name);
        }
    }
}