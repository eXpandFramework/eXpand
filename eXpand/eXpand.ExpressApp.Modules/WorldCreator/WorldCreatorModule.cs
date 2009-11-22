using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
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
        List<Type> _definedModules;

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
            
            var moduleCreator = new ModuleCreator();
            _typesInfo = new TypesInfo(GetAdditionalClasses());
            var unitOfWork = new UnitOfWork { ConnectionString = _connectionString };
            
            Application.SetupComplete += (sender, args) => mergeTypes(unitOfWork);
            createDynamicTypes(moduleManager, unitOfWork, _typesInfo, moduleCreator);
            var existentTypesMemberCreator = new ExistentTypesMemberCreator();
            existentTypesMemberCreator.CreateMembers(unitOfWork, _typesInfo);
        }

        void mergeTypes(UnitOfWork unitOfWork) {
            IEnumerable<Type> persistentTypes =
                DefinedModules.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof (IXPSimpleObject).IsAssignableFrom(type)));
            IDbCommand dbCommand =
                ((ISqlDataStore) XpoDefault.GetConnectionProvider(_connectionString,AutoCreateOption.DatabaseAndSchema)).CreateCommand();
//            IDbCommand command =((ISqlDataStore) (((BaseDataLayer) (unitOfWork.DataLayer)).ConnectionProvider)).Connection.CreateCommand();
            new XpoObjectMerger().MergeTypes(unitOfWork, _typesInfo.PersistentTypesInfoType, persistentTypes.ToList(), dbCommand);
        }


        void createDynamicTypes(ApplicationModulesManager moduleManager, Session session, TypesInfo typesInfo, ModuleCreator moduleCreator)
        {
            var collection = new XPCollection(session, typesInfo.PersistentTypesInfoType).Cast<IPersistentClassInfo>().ToList();
            _definedModules = moduleCreator.DefineModule(AssemblyRouter.GetLists(collection));
            foreach (var moduleType in _definedModules){
                moduleManager.AddModule(moduleType, false);
            }
        }

        public IEnumerable<Type> GetAdditionalClasses()
        {
            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
        }
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            if (Application != null){
                ShowOwnerForExtendedMembers(model);
                removeDynamicAssemblyFromImageSources(model);
            }
        }
        private void ShowOwnerForExtendedMembers(Dictionary dictionary)
        {
            foreach (ListViewInfoNodeWrapper listViewInfoNodeWrapper in GetListViewInfoNodeWrappers(dictionary)){
                var columnInfoNodeWrapper = listViewInfoNodeWrapper.Columns.FindColumnInfo("Owner");
                if (columnInfoNodeWrapper != null) columnInfoNodeWrapper.VisibleIndex = 2;
            }
        }

        private IEnumerable<ListViewInfoNodeWrapper> GetListViewInfoNodeWrappers(Dictionary dictionary)
        {
            var wrapper = new ApplicationNodeWrapper(dictionary);
            var wrappers = wrapper.Views.GetListViews(TypesInfo.ExtendedReferenceMemberInfoType);
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.ExtendedCollectionMemberInfoType));
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.ExtendedCoreMemberInfoType));
            return wrappers;
        }

        private void removeDynamicAssemblyFromImageSources(Dictionary model)
        {
            var imageSourcesNode = model.RootNode.FindChildNode("ImageSources");
            foreach (var definedModule in DefinedModules)
            {
                var name = new AssemblyName(definedModule.Assembly.FullName + "").Name;
                var dictionaryElement = imageSourcesNode.FindChildElementByPath("AssemblyResourceImageSource[@AssemblyName='" + name + "']");
                imageSourcesNode.RemoveChildNode((DictionaryNode)dictionaryElement);
            }
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider += (sender, args) => _connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
        }
        private string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args)
        {
            return args.Connection !=null ? args.Connection.ConnectionString : args.ConnectionString;
        }

//        public override void Setup(XafApplication application)
//        {
//            base.Setup(application);
//            application.CreateCustomObjectSpaceProvider += (sender, args) => _connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
//        }

//        private string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
//            return args.Connection !=
//                   null ? args.Connection.ConnectionString : args.ConnectionString;
//        }

//        public override void Setup(ApplicationModulesManager moduleManager){
//            base.Setup(moduleManager);
//            
//            if (Application != null) {
//                _typesInfo = new TypesInfo(GetAdditionalClasses());
//                
//                Application.SetupComplete+=ApplicationOnSetupComplete;
//                var worldCreatorAsembly = AppDomain.CurrentDomain.GetAssemblies().Where(
//                    assembly => assembly.FullName != null && assembly.FullName=="WorldCreator").FirstOrDefault();
//                if (worldCreatorAsembly != null)
//                    _dynamicModuleType= worldCreatorAsembly.GetTypes().OfType<ModuleBase>().Single().GetType();
//                _unitOfWork = new UnitOfWork {ConnectionString = _connectionString};
//                
//                _typeCreator = new TypeCreator(_typesInfo, _unitOfWork);
//                if (_dynamicModuleType== null)
//                    _dynamicModuleType = _typeCreator.GetDynamicModule();
//                _typeCreator.CreateExtendedTypes();
//
//                if (_dynamicModuleType != null) {
//                    moduleManager.AddModule(_dynamicModuleType,false);
//                    var types = _dynamicModuleType.Assembly.GetTypes().Where(type => typeof (IXPSimpleObject).IsAssignableFrom(type));
//                    _unitOfWork.UpdateSchema(types.ToArray());
//                }
//            }
//        }
//        private void ApplicationOnSetupComplete(object sender, EventArgs args){
//            mergeTypes(_unitOfWork);
//        }

//        private IEnumerable<Type> GetAdditionalClasses() {
//            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
//        }

//        private void mergeTypes(UnitOfWork unitOfWork)
//        {
//            var collection = new XPCollection(unitOfWork, _typesInfo.PersistentTypesInfoType).Cast<IPersistentClassInfo>().Where(info => info.MergedObjectType !=
//                                                                                                                                         null).ToList();
//            foreach (IPersistentClassInfo classInfo in collection)
//            {
//
//                XPClassInfo xpClassInfo = getClassInfo(classInfo.Session, classInfo.AssemblyName + "." + classInfo.Name);
//                var mergedXPClassInfo = getClassInfo(classInfo.Session, classInfo.MergedObjectType.AssemblyQualifiedName) ?? classInfo.Session.GetClassInfo(classInfo.MergedObjectType);
//                if (unitOfWork.GetCount(xpClassInfo.ClassType) == 0)
//                    createObjectTypeColumn(xpClassInfo, _unitOfWork);
//                updateObjectType(unitOfWork, xpClassInfo, mergedXPClassInfo);
//
//            }
//        }
//        private void createObjectTypeColumn(XPClassInfo xpClassInfo,UnitOfWork unitOfWork) {
//            unitOfWork.CreateObjectTypeRecords(xpClassInfo);
//            var newObject = xpClassInfo.CreateNewObject(unitOfWork);
//            unitOfWork.CommitChanges();
//            unitOfWork.Delete(newObject);
//            unitOfWork.CommitChanges();
//        }

//        private XPClassInfo getClassInfo(Session session, string assemblyQualifiedName) {
//            Type[] types = _dynamicModuleType.Assembly.GetTypes();
//            Type classType = types.Where(type => type.FullName == assemblyQualifiedName).SingleOrDefault();
//            if (classType != null) {
//                return session.GetClassInfo(classType);
//            }
//            return null;
//        }

//        private void updateObjectType(UnitOfWork unitOfWork, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo) {
//            IDbCommand command = ((ISqlDataStore) (((BaseDataLayer)(unitOfWork.DataLayer)).ConnectionProvider)).Connection.CreateCommand();
//            var propertyName = XPObject.Fields.ObjectType.PropertyName;
//            command.CommandText = "UPDATE " + mergedXPClassInfo.TableName + " SET " + propertyName + "=" + unitOfWork.GetObjectType(xpClassInfo).Oid +
//                                  " WHERE " + propertyName + " IS NULL OR " + propertyName+"="+
//                                  unitOfWork.GetObjectType(mergedXPClassInfo).
//                                      Oid;
//            command.ExecuteNonQuery();
//        }

//        public override void UpdateModel(Dictionary model) {
//            base.UpdateModel(model);
//            if (Application!= null){
//                ShowOwnerForExtendedMembers(model);
//                removeDynamicAssemblyFromImageSources(model);
//            }
//        }



//        private void ShowOwnerForExtendedMembers(Dictionary dictionary) {
//            foreach (ListViewInfoNodeWrapper listViewInfoNodeWrapper in GetListViewInfoNodeWrappers(dictionary)){
//                var columnInfoNodeWrapper = listViewInfoNodeWrapper.Columns.FindColumnInfo("Owner");
//                if (columnInfoNodeWrapper != null) columnInfoNodeWrapper.VisibleIndex = 2;
//            }
//        }

//        private IEnumerable<ListViewInfoNodeWrapper> GetListViewInfoNodeWrappers(Dictionary dictionary) {
//            var wrapper = new ApplicationNodeWrapper(dictionary);
//            var wrappers = wrapper.Views.GetListViews(_typesInfo.ExtendedReferenceMemberInfoType);
//            wrappers.AddRange(wrapper.Views.GetListViews(_typesInfo.ExtendedCollectionMemberInfoType));
//            wrappers.AddRange(wrapper.Views.GetListViews(_typesInfo.ExtendedCoreMemberInfoType));
//            return wrappers;
//        }

//        private void removeDynamicAssemblyFromImageSources(Dictionary model) {
//            var imageSourcesNode = model.RootNode.FindChildNode("ImageSources");
//            if (_dynamicModuleType != null) {
//                var name = new AssemblyName(_dynamicModuleType.Assembly.FullName+"").Name;
//                var dictionaryElement = imageSourcesNode.FindChildElementByPath("AssemblyResourceImageSource[@AssemblyName='" +name+ "']");
//                imageSourcesNode.RemoveChildNode((DictionaryNode) dictionaryElement);
//            }
//        }
    }
}
