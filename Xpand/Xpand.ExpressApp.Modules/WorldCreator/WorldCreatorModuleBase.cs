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
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.NodeUpdaters;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase:XpandModuleBase {
        string _connectionString;
        List<Type> _dynamicModuleTypes=new List<Type>();

        public List<Type> DynamicModuleTypes{
            get { return _dynamicModuleTypes; }
        }

        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            if (Application == null||GetPath()== null)
                return;
            Application.SettingUp+=ApplicationOnSettingUp;
            var businessClassesList = GetAdditionalClasses(moduleManager);
            WCTypesInfo.Instance.Register(businessClassesList);
            if (_connectionString != null) {
                var xpoMultiDataStoreProxy = new SqlMultiDataStoreProxy(_connectionString, GetReflectionDictionary());
                using (var dataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy)) {
                    using (var session = new Session(dataLayer)) {
                        RunUpdaters(session);
                        var unitOfWork = new UnitOfWork(session.DataLayer);
                        AddDynamicModules(moduleManager, unitOfWork);
                        SynchronizeTypes(unitOfWork);
                    }
                }
            }
            else {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(CompileEngine.XpandExtension));
                foreach (var assembly1 in assemblies) {
                    moduleManager.AddModule(assembly1.GetTypes().Where(type => typeof (ModuleBase).IsAssignableFrom(type)).Single());
                }
            }


            Application.SetupComplete +=ApplicationOnSetupComplete;
            
        }


        void RunUpdaters(Session session) {
            foreach (WorldCreatorUpdater worldCreatorUpdater in GetWorldCreatorUpdaters(session)) {
                worldCreatorUpdater.Update();
            }             
        }

        void SynchronizeTypes(UnitOfWork unitOfWork) {
            var xpObjectTypes = new XPCollection<XPObjectType>(unitOfWork);
            var moduleTypes = DynamicModuleTypes.Where(type => type.Assembly.GetCustomAttributes(typeof(Attribute), false).OfType<DataStoreAttribute>().SingleOrDefault() != null);
            var dataStoreManager = new SqlMultiDataStoreProxy(_connectionString).DataStoreManager;
            foreach (var moduleType in moduleTypes){
                var moduleBase = (ModuleBase)Activator.CreateInstance(moduleType);
                var businessClass = moduleBase.BusinessClasses[0];
                var connectionProvider = dataStoreManager.GetConnectionProvider(businessClass);
                var session = new Session(new SimpleDataLayer(connectionProvider));
                if (session.GetCount(typeof(XPObjectType))<xpObjectTypes.Count) {
                    session.Delete(new XPCollection<XPObjectType>(session));
                    foreach (var objectType in xpObjectTypes) {
                        session.Save(new XPObjectType(session, objectType.AssemblyName, objectType.TypeName));
                    }
                }
            }            
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
                var xpoMultiDataStoreProxy = new SqlMultiDataStoreProxy(_connectionString, GetReflectionDictionary());
                var simpleDataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy);
                var session = new Session(simpleDataLayer);
                existentTypesMemberCreator.CreateMembers(session);
            }
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof (WorldCreatorUpdater)).Descendants.Select(
                typeInfo => (WorldCreatorUpdater) ReflectionHelper.CreateObject(typeInfo.Type, session));
        }

        ReflectionDictionary GetReflectionDictionary() {
            Type persistentAssemblyInfoType = GetAdditionalClasses(Application.Modules).Where(type1 => typeof(IPersistentAssemblyInfo).IsAssignableFrom(type1)).FirstOrDefault();
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
            var objectSpaceProvider = setupEventArgs.SetupParameters.ObjectSpaceProvider as IXpandObjectSpaceProvider;
            if (objectSpaceProvider== null)
                throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IXpandObjectSpaceProvider).FullName);
        }

        void AddDynamicModules(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork){
            unitOfWork.LockingOption = LockingOption.None;
            Type assemblyInfoType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
            List<IPersistentAssemblyInfo> persistentAssemblyInfos =
                new XPCollection(unitOfWork, assemblyInfoType).Cast<IPersistentAssemblyInfo>().Where(info => !info.DoNotCompile &&
                    moduleManager.Modules.Where(@base => @base.Name == "Dynamic" + info.Name + "Module").FirstOrDefault() ==null).ToList();
            _dynamicModuleTypes = new CompileEngine().CompileModules(persistentAssemblyInfos,GetPath());
            foreach (var definedModule in _dynamicModuleTypes){
                moduleManager.AddModule(definedModule);
            }
            unitOfWork.CommitChanges();
        }

        public abstract string GetPath();

        void mergeTypes(UnitOfWork unitOfWork){
            IEnumerable<Type> persistentTypes =
                _dynamicModuleTypes.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof(IXPSimpleObject).IsAssignableFrom(type)));
            IDbCommand dbCommand =
                ((ISqlDataStore)XpoDefault.GetConnectionProvider(_connectionString, AutoCreateOption.DatabaseAndSchema)).CreateCommand();
            new XpoObjectMerger().MergeTypes(unitOfWork, persistentTypes.ToList(), dbCommand);
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DynamicModuleTypes));
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