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
            
            Application.SettingUp+=ApplicationOnSettingUp;

            WCTypesInfo.Instance.Register(GetAdditionalClasses());
            RunUpdaters();

            var bussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
            using (SimpleDataLayer simpleDataLayer 
                = XpoMultiDataStoreProxy.GetDataLayer(_connectionString, GetReflectionDictionary(), bussinessObjectType)) {
                using (var session = new UnitOfWork(simpleDataLayer)) {
                    AddDynamicModules(moduleManager, session);
                }
            }
            Application.SetupComplete +=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var session = Application.ObjectSpaceProvider.CreateUpdatingSession();
            mergeTypes(new UnitOfWork(session.DataLayer));
            
        }
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            var existentTypesMemberCreator = new ExistentTypesMemberCreator();
            if (_connectionString != null) {
                var xpoMultiDataStoreProxy = new XpoMultiDataStoreProxy(_connectionString, GetReflectionDictionary());
                var simpleDataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy);
                var session = new Session(simpleDataLayer);
                existentTypesMemberCreator.CreateMembers(session);
            }
        }

        void RunUpdaters() {
            if (_connectionString != null) {
                var xpoMultiDataStoreProxy = new XpoMultiDataStoreProxy(_connectionString,GetReflectionDictionary());
                using (var dataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy)) {
                    using (var session = new Session(dataLayer)) {
                        foreach (var worldCreatorUpdater in GetWorldCreatorUpdaters(session)){
                            worldCreatorUpdater.Update();
                        }
                    }
                }
            }
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof (WorldCreatorUpdater)).Descendants.Select(
                typeInfo => (WorldCreatorUpdater) ReflectionHelper.CreateObject(typeInfo.Type, session));
        }

        ReflectionDictionary GetReflectionDictionary() {
            Type persistentAssemblyInfoType = GetAdditionalClasses().Where(type1 => typeof(IPersistentAssemblyInfo).IsAssignableFrom(type1)).FirstOrDefault();
            if (persistentAssemblyInfoType == null)
                throw new ArgumentNullException("No bussincess object that implements " +
                                                typeof (IPersistentAssemblyInfo).FullName + " found");
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
            Type assemblyInfoType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
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