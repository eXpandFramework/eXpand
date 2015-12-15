using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase : XpandModuleBase {
        public const string WCAssembliesPath = "WCAssembliesPath";
        readonly List<Type> _dynamicModuleTypes = new List<Type>();

        public static event EventHandler<CollectTypesEventArgs> AddAdditionalReflectionDictionaryTypes;

        /// <summary>
        /// Add additional custom types to the ReflectionDictionary. 
        /// Required if you add your own PersistentXYZAttributes (similar to PersistentAggregatedAttribute, ...) implementations.
        /// </summary>
        /// <param name="e"></param>
        private static void OnAddAdditionalReflectionDictionaryTypes(CollectTypesEventArgs e) {
            EventHandler<CollectTypesEventArgs> handler = AddAdditionalReflectionDictionaryTypes;
            if (handler != null) {
                handler(null, e);
            }
        }

        public List<Type> DynamicModuleTypes {
            get { return _dynamicModuleTypes; }
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            WCTypesInfo.Instance.Register(GetAdditionalClasses(moduleManager));
            if (Application == null || GetPath() == null || (!RuntimeMode && string.IsNullOrEmpty(ConnectionString)))
                return;
            CheckObjectSpaceType();
            if (!string.IsNullOrEmpty(ConnectionString) || Application.ObjectSpaceProvider is DataServerObjectSpaceProvider) {
                using (var unitOfWork = GetUnitOfWork()) {
                    RunUpdaters(unitOfWork);
                    AddDynamicModules(moduleManager, unitOfWork);
                    if (unitOfWork.DataLayer != null) unitOfWork.DataLayer.Dispose();
                }
            }
            else {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(CompileEngine.XpandExtension));
                foreach (var assembly1 in assemblies) {
                    moduleManager.AddModule(assembly1.GetTypes().First(type => typeof(ModuleBase).IsAssignableFrom(type)));
                }
            }
            Application.LoggedOn += ApplicationOnLoggedOn;


        }

        private void CheckObjectSpaceType(){
            var objectSpaceProvider = Application.ObjectSpaceProvider as XPObjectSpaceProvider;
            if (objectSpaceProvider!=null&&objectSpaceProvider.GetType()==typeof(XPObjectSpaceProvider))
                throw new Exception("Override CreateDefaultObjectSpaceProvider method in "+Application.GetType() +@" and use the code found int the Demos\Modules\WorldCreatorTester solution");
        }

        public virtual UnitOfWork GetUnitOfWork() {
            var dataServerObjectSpaceProvider = Application.ObjectSpaceProvider as DataServerObjectSpaceProvider;
            if (dataServerObjectSpaceProvider != null) {
                return (UnitOfWork)((XPObjectSpace)dataServerObjectSpaceProvider.CreateObjectSpace()).Session;
            }
            var xpoMultiDataStoreProxy = new MultiDataStoreProxy(ConnectionString, GetReflectionDictionary(), AutoCreateOption.DatabaseAndSchema);
            return new UnitOfWork(new SimpleDataLayer(xpoMultiDataStoreProxy));
        }

        void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            var session = ((XPObjectSpace)((Application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false)))).Session;
            if (session.DataLayer != null) MergeTypes(new UnitOfWork(session.DataLayer));
        }

        void RunUpdaters(Session session){
            foreach (WorldCreatorUpdater worldCreatorUpdater in GetWorldCreatorUpdaters(session)) {
                worldCreatorUpdater.Update();
            }
        }

        IEnumerable<WorldCreatorUpdater> GetWorldCreatorUpdaters(Session session) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(WorldCreatorUpdater)).Descendants.Select(typeInfo => (WorldCreatorUpdater)typeInfo.Type.CreateInstance(session));
        }

        public static ReflectionDictionary GetReflectionDictionary(XpandModuleBase moduleBase) {
            var externalModelBusinessClassesList = moduleBase.GetAdditionalClasses(moduleBase.Application.Modules);
            Type persistentAssemblyInfoType = externalModelBusinessClassesList.FirstOrDefault(type1 => typeof(IPersistentAssemblyInfo).IsAssignableFrom(type1));
            if (persistentAssemblyInfoType == null)
                throw new ArgumentNullException("Add a business object that implements " +
                                                typeof(IPersistentAssemblyInfo).FullName + " at your AdditionalBusinessClasses (module.designer.cs)");
            var types = persistentAssemblyInfoType.Assembly.GetTypes().Where(type => (type.Namespace + "").StartsWith(persistentAssemblyInfoType.Namespace + ""));
            var reflectionDictionary = new ReflectionDictionary();

            var collectTypesEventArgs = new CollectTypesEventArgs(new List<Type>());
            OnAddAdditionalReflectionDictionaryTypes(collectTypesEventArgs);

            foreach (var type in types.Union(collectTypesEventArgs.Types)) {
                reflectionDictionary.QueryClassInfo(type);
            }
            return reflectionDictionary;
        }

        ReflectionDictionary GetReflectionDictionary() {
            return GetReflectionDictionary(this);
        }

        void AddDynamicModules(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork) {
            unitOfWork.LockingOption = LockingOption.None;
            Type assemblyInfoType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>();
            var persistentAssemblyInfos = new XPCollection(unitOfWork, assemblyInfoType).Cast<IPersistentAssemblyInfo>().ToArray().Where(IsValidAssemblyInfo(moduleManager)).ToArray();
            persistentAssemblyInfos.Where(info => !NeedsCompilation(info)).Each(LoadCompiledInfos);
            persistentAssemblyInfos.Where(NeedsCompilation).Each(Compile);
            unitOfWork.CommitChanges();
        }

        private bool NeedsCompilation(IPersistentAssemblyInfo info){
            return !info.NeedCompilation.HasValue || info.NeedCompilation.Value;
        }

        private void Compile(IPersistentAssemblyInfo obj){
            var item = new CompileEngine().CompileModules(new[] { obj }, GetPath()).FirstOrDefault();
            obj.NeedCompilation = false;
            if (item!=null){
                LoadCompiledInfos(obj);
            }
        }

        public void LoadCompiledInfos(IPersistentAssemblyInfo assemblyInfo){
            var assemblyFile = Path.Combine(GetPath(), assemblyInfo.Name + CompileEngine.XpandExtension);
            var assembly = Assembly.LoadFrom(assemblyFile);
            var moduleType = assembly.GetTypes().First(type => typeof(ModuleBase).IsAssignableFrom(type));
            if (ModuleManager.Modules.All(@base => @base.GetType() != moduleType)){
                _dynamicModuleTypes.Add(moduleType);
                ModuleManager.AddModule(Application, (ModuleBase)moduleType.CreateInstance());
            }
        }

        Func<IPersistentAssemblyInfo, bool> IsValidAssemblyInfo(ApplicationModulesManager moduleManager) {
            return info =>{
                var isLoaded = moduleManager.Modules.Any(@base => @base.Name == "Dynamic" + info.Name + "Module");
                return !info.DoNotCompile && !isLoaded;
            };
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors){
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), false)));
        }

        public abstract string GetPath();

        void MergeTypes(UnitOfWork unitOfWork) {
            var persistentTypes =
                _dynamicModuleTypes.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof(IXPSimpleObject).IsAssignableFrom(type)));
            var sqlDataStore = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema) as ISqlDataStore;
            if (sqlDataStore != null) {
                IDbCommand dbCommand = sqlDataStore.CreateCommand();
                new XpoObjectMerger().MergeTypes(unitOfWork, persistentTypes.ToList(), dbCommand);
                unitOfWork.CommitChanges();
            }
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DynamicModuleTypes));
        }


    }
}