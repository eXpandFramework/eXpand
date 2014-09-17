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
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
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
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.General {
    public interface IXpandModuleBase {
        event EventHandler<GeneratorUpdaterEventArgs> CustomAddGeneratorUpdaters;
        event EventHandler ApplicationModulesManagerSetup;
        ModuleTypeList RequiredModuleTypes { get; }
        XafApplication Application { get; }
    }


    public enum ModuleType{
        None,
        Agnostic,
        Win,
        Web
    }
    [ToolboxItem(false)]
    public class XpandModuleBase : ModuleBase, IModelNodeUpdater<IModelMemberEx>, IModelXmlConverter, IXpandModuleBase{
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
        private static readonly HashSet<Type> _disabledControllerTypes = new HashSet<Type>();
        private static readonly object _disabledControllerTypesLock = new object();
        private ModuleType _moduleType;
        private bool _customUserModelDifferenceStore;

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

        public XpandModuleBase(){
            AdditionalExportedTypes.Add(typeof(MessageBoxTextMessage));
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
            get{
                if (!_isHosted.HasValue) {
                    _isHosted = GetIsHosted(CaptionHelper.ApplicationModel);
                }
                return _isHosted.Value;
            }
        }

        public static bool GetIsHosted(IModelApplication application) {
            var modelSources = application as IModelSources;
            if (modelSources == null)
                return Assembly.GetEntryAssembly() == null;
            var moduleBases = modelSources.Modules;
            return GetIsHosted(moduleBases);
        }

        private static bool GetIsHosted(IEnumerable<ModuleBase> moduleBases){
            return moduleBases.Any(@base =>{
                var attribute =XafTypesInfo.Instance.FindTypeInfo(@base.GetType()).FindAttribute<ToolboxItemFilterAttribute>();
                return attribute != null && attribute.FilterString == "Xaf.Platform.Web";
            });
        }


        public static void DisableControllers(params Type[] types){
            lock (_disabledControllerTypesLock){
                foreach (Type type in types){
                    if (!_disabledControllerTypes.Contains(type))
                        _disabledControllerTypes.Add(type);
                }
            }
        }

        private IEnumerable<Type> FilterDisabledControllers(IEnumerable<Type> controllers){
            if (controllers == null) return null;
            lock (_disabledControllerTypesLock){
                return controllers.Where(t => !_disabledControllerTypes.Contains(t)).ToArray();
            }
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            return FilterDisabledControllers(GetDeclaredControllerTypesCore());
        }

        protected virtual IEnumerable<Type> GetDeclaredControllerTypesCore() {
            var declaredControllerTypes = base.GetDeclaredControllerTypes();
            if (!Executed<IDashboardInteractionUser>("DashboardUser")) {
                declaredControllerTypes =declaredControllerTypes.Concat(new[]
                    {typeof (DashboardInteractionController), typeof (WebDashboardRefreshController)});
            }
            if (!Executed<IModuleSupportUploadControl>("SupportUploadControl")) {
                declaredControllerTypes =declaredControllerTypes.Concat(new[] { typeof(UploadControlModelAdaptorController) });
            }
            if (!Executed<IModifyModelActionUser>("ModifyModelActionControllerTypes")) {
                declaredControllerTypes = declaredControllerTypes.Concat(new[] { typeof(ActionModifyModelController), typeof(ResetViewModelController), typeof(ModelConfigurationController) });
            }
            if (!Executed("GetDeclaredControllerTypes")) {
                declaredControllerTypes= declaredControllerTypes.Union(new[]{
                    typeof (CreatableItemController), typeof (FilterByColumnController),
                    typeof (CreateExpandAbleMembersViewController), typeof (HideFromNewMenuViewController),
                    typeof (CustomAttibutesController), typeof (NotifyMembersController),
                    typeof (XpandModelMemberInfoController), typeof (XpandLinkToListViewController),
                    typeof(ModifyObjectSpaceController),typeof (ActionItemsFromModelController),typeof(ActionModelChoiceItemController),
                    typeof (ModelViewSavingController),typeof (NavigationContainerController),typeof(ModelController)
                });
            }
            if (!Executed("GetDeclaredWinControllerTypes",ModuleType.Win))
                declaredControllerTypes = declaredControllerTypes.Union(new[] { typeof(InvalidEditorActionBaseControllerWin), typeof (NavigationContainerWinController) });
            if (!Executed("GetDeclaredWebControllerTypes", ModuleType.Web))
                declaredControllerTypes = declaredControllerTypes.Union(new[]{typeof(InvalidEditorActionBaseWebController),typeof (NavigationContainerWebController)});

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
            customLogics.RegisterLogic(typeof(IModelTypesInfoProvider), typeof(TypesInfoProviderDomainLogic));
            customLogics.RegisterLogic(typeof(IModelColumnDetailViews), typeof(ModelColumnDetailViewsDomainLogic));
            customLogics.RegisterLogic(typeof(IModelApplicationListViews), typeof(ModelApplicationListViewsDomainLogic));
        }

        public bool Executed<T>(string name){
            return !ExecutionConditions<T>() || ExecutedCore(name,typeof(T));
        }

        private bool ExecutedCore(string name,Type value=null){
            value = value ?? typeof (object);
            var keyValuePair = new KeyValuePair<string, ApplicationModulesManager>(name, ModuleManager);
            if (CallMonitor.ContainsKey(keyValuePair)){
                if (!CallMonitor.GetValues(keyValuePair, true).Contains(value)){
                    CallMonitor.Add(keyValuePair, value);
                    return false;
                }
                return true;
            }
            CallMonitor.Add(keyValuePair, value);

            return false;
        }

        private bool ExecutionConditions<T>(){
            return typeof(T).IsAssignableFrom(GetType());
        }

        public bool Executed(string name,ModuleType moduleType){
            return ModuleType != moduleType || ExecutedCore(name);
        }


        public ModuleType ModuleType{
            get{
                if (_moduleType==ModuleType.None){
                    var toolboxTabNameAttribute = GetType().Attributes<ToolboxTabNameAttribute>().FirstOrDefault();
                    if (toolboxTabNameAttribute!=null){
                        if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabWinModules)
                            _moduleType = ModuleType.Win;
                        else if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabAspNetModules)
                            _moduleType = ModuleType.Web;
                        else if (toolboxTabNameAttribute.TabName == XpandAssemblyInfo.TabWinWebModules)
                            _moduleType = ModuleType.Agnostic;
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

            if (Executed("ExtendModelInterfaces"))
                return;

            extenders.Add<IModelNode, IModelNodePath>();
            extenders.Add<IModelClass, IModelClassInvalidEditorAction>();
            extenders.Add<IModelDetailView, IModelDetailViewInvalidEditorAction>();
            extenders.Add<IModelOptions, IModelOptionMemberPersistent>();
            extenders.Add<IModelOptions, IModelOptionsMergedDifferenceStrategy>();
            extenders.Add<IModelClass, IModelClassEx>();
            extenders.Add<IModelClass, IModelClassDefaultCriteria>();
            extenders.Add<IModelColumn, IModelColumnDetailViews>();
            extenders.Add<IModelMember, IModelMemberDataStoreForeignKeyCreated>();
            extenders.Add<IModelApplication, IModelTypesInfoProvider>();
            extenders.Add<IModelApplication, IModelApplicationModule>();
            extenders.Add<IModelApplication, IModelApplicationReadonlyParameters>();
            extenders.Add<IModelApplication, IModelApplicationListViews>();
            extenders.Add<IModelApplication, IModelApplicationModelAdapterContexts>();
            extenders.Add<IModelObjectView, IModelObjectViewMergedDifferences>();
            
            
        }
        public static Type UserType { get; set; }

        public static Type RoleType { get; set; }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            OnCustomAddGeneratorUpdaters(new GeneratorUpdaterEventArgs(updaters));
            if (!Executed<IModifyModelActionUser>("ModifyModelActionUpdater")) {
                updaters.Add(new ModelActiosNodesUpdater());
                updaters.Add(new ModifyModelActionChoiceItemsUpdater());
            }
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
                    "Xpand.Persistent.BaseImpl assembly not found. Please reference it in your front end project and set its Copy Local=true");
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
                        var xpandNode = softwareNode.OpenSubKey(@"DevExpress\Components\v"+AssemblyInfo.VersionShort);
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

        protected void LoadDxBaseImplType(string typeName) {
            try {
                if (RuntimeMode) {
                    AppDomain.CurrentDomain.AssemblyResolve += DXAssemblyResolve;
                    Assembly assembly = Assembly.Load("DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix);
                    Application.TypesInfo.LoadTypes(assembly);
                    var info = Application.TypesInfo.FindTypeInfo(typeName);
                    if (info == null)
                        throw new FileNotFoundException();
                    Type typeInfo = info.Type;
                    AdditionalExportedTypes.Add(typeInfo);
                }
            }
            catch (FileNotFoundException) {
                throw new FileNotFoundException(
                    "Please make sure DevExpress.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
            }
            finally {
                AppDomain.CurrentDomain.AssemblyResolve -= DXAssemblyResolve;
            }
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes(){
            var declaredExportedTypes = base.GetDeclaredExportedTypes();
            return !Executed<IModifyModelActionUser>("GetDeclaredExportedTypes")
                ? declaredExportedTypes.Concat(new[]{typeof (ModelConfiguration)})
                : declaredExportedTypes;
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
                throw new InvalidOperationException(String.Format("Exception occurs while ensure classes from assembly {0}\r\n{1}", assembly.FullName,e.Message), e);
            }
            return typesList;
        }

        public Type LoadFromBaseImpl(string typeName){
            return BaseImplAssembly != null ? LoadFromBaseImplCore(typeName) : null;
        }

        private Type LoadFromBaseImplCore(string typeName){
            var type = BaseImplAssembly.GetType(typeName);
            XafTypesInfo.Instance.RegisterEntity(type);
            return type;
        }

        protected internal void AddToAdditionalExportedTypes(string[] types) {
            if (RuntimeMode) {
                var collection = BaseImplAssembly.GetTypes().Where(type1 => types.Contains(type1.FullName));
                AdditionalExportedTypes.AddRange(collection);
            }
        }

        protected void AddToAdditionalExportedTypes(string nameSpaceName, Assembly assembly) {
            if (RuntimeMode) {
                var types =assembly.GetTypes().Where(type1 => String.Join("", new[]{type1.Namespace}).StartsWith(nameSpaceName));
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
            lock (XafTypesInfo.Instance) {
                if (RuntimeMode && XafTypesInfo.PersistentEntityStore == null)
                    XafTypesInfo.SetPersistentEntityStore(new XpandXpoTypeInfoSource((TypesInfo)application.TypesInfo));
            }
            base.Setup(application);
            if (RuntimeMode)
                ApplicationHelper.Instance.Initialize(application);
            CheckApplicationTypes();
            var helper = new ConnectionStringHelper();
            helper.Attach(this);
            var generatorHelper = new SequenceGeneratorHelper();
            generatorHelper.Attach(this);
            helper.ConnectionStringUpdated += (sender, args) => generatorHelper.InitializeSequenceGenerator();
                
            if (Executed("Setup"))
                return;
            if (ManifestModuleName == null)
                ManifestModuleName = application.GetType().Assembly.ManifestModule.Name;
            application.CreateCustomUserModelDifferenceStore+=OnCreateCustomUserModelDifferenceStore;
            application.SetupComplete += ApplicationOnSetupComplete;
            application.SettingUp += ApplicationOnSettingUp;
            application.CreateCustomCollectionSource+=ApplicationOnCreateCustomCollectionSource;
            if (RuntimeMode){
                application.LoggedOn += (sender, args) => RuntimeMemberBuilder.CreateRuntimeMembers(application.Model);
            }
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB){
            var moduleUpdaters = base.GetModuleUpdaters(objectSpace, versionFromDB);
            if (Executed<ISequenceGeneratorUser>("GetModuleUpdaters")) 
                return moduleUpdaters;
            if (SequenceGenerator.UseGuidKey)
                moduleUpdaters = moduleUpdaters.Concat(new[]{new SequenceGeneratorUpdater(objectSpace, Version)});
            return moduleUpdaters;
        }

        private void ApplicationOnCreateCustomCollectionSource(object sender, CreateCustomCollectionSourceEventArgs e){
            e.CollectionSource=new XpandCollectionSource(e.ObjectSpace, e.ObjectType, e.DataAccessMode, e.Mode);
        }

        private void OnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
            if (IsHosted&&_customUserModelDifferenceStore)
                return;
            _customUserModelDifferenceStore = true;
            var stringModelStores = ResourceModelCollector.GetEmbededModelStores();
            foreach (var stringModelStore in stringModelStores){
                e.AddExtraDiffStore(stringModelStore.Key, stringModelStore.Value);    
            }
            IEnumerable<string> models = Directory.GetFiles(BinDirectory,"*.Xpand.xafml",SearchOption.TopDirectoryOnly);
            models = models.Concat(Directory.GetFiles(BinDirectory, "model.user*.xafml", SearchOption.TopDirectoryOnly)).Where(s => !s.ToLowerInvariant().EndsWith("model.user.xafml"));
            if (IsHosted){
                models=models.Concat(Directory.GetFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,"model.user*.xafml",SearchOption.TopDirectoryOnly));
            }
            foreach (var path in models){
                string fileNameTemplate = Path.GetFileNameWithoutExtension(path);
                var storePath = Path.GetDirectoryName(path);
                var fileModelStore = new FileModelStore(storePath,fileNameTemplate);
                e.AddExtraDiffStore(fileNameTemplate,fileModelStore);
            }
        }

        public static string BinDirectory{
            get{return IsHosted ? AppDomain.CurrentDomain.SetupInformation.PrivateBinPath : AppDomain.CurrentDomain.SetupInformation.ApplicationBase;}
        }

        public static bool ObjectSpaceCreated { get; internal set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Type SequenceObjectType { get; set; }

        public static bool IsEasyTesting { get; set; }

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

        IEnumerable<Attribute> GetAttributes(ITypeInfo type) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(AttributeRegistrator))
                .Descendants.Where(info => !info.IsAbstract).Select(typeInfo => (AttributeRegistrator)typeInfo.Type.CreateInstance())
                .SelectMany(registrator => GetAttributes(type, registrator));
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
            if (Executed("CustomizeTypesInfo"))
                return;
            if (RuntimeMode) {
                foreach (var persistentType in typesInfo.PersistentTypes) {
                    CreateAttributeRegistratorAttributes(persistentType);
                }
            }
            CreateXpandDefaultProperty(typesInfo);
            ModelValueOperator.Register();
            foreach (var memberInfo in typesInfo.PersistentTypes.SelectMany(info => info.Members).Where(info => info.FindAttribute<InvisibleInAllViewsAttribute>() != null).ToList()) {
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

            if (!SequenceGenerator.UseGuidKey) {
                var typeInfo = typesInfo.FindTypeInfo(SequenceObjectType);
                var memberInfo = (BaseInfo)typeInfo.FindMember("Oid");
                memberInfo.RemoveAttribute(new KeyAttribute(false));
                memberInfo = (BaseInfo)typeInfo.FindMember<ISequenceObject>(o => o.TypeName);
                memberInfo.AddAttribute(new KeyAttribute(false));
            }

        }

        private static void CreateXpandDefaultProperty(ITypesInfo typesInfo){
            var infos =typesInfo.PersistentTypes.Select(info => new{TypeInfo = info, Attribute = info.FindAttribute<XpandDefaultPropertyAttribute>()})
                    .Where(arg => arg.Attribute != null).ToList();
            foreach (var info in infos.Where(arg => arg.TypeInfo.Base.FindAttribute<XpandDefaultPropertyAttribute>()==null)){
                var classInfo = Dictiorary.GetClassInfo(info.TypeInfo.Type);
                var memberInfo = new XpandCalcMemberInfo(classInfo, info.Attribute.MemberName,typeof (string), info.Attribute.Expression);
                if (info.Attribute.InVisibleInAllViews)
                    memberInfo.AddAttribute(new InvisibleInAllViewsAttribute());
                typesInfo.RefreshInfo(info.TypeInfo);
                ((TypeInfo)info.TypeInfo).DefaultMember = info.TypeInfo.FindMember(info.Attribute.MemberName);
            }
        }

        void ModifySequenceObjectWhenMySqlDatalayer(ITypesInfo typesInfo) {
            var typeInfo = typesInfo.FindTypeInfo(SequenceObjectType);
            if (IsMySql(typeInfo)){
                var memberInfo = (XafMemberInfo) typeInfo.FindMember<ISequenceObject>(o => o.TypeName);
                memberInfo.RemoveAttributes<SizeAttribute>();
                memberInfo.AddAttribute(new SizeAttribute(255));
            }

        }
        public static bool IsMySql() {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(SequenceObjectType);
            return IsMySql(typeInfo);
        }

        private static bool IsMySql(ITypeInfo typeInfo) {
            var sequenceObjectObjectSpaceProvider = GetSequenceObjectObjectSpaceProvider(typeInfo.Type);
            if (sequenceObjectObjectSpaceProvider != null) {
                var helper = new ConnectionStringParser(sequenceObjectObjectSpaceProvider.ConnectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                return providerType == MySqlConnectionProvider.XpoProviderTypeString;
            }
            return false;
        }

        static IObjectSpaceProvider GetSequenceObjectObjectSpaceProvider(Type type) {
            return (ApplicationHelper.Instance.Application.ObjectSpaceProviders.Select(objectSpaceProvider 
                => new{objectSpaceProvider, originalObjectType = objectSpaceProvider.EntityStore.GetOriginalType(type)})
                .Where(@t =>(@t.originalObjectType != null) &&@t.objectSpaceProvider.EntityStore.RegisteredEntities.Contains(@t.originalObjectType))
                .Select(@t => @t.objectSpaceProvider)).FirstOrDefault();
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

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            lock (_lockObject) {
                RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
                ModifySequenceObjectWhenMySqlDatalayer(XafTypesInfo.Instance);
            }
        }

        public void UpdateNode(IModelMemberEx node, IModelApplication application) {
            node.ClearValue(ex => ex.IsCustom);
            node.ClearValue(ex => ex.IsCalculated);
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
            var xafApplication = ((XafApplication) sender);
            xafApplication.DatabaseVersionMismatch-=ApplicationOnDatabaseVersionMismatch;
            xafApplication.StatusUpdating+=XafApplicationOnStatusUpdating;
        }

        void XafApplicationOnStatusUpdating(object sender, StatusUpdatingEventArgs statusUpdatingEventArgs) {
            if (statusUpdatingEventArgs.Context == ApplicationStatusMesssageId.UpdateDatabaseData.ToString()) {
                Application.StatusUpdating -= XafApplicationOnStatusUpdating;
                ApplicationOnObjectSpaceCreated(Application, null);
            }
        }

        bool Executed(string name) {
            return _xpandModuleBase.Executed(name);
        }


    }
}