using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator
{
    public sealed partial class WorldCreatorModule : ModuleBase
    {
        
//        private Type _dynamicModuleType;
//        private ITypesInfo _typesInfo;
//        private TypeCreator _typeCreator;
//        private UnitOfWork _unitOfWork;
//        private string _connectionString;

        public WorldCreatorModule(){
            InitializeComponent();
        }
        string _connectionString;
        TypesInfo _typesInfo;
        readonly List<Type> _definedModules=new List<Type>();

        public List<Type> DefinedModules
        {
            get { return _definedModules; }
        }

        public TypesInfo TypesInfo
        {
            get { return _typesInfo; }
        }

        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            _typesInfo = new TypesInfo(GetAdditionalClasses());
            var unitOfWork = new UnitOfWork { ConnectionString = _connectionString };
            Application.SetupComplete += (sender, args) => mergeTypes(unitOfWork);
            CreateDynamicTypes(moduleManager, unitOfWork, _typesInfo.PersistentAssemblyInfoType);
            var existentTypesMemberCreator = new ExistentTypesMemberCreator();
            existentTypesMemberCreator.CreateMembers(unitOfWork, _typesInfo);
        }

        void mergeTypes(UnitOfWork unitOfWork) {
            IEnumerable<Type> persistentTypes =
                DefinedModules.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof (IXPSimpleObject).IsAssignableFrom(type)));
            IDbCommand dbCommand =
                ((ISqlDataStore) XpoDefault.GetConnectionProvider(_connectionString,AutoCreateOption.DatabaseAndSchema)).CreateCommand();
            new XpoObjectMerger().MergeTypes(unitOfWork, _typesInfo.PersistentTypesInfoType, persistentTypes.ToList(), dbCommand);
        }


        public void CreateDynamicTypes(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork, Type persistentAssemblyInfoType) {
            
            List<IPersistentAssemblyInfo> collection =
                new XPCollection(unitOfWork, persistentAssemblyInfoType).Cast<IPersistentAssemblyInfo>().Where(info => !info.DoNotCompile).ToList();
            foreach (IPersistentAssemblyInfo persistentAssemblyInfo in collection) {
                persistentAssemblyInfo.CompileErrors = null;
                Type compileModule = CompileEngine.CompileModule(persistentAssemblyInfo);
                if (compileModule != null) {
                    _definedModules.Add(compileModule);
                    moduleManager.AddModule(compileModule, false);
                }
            }
            unitOfWork.CommitChanges();
        }

        public IEnumerable<Type> GetAdditionalClasses() {
            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
        }

        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            if (Application != null) {
                ShowOwnerForExtendedMembers(model);
                removeDynamicAssemblyFromImageSources(model);
                hideTemplateInfoNameFromTypeInfoDetailView(model);
            }
        }

        void hideTemplateInfoNameFromTypeInfoDetailView(Dictionary model) {
            throw new NotImplementedException();
        }

        void ShowOwnerForExtendedMembers(Dictionary dictionary) {
            foreach (ListViewInfoNodeWrapper listViewInfoNodeWrapper in GetListViewInfoNodeWrappers(dictionary)) {
                ColumnInfoNodeWrapper columnInfoNodeWrapper = listViewInfoNodeWrapper.Columns.FindColumnInfo("Owner");
                if (columnInfoNodeWrapper != null) columnInfoNodeWrapper.VisibleIndex = 2;
            }
        }

        IEnumerable<ListViewInfoNodeWrapper> GetListViewInfoNodeWrappers(Dictionary dictionary) {
            var wrapper = new ApplicationNodeWrapper(dictionary);
            List<ListViewInfoNodeWrapper> wrappers =
                wrapper.Views.GetListViews(TypesInfo.ExtendedReferenceMemberInfoType);
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.ExtendedCollectionMemberInfoType));
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.ExtendedCoreMemberInfoType));
            return wrappers;
        }

        void removeDynamicAssemblyFromImageSources(Dictionary model) {
            DictionaryNode imageSourcesNode = model.RootNode.FindChildNode("ImageSources");
            foreach (Type definedModule in DefinedModules) {
                string name = new AssemblyName(definedModule.Assembly.FullName + "").Name;
                DictionaryElement dictionaryElement =
                    imageSourcesNode.FindChildElementByPath("AssemblyResourceImageSource[@AssemblyName='" + name + "']");
                imageSourcesNode.RemoveChildNode((DictionaryNode) dictionaryElement);
            }
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider +=
                (sender, args) => _connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
        }

        string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}
