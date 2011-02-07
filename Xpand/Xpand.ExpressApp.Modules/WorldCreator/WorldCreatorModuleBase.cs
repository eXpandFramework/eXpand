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


namespace Xpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase : XpandModuleBase {
        
        List<Type> _dynamicModuleTypes = new List<Type>();
        string _connectionString;


        public List<Type> DynamicModuleTypes {
            get { return _dynamicModuleTypes; }
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var businessClassesList = GetAdditionalClasses(moduleManager);
            WCTypesInfo.Instance.Register(businessClassesList);
            if (Application == null || GetPath() == null)
                return;
            Application.SettingUp += ApplicationOnSettingUp;
            _connectionString = ((ISupportFullConnectionString) Application).ConnectionString;
            if (_connectionString != null) {
                var xpoMultiDataStoreProxy = new SqlMultiDataStoreProxy(_connectionString, GetReflectionDictionary());
                using (var dataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy)) {
                    using (var session = new Session(dataLayer)) {
                        var unitOfWork = new UnitOfWork(session.DataLayer);
                        var typeSynchronizer = new TypeSynchronizer();
                        typeSynchronizer.SynchronizeTypes(_connectionString);
                        RunUpdaters(session);
                        AddDynamicModules(moduleManager, unitOfWork);

                        //                        SynchronizeTypes(unitOfWork);
                    }
                }
            } else {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(CompileEngine.XpandExtension));
                foreach (var assembly1 in assemblies) {
                    moduleManager.AddModule(assembly1.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single());
                }
            }


            Application.SetupComplete += ApplicationOnSetupComplete;

        }


        void RunUpdaters(Session session) {
            foreach (WorldCreatorUpdater worldCreatorUpdater in GetWorldCreatorUpdaters(session)) {
                worldCreatorUpdater.Update();
            }
        }

//        void SynchronizeTypes(UnitOfWork unitOfWork) {
//            var xpObjectTypes = new XPCollection<XPObjectType>(unitOfWork);
//            var dataStoreManager = new SqlMultiDataStoreProxy(ConnectionString);
//            foreach (var xpObjectType in xpObjectTypes) {
//                var type = ReflectionHelper.FindType(xpObjectType.TypeName);
//                if (type != null) {
//                    var connectionString = dataStoreManager.DataStoreManager.GetConnectionString(type);
//                    var sqlDataStoreProxy = new SqlDataStoreProxy(connectionString);
//                    var xpoObjectHacker = new XpoObjectHacker();
//                    XPObjectType type1 = xpObjectType;
//                    var simpleDataLayer = new SimpleDataLayer(sqlDataStoreProxy);
//                    var session = new Session(simpleDataLayer);
//                    bool sync = false;
//                    sqlDataStoreProxy.DataStoreModifyData += (sender, args) => {
//                        var insertStatement = args.ModificationStatements.OfType<InsertStatement>().Where(statement => statement.TableName == typeof(XPObjectType).Name).SingleOrDefault();
//                        if (insertStatement != null && !sync) {
//                            sync = true;
//                            xpoObjectHacker.CreateObjectTypeIndetifier(insertStatement, simpleDataLayer, type1.Oid);
//                            ModificationResult modificationResult = sqlDataStoreProxy.ModifyData(insertStatement);
//                            args.ModificationResult = modificationResult;
//                            args.ModificationResult.Identities = new[] { new ParameterValue { Value = type1.Oid }, };
//                        }
//                    };
//
//                    if (session.FindObject<XPObjectType>(objectType => objectType.TypeName == type1.TypeName) == null) {
//                        var objectType = new XPObjectType(session, xpObjectType.AssemblyName, xpObjectType.TypeName);
//                        session.Save(objectType);
//
//                    }
//                }
//            }
//        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var session = (((ObjectSpace)Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false))).Session;
            mergeTypes(new UnitOfWork(session.DataLayer));

        }
        protected override BusinessClassesList GetBusinessClassesCore() {
            var existentTypesMemberCreator = new ExistentTypesMemberCreator();
            if (_connectionString != null) {
                var xpoMultiDataStoreProxy = new SqlMultiDataStoreProxy(_connectionString, GetReflectionDictionary());
                var simpleDataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy);
                var session = new Session(simpleDataLayer);
                existentTypesMemberCreator.CreateMembers(session);
            }
            return base.GetBusinessClassesCore();
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(WorldCreatorUpdater)).Descendants.Select(
                typeInfo => (WorldCreatorUpdater)ReflectionHelper.CreateObject(typeInfo.Type, session));
        }

        ReflectionDictionary GetReflectionDictionary() {
            Type persistentAssemblyInfoType = GetAdditionalClasses(BaseApplication.Modules).Where(type1 => typeof(IPersistentAssemblyInfo).IsAssignableFrom(type1)).FirstOrDefault();
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
                throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IXpandObjectSpaceProvider).FullName);
        }

        void AddDynamicModules(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork) {
            unitOfWork.LockingOption = LockingOption.None;
            Type assemblyInfoType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
            List<IPersistentAssemblyInfo> persistentAssemblyInfos =
                new XPCollection(unitOfWork, assemblyInfoType).Cast<IPersistentAssemblyInfo>().Where(info => !info.DoNotCompile &&
                    moduleManager.Modules.Where(@base => @base.Name == "Dynamic" + info.Name + "Module").FirstOrDefault() == null).ToList();
            _dynamicModuleTypes = new CompileEngine().CompileModules(persistentAssemblyInfos, GetPath());
            foreach (var definedModule in _dynamicModuleTypes) {
                moduleManager.AddModule(definedModule);
            }
            unitOfWork.CommitChanges();
        }

        public abstract string GetPath();

        void mergeTypes(UnitOfWork unitOfWork) {
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


    }
}