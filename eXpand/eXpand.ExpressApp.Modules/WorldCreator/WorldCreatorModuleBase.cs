using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.NodeUpdaters;
using eXpand.Persistent.Base.PersistentMetaData;
using TypesInfo = eXpand.ExpressApp.WorldCreator.Core.TypesInfo;

namespace eXpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase:ModuleBase {
        string _connectionString;
        List<Type> _definedModules=new List<Type>();

        public List<Type> DefinedModules{
            get { return _definedModules; }
        }

        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            if (Application == null||GetPath()== null)
                return;
            
            TypesInfo.Instance.AddTypes(GetAdditionalClasses());
            Application.SettingUp+=ApplicationOnSettingUp;


            RunUpdaters();

            using (SimpleDataLayer simpleDataLayer = XpoMultiDataStoreProxy.GetDataLayer(_connectionString, GetReflectionDictionary(), TypesInfo.Instance.PersistentAssemblyInfoType)) {
                using (var session = new Session(simpleDataLayer)) {
                    AddDynamicModules(moduleManager, session);
//                Application.SetupComplete += (sender, args) => {
//                    mergeTypes(unitOfWork);
//                    Application.ObjectSpaceProvider.CreateUpdatingSession().UpdateSchema();
//                };
                    var existentTypesMemberCreator = new ExistentTypesMemberCreator();
                    existentTypesMemberCreator.CreateMembers(session, TypesInfo.Instance);
                }
            }
        }

        void RunUpdaters() {
            var xpoMultiDataStoreProxy = new XpoMultiDataStoreProxy(_connectionString,GetReflectionDictionary());

            using (var dataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy)) {
                using (var session = new Session(dataLayer)) {
                    foreach (var worldCreatorUpdater in GetWorldCreatorUpdaters(session)){
                        worldCreatorUpdater.CreatePersistentAssemblies();
                    }
                }
            }
            
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof (WorldCreatorUpdater)).Descendants.Select(
                typeInfo => (WorldCreatorUpdater) ReflectionHelper.CreateObject(typeInfo.Type, session));
        }

        ReflectionDictionary GetReflectionDictionary() {
            Type persistentAssemblyInfoType = TypesInfo.Instance.PersistentAssemblyInfoType;
            IEnumerable<Type> types = persistentAssemblyInfoType.Assembly.GetTypes().Where(type => type.Namespace.StartsWith(persistentAssemblyInfoType.Namespace));
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
            var objectSpaceProvider = setupEventArgs.SetupParameters.ObjectSpaceProvider as IObjectSpaceProvider;
            if (objectSpaceProvider== null)
                throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IObjectSpaceProvider).FullName);
        }

        public void AddDynamicModules(ApplicationModulesManager moduleManager, Session session){
            Type assemblyInfoType = TypesInfo.Instance.PersistentAssemblyInfoType;
            List<IPersistentAssemblyInfo> persistentAssemblyInfos =
                new XPCollection(session, assemblyInfoType).Cast<IPersistentAssemblyInfo>().Where(info => !info.DoNotCompile &&
                    moduleManager.Modules.Where(@base => @base.Name == "Dynamic" + info.Name + "Module").FirstOrDefault() ==null).ToList();
            _definedModules = new CompileEngine().CompileModules(persistentAssemblyInfos,GetPath());
            foreach (var definedModule in _definedModules){
                moduleManager.AddModule(definedModule);
            }
        }

        public abstract string GetPath();

        void mergeTypes(UnitOfWork unitOfWork){
            IEnumerable<Type> persistentTypes =
                _definedModules.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof(IXPSimpleObject).IsAssignableFrom(type)));
            IDbCommand dbCommand =
                ((ISqlDataStore)XpoDefault.GetConnectionProvider(_connectionString, AutoCreateOption.DatabaseAndSchema)).CreateCommand();
            new XpoObjectMerger().MergeTypes(unitOfWork, persistentTypes.ToList(), dbCommand);
        }

        public IEnumerable<Type> GetAdditionalClasses()
        {
            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DefinedModules));
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider +=
                (sender, args) => _connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
        }

        string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args)
        {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}