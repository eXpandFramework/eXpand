using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using Microsoft.Win32;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Controllers.Actions;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.CustomFunctions;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.MessageBox;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Persistent.Base.Xpo.MetaData;
using Xpand.Utils.GeneralDataStructures;
using Fasterflect;
using Mono.Cecil;
using Xpand.Extensions.AppDomainExtensions;
using Xpand.Extensions.Mono.Cecil;
using Xpand.Extensions.XAF.AppDomainExtensions;
using Xpand.Persistent.Base.General.Web;
using Xpand.Persistent.Base.General.Web.SyntaxHighlight;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;
using Xpand.Xpo.MetaData;
using PropertyEditorAttribute = DevExpress.ExpressApp.Editors.PropertyEditorAttribute;
using TypeInfo = DevExpress.ExpressApp.DC.TypeInfo;

namespace Xpand.Persistent.Base.General {
    public interface IXpandModuleBase {
        event EventHandler<GeneratorUpdaterEventArgs> CustomAddGeneratorUpdaters;
        event EventHandler<ApplicationModulesManagerSetupArgs> ApplicationModulesManagerSetup;
        ModuleTypeList RequiredModuleTypes { get; }
        XafApplication Application { get; }
    }


    public enum ModuleType {
        None,
        Agnostic,
        Win,
        Web,
        Mobile
    }
    [ToolboxItem(false)]
    public class XpandModuleBase : ModuleBase, IModelNodeUpdater<IModelMemberEx>, IModelXmlConverter, IXpandModuleBase {
        private static string _xpandPathInRegistry;
        private static string _dxPathInRegistry;
        public static string ManifestModuleName;
        static readonly object LockObject = new object();
        public static object Control;
        static Assembly _baseImplAssembly;
        static string _connectionString;
        private static readonly object SyncRoot = new object();
        protected Type DefaultXafAppType = typeof(XafApplication);
        static string _assemblyString;
        private static volatile IValueManager<MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>> _callMonitor;
        private static readonly HashSet<Type> DisabledControllerTypes = new HashSet<Type>();
        private static readonly object DisabledControllerTypesLock = new object();
        private ModuleType _moduleType;
        private List<KeyValuePair<string, ModelDifferenceStore>> _extraDiffStores;
        private bool _loggedOn;
        private static readonly TypesInfo AdditionalTypesTypesInfo;
        
        public event EventHandler<ApplicationModulesManagerSetupArgs> ApplicationModulesManagerSetup;

        static XpandModuleBase(){
            AppDomain.CurrentDomain.AddModelReference("netstandard");
            try {
                AdditionalTypesTypesInfo=new TypesInfo();
                if (Process.GetCurrentProcess().ProcessName!="devenv") {
                    LoadAssemblyRegularTypes();
                }
            }
            catch (TypeLoadException e) {
                throw new Exception("Possible fix as per issue #753",e);
            }
        }

        protected virtual void OnApplicationModulesManagerSetup(ApplicationModulesManagerSetupArgs e) {
            var handler = ApplicationModulesManagerSetup;
            handler?.Invoke(this, e);
        }

        public event CancelEventHandler InitSeqGenerator;
        public event EventHandler<ExtendingModelInterfacesArgs> ExtendingModelInterfaces;
        public event EventHandler<GeneratorUpdaterEventArgs> CustomAddGeneratorUpdaters;

        void OnCustomAddGeneratorUpdaters(GeneratorUpdaterEventArgs e) {
            EventHandler<GeneratorUpdaterEventArgs> handler = CustomAddGeneratorUpdaters;
            handler?.Invoke(this, e);
        }

        protected virtual void OnExtendingModelInterfaces(ExtendingModelInterfacesArgs e) {
            EventHandler<ExtendingModelInterfacesArgs> handler = ExtendingModelInterfaces;
            handler?.Invoke(this, e);
        }

        public XpandModuleBase() {
            AdditionalExportedTypes.Add(typeof(MessageBoxTextMessage));
        }

        public static MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object> CallMonitor {
            get {
                if (_callMonitor == null) {
                    lock (SyncRoot) {
                        _callMonitor ??=
                            ValueManager.GetValueManager<MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>>(
                                "CallMonitor");
                    }
                }
                if (_callMonitor.CanManageValue) {
                    if (_callMonitor.Value == null) {
                        lock (SyncRoot) {
                            _callMonitor.Value ??= new MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>();
                        }
                    }
                    return _callMonitor.Value;
                }
                return new MultiValueDictionary<KeyValuePair<string, ApplicationModulesManager>, object>();
            }
        }

        public static void DisableControllers(params Type[] types) {
            lock (DisabledControllerTypesLock) {
                foreach (Type type in types) {
                    if (!DisabledControllerTypes.Contains(type))
                        DisabledControllerTypes.Add(type);
                }
            }
        }

        protected IEnumerable<Type> FilterDisabledControllers(IEnumerable<Type> controllers) {
            if (controllers == null) return null;
            lock (DisabledControllerTypesLock) {
                return controllers.Where(t => !DisabledControllerTypes.Contains(t)).ToArray();
            }
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes() {
            var declaredControllerTypes = base.GetDeclaredControllerTypes();
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(declaredControllerTypes));
        }

        protected virtual IEnumerable<Type> GetDeclaredControllerTypesCore(IEnumerable<Type> declaredControllerTypes) {
            if (!Executed<IDashboardInteractionUser>("DashboardUser")) {
                declaredControllerTypes =
                    declaredControllerTypes.Concat(new[] {
                        typeof(DashboardInteractionController)
                    });
            }
            if (!Executed<IModifyModelActionUser>("ModifyModelActionControllerTypes")) {
                declaredControllerTypes = declaredControllerTypes.Concat(new[] {
                    typeof(ActionModifyModelController), 
                    typeof(ResetViewModelController),
                    typeof(ModelConfigurationController)
                });
            }
            if (!Executed("GetDeclaredControllerTypes")) {
                declaredControllerTypes = declaredControllerTypes.Union(new[]{
                    typeof (CreatableItemController), typeof (FilterByColumnController),
                    typeof (CreateExpandAbleMembersViewController), typeof (HideFromNewMenuViewController),
                    typeof (CustomAttibutesController), typeof (NotifyMembersController),
                    typeof (XpandModelMemberInfoController), typeof (XpandLinkToListViewController),
                    typeof(ModifyObjectSpaceController),typeof (ActionItemsFromModelController),typeof(ActionModelChoiceItemController),
                    typeof(ModelViewSavingController), typeof(NavigationContainerController), typeof(ModelController),
                    typeof(NavigationItemsController),typeof(FilteredMasterObjectViewController)
                });
            }

            if (ModuleType == ModuleType.Win)
                declaredControllerTypes = GetDeclaredWinControllerTypesCore(declaredControllerTypes);
            if (ModuleType == ModuleType.Web)
                declaredControllerTypes = GetDeclaredWebControllerTypesCore(declaredControllerTypes);

            return declaredControllerTypes;
        }

        protected virtual IEnumerable<Type> GetDeclaredWinControllerTypesCore(IEnumerable<Type> declaredControllerTypes) {
            if (!Executed("GetDeclaredWinControllerTypes", ModuleType.Win))
                declaredControllerTypes = declaredControllerTypes.Union(new[] {
                    typeof(NavigationContainerWinController)
                });
            return declaredControllerTypes;
        }

        protected virtual IEnumerable<Type> GetDeclaredWebControllerTypesCore(IEnumerable<Type> declaredControllerTypes) {
            if (!Executed("GetDeclaredWebControllerTypes", ModuleType.Web))
                declaredControllerTypes = declaredControllerTypes.Union(new[] {
                    typeof(NavigationContainerWebController), 
                    typeof(ActionsClientScriptController),
                    typeof(ActionsClientConfirmationController),
                    typeof(SyntaxHighlightController)
                });

            if (!Executed("DashboardUser", ModuleType.Web)) {
                declaredControllerTypes = declaredControllerTypes.Concat(new[] {
                    typeof(WebDashboardRefreshController)
                });
            }

            return declaredControllerTypes;
        }
        
        internal void OnInitSeqGenerator(CancelEventArgs e) {
            CancelEventHandler handler = InitSeqGenerator;
            handler?.Invoke(this, e);
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator){
            base.AddModelNodeUpdaters(updaterRegistrator);
            updaterRegistrator.AddUpdater(this);
        }


        public override void CustomizeLogics(CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            if (Executed("CustomizeLogics"))
                return;
            customLogics.RegisterLogic(typeof(IModelClassEx), typeof(ModelClassExDomainLogic));
            customLogics.RegisterLogic(typeof(IModelColumnDetailViews), typeof(ModelColumnDetailViewsDomainLogic));
            customLogics.RegisterLogic(typeof(IModelApplicationViews), typeof(ModelApplicationViewsDomainLogic));
        }

        public bool Executed<T>(string name) {
            return !ExecutionConditions<T>() || ExecutedCore(name, typeof(T));
        }

        private bool ExecutedCore(string name, object value = null) {
            value ??= typeof(object);
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

        private bool ExecutionConditions<T>() {
            return typeof(T).IsAssignableFrom(GetType());
        }

        public bool Executed(string name, ModuleType moduleType) {
            if (RuntimeMode){
                var platform = Application.GetPlatform();
                if ((moduleType == ModuleType.Web&&platform==Platform.Web) || (moduleType == ModuleType.Win&&platform==Platform.Win))
//                if ((moduleType == ModuleType.Web&&platform) || (moduleType == ModuleType.Win&&!platform))
                    return ExecutedCore(name, moduleType);
            }
            return (ModuleType != moduleType) || ExecutedCore(name,moduleType);
        }


        public ModuleType ModuleType {
            get {
                if (_moduleType == ModuleType.None) {
                    var toolboxTabNameAttribute = GetType().Attributes<ToolboxTabNameAttribute>().FirstOrDefault();
                    if (toolboxTabNameAttribute != null) {
                        if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabWinModules)
                            _moduleType = ModuleType.Win;
                        else if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabAspNetModules)
                            _moduleType = ModuleType.Web;
                        else if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabWinWebModules)
                            _moduleType = ModuleType.Agnostic;
                        else if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabMobileModules)
                            _moduleType = ModuleType.Mobile;
                    }
                }
                return _moduleType;
            }
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

            if (!Executed("ExtendModelInterfaces")){
                extenders.Add<IModelNode, IModelNodePath>();
                extenders.Add<IModelClass, IModelClassEx>();
                extenders.Add<IModelClass, IModelClassDefaultCriteria>();
                extenders.Add<IModelColumn, IModelColumnDetailViews>();
                extenders.Add<IModelMember, IModelMemberDataStoreForeignKeyCreated>();
                extenders.Add<IModelApplication, IModelApplicationModule>();
                extenders.Add<IModelApplication, IModelApplicationReadonlyParameters>();
                extenders.Add<IModelApplication, IModelApplicationViews>();
//                extenders.Add<IModelApplication, IModelApplicationModelAdapterContexts>();
                extenders.Add<IModelOptions, IModelOptionsNavigationContainer>();
            }

            if (ModuleType == ModuleType.Web)
                ExtendModelWebInterfaces(extenders);
        }

        public virtual void ExtendModelWebInterfaces(ModelInterfaceExtenders extenders) {
            if (!Executed("ExtendModelInterfaces", ModuleType.Web)) {
                extenders.Add<IModelOptions, IModelOptionsCollectionEditMode>();
            }
        }
        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        class CommonModelUpdater:ModelNodesGeneratorUpdater<ImageSourceNodesGenerator> {
            public override void UpdateNode(ModelNode node) {
                node.Application.Logo = "eXpand-Logo";
                ((IModelImageSources) node).AddNode<IModelAssemblyResourceImageSource>(
                    GetType().Assembly.GetName().Name);
            }
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            OnCustomAddGeneratorUpdaters(new GeneratorUpdaterEventArgs(updaters));
            if (!Executed<IModifyModelActionUser>("ModifyModelActionUpdater")) {
                updaters.Add(new ModelActiosNodesUpdater());
                updaters.Add(new ModifyModelActionChoiceItemsUpdater());
            }
            if (Executed("AddGeneratorUpdaters"))
                return;
            updaters.Add(new CommonModelUpdater());
            updaters.Add(new ToggleNavigationActionUpdater());
            updaters.Add(new XpandNavigationItemNodeUpdater());
        }

        protected internal bool RuntimeMode => InterfaceBuilder.RuntimeMode;

        public static Assembly BaseImplAssembly {
            get {
                if (_baseImplAssembly == null)
                    LoadBaseImplAssembly();
                return _baseImplAssembly;
            }
        }

        public static XpoTypeInfoSource XpoTypeInfoSource => XpoTypesInfoHelper.GetXpoTypeInfoSource();

        public static string ConnectionString{
            get => _connectionString != null || !InterfaceBuilder.RuntimeMode ? _connectionString : null;
            internal set => _connectionString = value;
        }

        [SecuritySafeCritical]
        static void LoadBaseImplAssembly() {
            _assemblyString =
                $"Xpand.Persistent.BaseImpl, Version={XpandAssemblyInfo.FileVersion}, Culture=neutral, PublicKeyToken={XpandAssemblyInfo.Token}";

            string baseImplAssemblyName = ConfigurationManager.AppSettings["BaseImplAssembly"];
            if (!String.IsNullOrEmpty(_assemblyString)) {
                _assemblyString =
                    $"{(!String.IsNullOrEmpty(baseImplAssemblyName) ? baseImplAssemblyName : "Xpand.Persistent.BaseImpl")}, Version={XpandAssemblyInfo.FileVersion}, Culture=neutral, PublicKeyToken={XpandAssemblyInfo.Token}";
            }
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
                    "Xpand.Persistent.BaseImpl assembly not found. Please reference it in your front end project and set its Copy Local=true");
        }

        public static string XpandPathInRegistry {
            get {
                if (_xpandPathInRegistry == null) {
                    _xpandPathInRegistry = "";
                    var softwareNode = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node") ??
                                       Registry.LocalMachine.OpenSubKey("Software");
                    var xpandNode = softwareNode?.OpenSubKey(@"Microsoft\.NetFramework\AssemblyFolders\Xpand");
                    if (xpandNode != null)
                        _xpandPathInRegistry = xpandNode.GetValue(null) + "";
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
                    var xpandNode = softwareNode?.OpenSubKey(@"DevExpress\Components\v" + AssemblyInfo.VersionShort);
                    if (xpandNode != null)
                        _dxPathInRegistry = xpandNode.GetValue("RootDirectory") + "";
                }
                return _dxPathInRegistry;
            }
        }

        public static Assembly XpandAssemblyResolve(object sender, ResolveEventArgs args) {
            if (!string.IsNullOrEmpty(XpandPathInRegistry)) {
                var path = Path.Combine(XpandPathInRegistry, args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll");
                if (File.Exists(path))
                    return Assembly.LoadFile(path);
            }
            return null;
        }

        public static Assembly DXAssemblyResolve(object sender, ResolveEventArgs args) {
            if (!string.IsNullOrEmpty(DXPathInRegistry)) {
                var path = Path.Combine(DXPathInRegistry, @"bin\framework\" + args.Name + ".dll");
                if (File.Exists(path))
                    return Assembly.LoadFile(path);
            }
            return null;
        }

        public static Type GetDxBaseImplType(string typeName){
            try {
                if (InterfaceBuilder.RuntimeMode) {
                    AppDomain.CurrentDomain.AssemblyResolve += DXAssemblyResolve;
                    Assembly assembly = Assembly.Load("DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix);
                    XafTypesInfo.Instance.LoadTypes(assembly);
                    var info = XafTypesInfo.Instance.FindTypeInfo(typeName);
                    if (info == null)
                        throw new FileNotFoundException();
                    return info.Type;
                }
            }
            catch (FileNotFoundException) {
                throw new FileNotFoundException(
                    "Please make sure DevExpress.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
            }
            finally {
                AppDomain.CurrentDomain.AssemblyResolve -= DXAssemblyResolve;
            }
            return null;
        }
        

        protected void LoadDxBaseImplType(string typeName){
            var type = GetDxBaseImplType(typeName);
            if (type != null) AdditionalExportedTypes.Add(type);
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            var declaredExportedTypes = base.GetDeclaredExportedTypes().ToArray();
            declaredExportedTypes = !Executed<IModifyModelActionUser>("GetDeclaredExportedTypes")
                ? declaredExportedTypes.Concat(new[] { typeof(ModelConfiguration), SequenceObjectType ,typeof(RuleInfoObject)}).Where(type => type != null).ToArray()
                : declaredExportedTypes;
            if (Application != null && (Application.Security?.UserType == null))
                return declaredExportedTypes.Where(type => !typeof(ISecurityRelated).IsAssignableFrom(type));
            if ( (Application?.Security != null && typeof(IPermissionPolicyUser).IsAssignableFrom(Application.Security.UserType))){
                var oldSecurityTypes = declaredExportedTypes.Where(type => typeof(ISecurityRelated).IsAssignableFrom(type)&&!typeof(ISecurityPermisssionPolicyRelated).IsAssignableFrom(type));
                return declaredExportedTypes.Except(oldSecurityTypes);
            }
            return declaredExportedTypes;
        }

        void AssignSecurityEntities() {
            if (Application != null) {
                if (Application.Security is IRoleTypeProvider roleTypeProvider) {
                    RoleType =XafTypesInfo.Instance.PersistentTypes.First(info => info.Type == roleTypeProvider.RoleType).Type;
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

        public static bool IsLoadingExternalModel() {
            return XafTypesInfo.Instance.GetType() != typeof(TypesInfo);
        }

        public static IEnumerable<Type> CollectExportedTypesFromAssembly(Assembly assembly) {
            var typesList = new ExportedTypeCollection();
            try {
                XafTypesInfo.Instance.LoadTypes(assembly);
                if (Equals(assembly, typeof(XPObject).Assembly)) {
                    typesList.AddRange(XpoTypeInfoSource.XpoBaseClasses);
                }
                else {
                    typesList.AddRange(assembly.GetTypes());
                }
            }
            catch (Exception e) {
                throw new InvalidOperationException(
                    $"Exception occurs while ensure classes from assembly {assembly.FullName}\r\n{e.Message}", e);
            }
            return typesList;
        }

        public Type LoadFromBaseImpl(string typeName) {
            return BaseImplAssembly != null ? LoadFromBaseImplCore(typeName) : null;
        }

        private Type LoadFromBaseImplCore(string typeName) {
            var type = BaseImplAssembly.GetType(typeName);
            XafTypesInfo.Instance.RegisterEntity(type);
            return type;
        }

        protected internal void AddToAdditionalExportedTypes(string[] types) {
            var collection = BaseImplAssembly.GetTypes().Where(type1 => types.Contains(type1.FullName));
            AdditionalExportedTypes.AddRange(collection);
        }

        protected Type[] AddToAdditionalExportedTypes(string nameSpaceName, Assembly assembly) {
            var types = GetTypeInfos(assembly).Where(type1 => String.Join("", type1.Type.Namespace).StartsWith(nameSpaceName));
            var objects = types.Select(info => info.Type).ToArray();
            AdditionalExportedTypes.AddRange(objects);
            return objects;
        }

        private IEnumerable<ITypeInfo> GetTypeInfos(Assembly assembly) {
            var assemblyInfo = AdditionalTypesTypesInfo.FindAssemblyInfo(assembly);
            if (!assemblyInfo.AllTypesLoaded)
                assemblyInfo.LoadTypes();
            return assemblyInfo.Types;
        }

        protected internal Type[] AddToAdditionalExportedTypes(string nameSpaceName) {
            return AddToAdditionalExportedTypes(nameSpaceName, BaseImplAssembly);
        }

        protected void CreateWeaklyTypedCollection(ITypesInfo typesInfo, Type classType, string propertyName) {
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

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            
            OnApplicationModulesManagerSetup(new ApplicationModulesManagerSetupArgs(moduleManager));
            if (Executed("Setup2"))
                return;
            if (RuntimeMode)
                ConnectionString = this.GetConnectionString();
            
        }

        class AssemblyResolver:DefaultAssemblyResolver {
            public override AssemblyDefinition Resolve(AssemblyNameReference name) {
                AssemblyDefinition assemblyDefinition;
                try {
                    assemblyDefinition = base.Resolve(name);
                }
                catch (Exception) {
                    return AssemblyDefinition.ReadAssembly(@$"{AppDomain.CurrentDomain.ApplicationPath()}\{name.Name}.dll",new ReaderParameters(){AssemblyResolver = this});
                }

                return assemblyDefinition;
            }
        }
        private static void LoadAssemblyRegularTypes(){
            var assembly = typeof(XpandModuleBase).Assembly;
            using var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location,new ReaderParameters(ReadingMode.Immediate){AssemblyResolver = new AssemblyResolver()});
            var dxAssembly = AssemblyDefinition.ReadAssembly(typeof(ModuleBase).Assembly.Location);
                
            var typeDefinition =
                dxAssembly.MainModule.Types.First(definition => definition.FullName == typeof(Controller).FullName);
            var typeDefinitions = assemblyDefinition.MainModule.Types.Where(definition => !definition.IsSubclassOf(typeDefinition));
            foreach (var definition in typeDefinitions){
                XafTypesInfo.Instance.FindTypeInfo(assembly.GetType(definition.FullName));
            }
        }

        public override void Setup(XafApplication application) {
            lock (XafTypesInfo.Instance) {
                if (RuntimeMode && ((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(XpoTypeInfoSource)) == null) {
                    XpoTypesInfoHelper.ForceInitialize();
                    new XpandXpoTypeInfoSource((TypesInfo)application.TypesInfo).AssignAsPersistentEntityStore();
                }
            }
            base.Setup(application);
            CheckApplicationTypes();
            if (Executed("Setup"))
                return;
            if (RuntimeMode) {
                ApplicationHelper.Instance.Initialize(application);
            }

            
            application.Disposed+=ApplicationOnDisposed;
            ManifestModuleName ??= application.GetType().Assembly.ManifestModule.Name;
            application.CreateCustomUserModelDifferenceStore += OnCreateCustomUserModelDifferenceStore;
            application.SetupComplete += ApplicationOnSetupComplete;
            application.SettingUp += ApplicationOnSettingUp;
            application.CreateCustomCollectionSource += ApplicationOnCreateCustomCollectionSource;
            if (RuntimeMode) {
                application.LoggedOn += (sender, args) => {
                    RuntimeMemberBuilder.CreateRuntimeMembers(application.Model);
                    _loggedOn = true;
                };
            }
        }

        private void ApplicationOnDisposed(object sender, EventArgs e) {
            ObjectSpaceCreated = false;
            _baseImplAssembly = null;
            CallMonitor.Clear();
        }
        
        private void ApplicationOnCreateCustomCollectionSource(object sender, CreateCustomCollectionSourceEventArgs e) {
            e.CollectionSource = new XpandCollectionSource(e.ObjectSpace, e.ObjectType, e.DataAccessMode, e.Mode);
        }

        private void OnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
            if (!_loggedOn) {
                _extraDiffStores = GetExtraDiffStores().ToList();
                foreach (var extraDiffStore in _extraDiffStores) {
                    e.AddExtraDiffStore(extraDiffStore.Key, extraDiffStore.Value);
                }
                if (_extraDiffStores.Any())
                    e.AddExtraDiffStore("After Setup", new ModelStoreBase.EmptyModelStore());
            }
        }

        private IEnumerable<KeyValuePair<string, ModelDifferenceStore>> GetExtraDiffStores() {
            var stringModelStores = ResourceModelCollector.GetEmbededModelStores();
            foreach (var stringModelStore in stringModelStores) {
                yield return new KeyValuePair<string, ModelDifferenceStore>(stringModelStore.Key, stringModelStore.Value);
            }

            IEnumerable<string> models = Directory.GetFiles(BinDirectory, "*.Xpand.xafml", SearchOption.TopDirectoryOnly);
            models = models.Concat(Directory.GetFiles(BinDirectory, "model.user*.xafml", SearchOption.TopDirectoryOnly))
                    .Where(s => !s.ToLowerInvariant().EndsWith("model.user.xafml"));
            if (Application.GetPlatform()==Platform.Web) {
                models = models.Concat(Directory.GetFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                        "model.user*.xafml", SearchOption.TopDirectoryOnly));
            }
            foreach (var path in models) {
                string fileNameTemplate = Path.GetFileNameWithoutExtension(path);
                var storePath = Path.GetDirectoryName(path);
                var fileModelStore = new FileModelStore(storePath, fileNameTemplate);
                yield return new KeyValuePair<string, ModelDifferenceStore>(fileNameTemplate, fileModelStore);
            }

        }

        public static string BinDirectory => ApplicationHelper.Instance.Application.GetPlatform()==Platform.Web ? AppDomain.CurrentDomain.SetupInformation.PrivateBinPath : AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static bool ObjectSpaceCreated { get; internal set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Type SequenceObjectType { get; set; }

        public static bool IsEasyTesting { get; set; }

        void CheckApplicationTypes() {
            if (RuntimeMode&& !(Application is ITestXafApplication)) {
                foreach (var applicationType in ApplicationTypes()) {
                    if (!applicationType.IsInstanceOfType(Application)) {
                        throw new CannotLoadInvalidTypeException(Application.GetType().FullName + " from " + GetType().FullName + ". " + Environment.NewLine + Application.GetType().FullName + " must implement/derive from " +
                                                                 applicationType.FullName + Environment.NewLine +
                                                                 "Please check folder Demos/Modules/" +
                                                                 GetType().Name.Replace("Module", null) + " to see how to install correctly this module. As an quick workaround you can derive your " + Application.GetType().FullName + " from XpandWinApplication or from XpandWebApplication");
                    }
                }
            }
        }

        void ApplicationOnSettingUp(object sender, SetupEventArgs e) {
            AssignSecurityEntities();
        }

        protected virtual Type[] ApplicationTypes() {
            return new[] { DefaultXafAppType };
        }

        IEnumerable<Attribute> GetAttributes(ITypeInfo type) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(AttributeRegistrator))
                .Descendants.Where(info => !info.IsAbstract).Select(typeInfo => (AttributeRegistrator)typeInfo.Type.CreateInstance())
                .SelectMany(registration => GetAttributes(type, registration));
        }

        private IEnumerable<Attribute> GetAttributes(ITypeInfo type, AttributeRegistrator registrator) {
            return registrator.GetType().IsGenericType && type.Type != registrator.GetType().GetGenericArguments()[0]
                ? Enumerable.Empty<Attribute>()
                : registrator.GetAttributes(type);
        }

        void CreateAttributeRegistratorAttributes(ITypeInfo persistentType) {
            IEnumerable<Attribute> attributes = GetAttributes(persistentType);
            foreach (var attribute in attributes) {
                persistentType.AddAttribute(attribute);
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!Executed("CustomizeTypesInfo")) {
                if (RuntimeMode) {
                    foreach (var persistentType in typesInfo.PersistentTypes) {
                        CreateAttributeRegistratorAttributes(persistentType);
                    }
                }
                CreateXpandDefaultProperty(typesInfo);
                ModelValueOperator.Instance.Register();
                EvaluateCSharpOperator.Instance.Register();
                ConvertInvisibleInAllViewsAttribute(typesInfo);

                AssignSecurityEntities();
                ITypeInfo findTypeInfo = typesInfo.FindTypeInfo(typeof(IModelMember));
                var type = (BaseInfo)findTypeInfo.FindMember("Type");

                var attribute = type.FindAttribute<ModelReadOnlyAttribute>();
                if (attribute != null)
                    type.RemoveAttribute(attribute);

                type = (BaseInfo)typesInfo.FindTypeInfo(typeof(IModelBOModelClassMembers));
                attribute = type.FindAttribute<ModelReadOnlyAttribute>();
                if (attribute != null)
                    type.RemoveAttribute(attribute);
                
            }
            if ((!Executed("CustomizedTypesInfo", ModuleType.Win)))
                EditorAliasForNullableEnums(typesInfo);
            else if (!Executed("CustomizedTypesInfo", ModuleType.Web)) {
                EditorAliasForNullableEnums(typesInfo);
            }

            var info = ModuleManager.Modules.OfType<XpandModuleBase>().Last().GetType();
            if (GetType()== info) {
                var keyValuePairs = CallMonitor.Keys.ToList();
                foreach (var pair in keyValuePairs) {
                    CallMonitor[pair].Clear();
                }
            }
        }

        private void EditorAliasForNullableEnums(ITypesInfo typesInfo) {
            var typeInfos = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo<PropertyEditor>()).Where(info => {
                var editorAttribute = info.FindAttribute<PropertyEditorAttribute>();
                return editorAttribute?.GetFieldValue("alias") != null;
            }).ToArray();
            var memberInfos = typesInfo.PersistentTypes.SelectMany(info => info.OwnMembers)
                .Where(info => info.MemberType.IsNullableType() && info.MemberType.GetGenericArguments()[0].IsEnum &&
                               info.FindAttribute<EditorAliasAttribute>() != null);
            foreach (var memberInfo in memberInfos) {
                var editorAliasAttribute = memberInfo.FindAttribute<EditorAliasAttribute>();
                var typeInfo = typeInfos.FirstOrDefault(info => {
                    var propertyEditorAttribute = info.FindAttribute<PropertyEditorAttribute>();
                    return (string)propertyEditorAttribute.GetFieldValue("alias") == editorAliasAttribute.Alias;
                });
                if (typeInfo != null)
                    memberInfo.AddAttribute(new ModelDefaultAttribute("PropertyEditorType", typeInfo.Type.FullName));
            }
        }

        private static void ConvertInvisibleInAllViewsAttribute(ITypesInfo typesInfo) {
            foreach (var memberInfo in typesInfo.PersistentTypes.SelectMany(typeInfo =>
                typeInfo.OwnMembers.Where(info => info.FindAttribute<InvisibleInAllViewsAttribute>() != null))
                .ToList()) {
                memberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
                memberInfo.AddAttribute(new VisibleInListViewAttribute(false));
                memberInfo.AddAttribute(new VisibleInLookupListViewAttribute(false));
            }
        }

        private static void CreateXpandDefaultProperty(ITypesInfo typesInfo) {
            var infos = typesInfo.PersistentTypes.Select(info => new { TypeInfo = info, Attribute = info.FindAttribute<XpandDefaultPropertyAttribute>() })
                    .Where(arg => arg.Attribute != null).ToList();
            foreach (var info in infos.Where(arg => arg.TypeInfo.Base.FindAttribute<XpandDefaultPropertyAttribute>() == null)) {
                var classInfo = info.TypeInfo.QueryXPClassInfo();
                var memberInfo = new XpandCalcMemberInfo(classInfo, info.Attribute.MemberName, typeof(string), info.Attribute.Expression);
                if (info.Attribute.InVisibleInAllViews)
                    memberInfo.AddAttribute(new InvisibleInAllViewsAttribute());
                typesInfo.RefreshInfo(info.TypeInfo);
                ((TypeInfo)info.TypeInfo).DefaultMember = info.TypeInfo.FindMember(info.Attribute.MemberName);
            }
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
            lock (LockObject) {
                RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
                if (Executed("ApplicationOnSetupComplete"))
                    return;
                Application.SetClientSideSecurity();
            }
        }
        
        public void UpdateNode(IModelMemberEx node, IModelApplication application) {
            node.ClearValue(ex => ex.IsCustom);
            node.ClearValue(ex => ex.IsCalculated);
        }

        public static void RemoveCall(string name, ApplicationModulesManager applicationModulesManager) {
            CallMonitor?.TryRemove(new KeyValuePair<string, ApplicationModulesManager>(name, applicationModulesManager),out _);
        }
    }

    public class ApplicationModulesManagerSetupArgs : EventArgs{
        public ApplicationModulesManagerSetupArgs(ApplicationModulesManager moduleManager){
            ModuleManager = moduleManager;
        }

        public ApplicationModulesManager ModuleManager { get; }
    }


    public class GeneratorUpdaterEventArgs : EventArgs {
        public GeneratorUpdaterEventArgs(ModelNodesGeneratorUpdaters updaters) {
            Updaters = updaters;
        }

        public ModelNodesGeneratorUpdaters Updaters { get; }
    }

    public class ExtendingModelInterfacesArgs : EventArgs {
        public ExtendingModelInterfacesArgs(ModelInterfaceExtenders extenders) {
            Extenders = extenders;
        }

        public ModelInterfaceExtenders Extenders { get; }
    }

    public static class ModuleBaseExtensions {
        public static string GetConnectionString(this ModuleBase moduleBase) {
            if (moduleBase.Application.ObjectSpaceProviders.Count == 0) {
                return moduleBase.Application.ConnectionString;
            }
            var provider = moduleBase.Application.ObjectSpaceProviders.OfType<IXpandObjectSpaceProvider>().FirstOrDefault();
            if (provider != null) {
                return provider.DataStoreProvider.ConnectionString;
            }
            var xpObjectSpaceProvider = moduleBase.Application.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>().FirstOrDefault();
            if (xpObjectSpaceProvider!=null) {
                return ((IXpoDataStoreProvider)xpObjectSpaceProvider.GetFieldValue("dataStoreProvider")).ConnectionString;
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
            handler?.Invoke(this, EventArgs.Empty);
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            XpandModuleBase.ObjectSpaceCreated = false;
            var xafApplication = ((XafApplication)sender);
            xafApplication.LoggedOff -= ApplicationOnLoggedOff;
            if (xafApplication.GetPlatform()==Platform.Win){
                Application.ObjectSpaceCreated += ConnectionStringActions;
            }
            else
                XpandModuleBase.RemoveCall(ConnectionStringHelperName, _xpandModuleBase.ModuleManager);
        }


        void ConnectionStringActions(object sender, ObjectSpaceCreatedEventArgs e) {
            XpandModuleBase.ObjectSpaceCreated = true;
            var xafApplication = ((XafApplication)sender);
            xafApplication.ObjectSpaceCreated -= ConnectionStringActions;
            if (String.CompareOrdinal(_currentConnectionString, Application.ConnectionString) != 0) {
                _currentConnectionString = Application.ConnectionString;
                XpandModuleBase.ConnectionString = _xpandModuleBase.GetConnectionString();
                OnConnectionStringUpdated();
            }
        }

        protected XafApplication Application => _xpandModuleBase.Application;

        protected bool RuntimeMode => _xpandModuleBase.RuntimeMode;

        public void Attach(XpandModuleBase moduleBase) {
            _xpandModuleBase = moduleBase;
            if (RuntimeMode && !Executed(ConnectionStringHelperName)) {
                Application.ObjectSpaceCreated += ConnectionStringActions;
                Application.LoggedOff += ApplicationOnLoggedOff;
                Application.DatabaseVersionMismatch += ApplicationOnDatabaseVersionMismatch;
            }
        }

        void ApplicationOnDatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs databaseVersionMismatchEventArgs) {
            var xafApplication = ((XafApplication)sender);
            xafApplication.DatabaseVersionMismatch -= ApplicationOnDatabaseVersionMismatch;
            xafApplication.StatusUpdating += XafApplicationOnStatusUpdating;
        }

        void XafApplicationOnStatusUpdating(object sender, StatusUpdatingEventArgs statusUpdatingEventArgs) {
            if (statusUpdatingEventArgs.Context == ApplicationStatusMessageId.UpdateDatabaseData.ToString()) {
                Application.StatusUpdating -= XafApplicationOnStatusUpdating;
                ConnectionStringActions(Application, null);
            }
        }

        bool Executed(string name) {
            return _xpandModuleBase.Executed(name);
        }
    }

    public class EasyTestModule : ModuleBase {
        protected override IEnumerable<Type> GetDeclaredControllerTypes() {
            var declaredControllerTypes = base.GetDeclaredControllerTypes();
            return declaredControllerTypes.Concat(new[] { typeof(EasyTestController) });
        }
    }
}