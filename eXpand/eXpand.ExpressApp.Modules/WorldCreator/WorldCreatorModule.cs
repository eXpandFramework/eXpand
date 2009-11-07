using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator
{
    public sealed partial class WorldCreatorModule : ModuleBase
    {
        
        private Type _dynamicModuleType;
        private TypesInfo _typesInfo;
        private TypeCreator _typeCreator;
        private string _connectionString;
        private UnitOfWork _unitOfWork;

        public WorldCreatorModule(){
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            
            if (Application != null) {
                Application.SetupComplete+=ApplicationOnSetupComplete;
                var worldCreatorAsembly = AppDomain.CurrentDomain.GetAssemblies().Where(
                    assembly => assembly.FullName != null && assembly.FullName=="WorldCreator").FirstOrDefault();
                if (worldCreatorAsembly != null)
                    _dynamicModuleType= worldCreatorAsembly.GetTypes().OfType<ModuleBase>().Single().GetType();
                _unitOfWork = new UnitOfWork { ConnectionString = _connectionString };
                _typesInfo = new TypesInfo(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
                _typeCreator = new TypeCreator(_typesInfo, _unitOfWork);
                if (_dynamicModuleType== null)
                    _dynamicModuleType = _typeCreator.GetDynamicModule();
                _typeCreator.CreateExtendedTypes();
                if (_dynamicModuleType != null) moduleManager.AddModule(_dynamicModuleType,false);
            }
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs args) {
            mergeTypes(_unitOfWork);
        }

        private void mergeTypes(UnitOfWork unitOfWork) {
            var collection = new XPCollection(unitOfWork, _typesInfo.PersistentTypesInfoType).Cast<IPersistentClassInfo>().Where(info => info.MergedObjectType!=
                                                                                                                                         null).ToList();
            foreach (IPersistentClassInfo classInfo in collection) {

                XPClassInfo xpClassInfo = getClassInfo(classInfo.Session, classInfo.AssemblyName+"."+ classInfo.Name);
                var mergedXPClassInfo = getClassInfo(classInfo.Session, classInfo.MergedObjectType.AssemblyQualifiedName) ?? classInfo.Session.GetClassInfo(classInfo.MergedObjectType);
                if (unitOfWork.GetCount(xpClassInfo.ClassType) == 0)
                    createObjectTypeColumn(xpClassInfo,_unitOfWork);
                updateObjectType(unitOfWork, xpClassInfo,mergedXPClassInfo);

            }
        }

        private void createObjectTypeColumn(XPClassInfo xpClassInfo,UnitOfWork unitOfWork) {
            unitOfWork.CreateObjectTypeRecords(xpClassInfo);
            var newObject = xpClassInfo.CreateNewObject(unitOfWork);
            unitOfWork.CommitChanges();
            unitOfWork.Delete(newObject);
            unitOfWork.CommitChanges();
        }

        private XPClassInfo getClassInfo(Session session, string assemblyQualifiedName) {
            Type[] types = _dynamicModuleType.Assembly.GetTypes();
            Type classType = types.Where(type => type.FullName == assemblyQualifiedName).SingleOrDefault();
            if (classType != null) {
                return session.GetClassInfo(classType);
            }
            return null;
        }

        private void updateObjectType(UnitOfWork unitOfWork, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo) {
            var command = unitOfWork.Connection.CreateCommand();
            var propertyName = XPObject.Fields.ObjectType.PropertyName;
            command.CommandText = "UPDATE " + mergedXPClassInfo.TableName + " SET " + propertyName + "=" + unitOfWork.GetObjectType(xpClassInfo).Oid +
                                  " WHERE " + propertyName + " IS NULL OR " + propertyName+"="+
                                  unitOfWork.GetObjectType(mergedXPClassInfo).
                                      Oid;
            command.ExecuteNonQuery();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider += (sender, args) => _connectionString = args.ConnectionString;
        }
        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            if (_typesInfo!= null) {
                ShowOwnerForExtendedMembers(model);
                removeDynamicAssemblyFromImageSources(model);
            }
        }


        private void ShowOwnerForExtendedMembers(Dictionary dictionary) {
            foreach (ListViewInfoNodeWrapper listViewInfoNodeWrapper in GetListViewInfoNodeWrappers(dictionary)){
                var columnInfoNodeWrapper = listViewInfoNodeWrapper.Columns.FindColumnInfo("Owner");
                if (columnInfoNodeWrapper != null) columnInfoNodeWrapper.VisibleIndex = 2;
            }
        }

        private List<ListViewInfoNodeWrapper> GetListViewInfoNodeWrappers(Dictionary dictionary) {
            var wrapper = new ApplicationNodeWrapper(dictionary);
            var wrappers = wrapper.Views.GetListViews(_typesInfo.ExtendedReferenceMemberInfoType);
            wrappers.AddRange(wrapper.Views.GetListViews(_typesInfo.ExtendedCollectionMemberInfoType));
            wrappers.AddRange(wrapper.Views.GetListViews(_typesInfo.ExtendedCoreMemberInfoType));
            return wrappers;
        }

        private void removeDynamicAssemblyFromImageSources(Dictionary model) {
            var imageSourcesNode = model.RootNode.FindChildNode("ImageSources");
            if (_dynamicModuleType != null) {
                var name = new AssemblyName(_dynamicModuleType.Assembly.FullName+"").Name;
                var dictionaryElement = imageSourcesNode.FindChildElementByPath("AssemblyResourceImageSource[@AssemblyName='" +name+ "']");
                imageSourcesNode.RemoveChildNode((DictionaryNode) dictionaryElement);
            }
        }
    }
}
