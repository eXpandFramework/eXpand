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

        public WorldCreatorModule(){
            InitializeComponent();
        }
        
        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            _typesInfo = new TypesInfo(GetAdditionalClasses());
            if (Application != null) {
                var worldCreatorAsembly = AppDomain.CurrentDomain.GetAssemblies().Where(
                    assembly => assembly.FullName != null && assembly.FullName=="WorldCreator").FirstOrDefault();
                if (worldCreatorAsembly != null)
                    _dynamicModuleType= worldCreatorAsembly.GetTypes().OfType<ModuleBase>().Single().GetType();
                var unitOfWork = new UnitOfWork { ConnectionString = Application.ConnectionString };
                
                _typeCreator = new TypeCreator(_typesInfo, unitOfWork);
                if (_dynamicModuleType== null)
                    _dynamicModuleType = _typeCreator.GetDynamicModule();
                _typeCreator.CreateExtendedTypes();
                if (_dynamicModuleType != null) moduleManager.AddModule(_dynamicModuleType,false);
            }
        }

        private IEnumerable<Type> GetAdditionalClasses() {
            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
        }

        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            if (!DesignMode){
                
                disableServerModeForInterfaceInfoListViews(model);
                ShowOwnerForExtendedMembers(model);
                removeDynamicAssemblyFromImageSources(model);
            }
        }

        private void disableServerModeForInterfaceInfoListViews(Dictionary dictionary) {
            var wrapper = new ApplicationNodeWrapper(dictionary);
            foreach (var listView in wrapper.Views.GetListViews(_typesInfo.IntefaceInfoType)){
                listView.Node.SetAttribute("UseServerMode",false);
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
