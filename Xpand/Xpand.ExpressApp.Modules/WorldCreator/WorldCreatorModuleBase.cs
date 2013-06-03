using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.NodeUpdaters;
using Xpand.Persistent.Base.PersistentMetaData;


namespace Xpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase : XpandModuleBase {
        private static string _connectionString;
        List<Type> _dynamicModuleTypes = new List<Type>();
        static ExistentTypesMemberCreator _existentTypesMemberCreator;

        public List<Type> DynamicModuleTypes {
            get { return _dynamicModuleTypes; }
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode)
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.PersistentMetaData");
            application.CreateCustomObjectSpaceProvider += ApplicationOnCreateCustomObjectSpaceProvider;
        }
        private void ApplicationOnCreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs createCustomObjectSpaceProviderEventArgs) {
            if (!(createCustomObjectSpaceProviderEventArgs.ObjectSpaceProvider is IXpandObjectSpaceProvider))
                Application.CreateCustomObjectSpaceprovider(createCustomObjectSpaceProviderEventArgs, "WorldCreator");
        }

        public static string FullConnectionString {
            get {
                return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
        }
        protected override void OnApplicationInitialized(XafApplication xafApplication) {
            if (xafApplication == null)
                return;
            if (RuntimeMode)
                _connectionString = FullConnectionString;
            base.OnApplicationInitialized(xafApplication);
        }



        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            WCTypesInfo.Instance.Register(GetAdditionalClasses(moduleManager));
            if (Application == null || GetPath() == null)
                return;
            Application.SettingUp += ApplicationOnSettingUp;
            if (FullConnectionString != null) {
                var xpoMultiDataStoreProxy = new MultiDataStoreProxy(FullConnectionString, GetReflectionDictionary());
                using (var dataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy)) {
                    using (var session = new Session(dataLayer)) {
                        using (var unitOfWork = new UnitOfWork(session.DataLayer)) {
                            RunUpdaters(session);
                            AddDynamicModules(moduleManager, unitOfWork);
                        }
                    }
                }
            } else {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(CompileEngine.XpandExtension));
                foreach (var assembly1 in assemblies) {
                    moduleManager.AddModule(assembly1.GetTypes().Single(type => typeof(ModuleBase).IsAssignableFrom(type)));
                }
            }

            Application.SetupComplete += ApplicationOnSetupComplete;

        }

        void RunUpdaters(Session session) {
            foreach (WorldCreatorUpdater worldCreatorUpdater in GetWorldCreatorUpdaters(session)) {
                worldCreatorUpdater.Update();
            }
        }


        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var session = (((XPObjectSpace)Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false))).Session;
            MergeTypes(new UnitOfWork(session.DataLayer));

        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            if (FullConnectionString != null && _existentTypesMemberCreator != null) {
                _existentTypesMemberCreator = new ExistentTypesMemberCreator();
                var xpoMultiDataStoreProxy = new MultiDataStoreProxy(FullConnectionString, GetReflectionDictionary());
                var simpleDataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy);
                var session = new Session(simpleDataLayer);
                _existentTypesMemberCreator.CreateMembers(session);
            }
            return base.GetDeclaredExportedTypes();
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(WorldCreatorUpdater)).Descendants.Select(typeInfo => (WorldCreatorUpdater)ReflectionHelper.CreateObject(typeInfo.Type, session));
        }

        ReflectionDictionary GetReflectionDictionary() {
            var externalModelBusinessClassesList = GetAdditionalClasses(Application.Modules);
            Type persistentAssemblyInfoType = externalModelBusinessClassesList.FirstOrDefault(type1 => typeof(IPersistentAssemblyInfo).IsAssignableFrom(type1));
            if (persistentAssemblyInfoType == null)
                throw new ArgumentNullException("Add a business object that implements " +
                                                typeof(IPersistentAssemblyInfo).FullName + " at your AdditionalBusinessClasses (module.designer.cs)");
            IEnumerable<Type> types = persistentAssemblyInfoType.Assembly.GetTypes().Where(type => (type.Namespace + "").StartsWith(persistentAssemblyInfoType.Namespace + ""));
            var reflectionDictionary = new ReflectionDictionary();
            foreach (var type in types) {
                reflectionDictionary.QueryClassInfo(type);
            }
            return reflectionDictionary;
        }

        void ApplicationOnSettingUp(object sender, SetupEventArgs setupEventArgs) {
            CreateDataStore(setupEventArgs);
        }

        void CreateDataStore(SetupEventArgs setupEventArgs) {
            var objectSpaceProvider = setupEventArgs.SetupParameters.ObjectSpaceProvider as IXpandObjectSpaceProvider;
            if (objectSpaceProvider == null)
                throw new NotImplementedException("WorldCreator ObjectSpaceProvider does not implement " + typeof(IXpandObjectSpaceProvider).FullName);
        }

        void AddDynamicModules(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork) {
            unitOfWork.LockingOption = LockingOption.None;
            Type assemblyInfoType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
            List<IPersistentAssemblyInfo> persistentAssemblyInfos =
                new XPCollection(unitOfWork, assemblyInfoType).OfType<IPersistentAssemblyInfo>().Where(IsValidAssemblyInfo(moduleManager)).ToList();
            _dynamicModuleTypes = new CompileEngine().CompileModules(persistentAssemblyInfos, GetPath());
            foreach (var definedModule in _dynamicModuleTypes) {
                moduleManager.AddModule(definedModule);
                var module = moduleManager.Modules.FindModule(definedModule);
                moduleManager.ControllersManager.RegisterControllerTypes(module.GetControllerTypes().ToArray());
            }
            unitOfWork.CommitChanges();
        }

        Func<IPersistentAssemblyInfo, bool> IsValidAssemblyInfo(ApplicationModulesManager moduleManager) {
            return info => !info.DoNotCompile && moduleManager.Modules.FirstOrDefault(@base => @base.Name == "Dynamic" + info.Name + "Module") == null;
        }

        public abstract string GetPath();

        void MergeTypes(UnitOfWork unitOfWork) {
            var persistentTypes =
                _dynamicModuleTypes.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof(IXPSimpleObject).IsAssignableFrom(type)));
            var sqlDataStore = XpoDefault.GetConnectionProvider(FullConnectionString, AutoCreateOption.DatabaseAndSchema) as ISqlDataStore;
            if (sqlDataStore != null) {
                IDbCommand dbCommand = sqlDataStore.CreateCommand();
                new XpoObjectMerger().MergeTypes(unitOfWork, persistentTypes.ToList(), dbCommand);
            }
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DynamicModuleTypes));
        }


    }
}