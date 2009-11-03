using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using System.Linq;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator
{
    public sealed partial class WorldCreatorModule : ModuleBase
    {
        
        private Type _dynamicModuleType;
        private TypesInfo _typesInfo;
        private TypeCreator _typeCreator;
        private string _connectionString;

        public WorldCreatorModule(){
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null) {
                var worldCreatorAsembly = AppDomain.CurrentDomain.GetAssemblies().Where(
                    assembly => assembly.FullName != null && assembly.FullName=="WorldCreator").FirstOrDefault();
                if (worldCreatorAsembly != null)
                    _dynamicModuleType= worldCreatorAsembly.GetTypes().OfType<ModuleBase>().Single().GetType();
                var unitOfWork = new UnitOfWork { ConnectionString = _connectionString };
                _typesInfo = new TypesInfo(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
                _typeCreator = new TypeCreator(_typesInfo, unitOfWork);
                if (_dynamicModuleType== null)
                    _dynamicModuleType = _typeCreator.GetDynamicModule();
                _typeCreator.CreateExtendedTypes();
                if (_dynamicModuleType != null) moduleManager.AddModule(_dynamicModuleType,false);
            }
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
