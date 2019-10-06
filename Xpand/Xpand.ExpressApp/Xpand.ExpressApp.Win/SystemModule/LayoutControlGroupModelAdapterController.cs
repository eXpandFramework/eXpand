using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class LayoutControlGroupController:ViewController<DetailView> {
        protected override void OnActivated() {
            base.OnActivated();
            ((LayoutControl) View.LayoutManager.Container).GroupExpandChanged+=OnGroupExpandChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ((LayoutControl) View.LayoutManager.Container).GroupExpandChanged-=OnGroupExpandChanged;
        }

        private void OnGroupExpandChanged(object sender, LayoutGroupEventArgs e) {
            var layoutControlGroup = ((XafLayoutControlGroup) e.Group);
            var modelNode = layoutControlGroup.Model.GetNode(PredefinedMap.LayoutControlGroup);
            modelNode.SetValue<bool?>("Expanded",layoutControlGroup.Expanded);
        }
    }

//    public class LayoutControlGroupModelAdapterController:ModelAdapterController,IModelExtender {
//        private LayoutManager _layoutManager;
//
//        protected override void OnActivated() {
//            base.OnActivated();
//            var detailView = View as DetailView;
//            if (detailView != null){
//                _layoutManager = detailView.LayoutManager;
//                ((LayoutControl) _layoutManager.Container).GroupExpandChanged+=OnGroupExpandChanged;
//                ((ISupportAppearanceCustomization)_layoutManager).CustomizeAppearance += layoutManager_CustomizeAppearance;
//            }
//        }
//
//        protected override void OnDeactivated(){
//            if (_layoutManager != null){
//                ((ISupportAppearanceCustomization)_layoutManager).CustomizeAppearance -= layoutManager_CustomizeAppearance;
//                if (_layoutManager.Container != null)
//                    ((LayoutControl) _layoutManager.Container).GroupExpandChanged-=OnGroupExpandChanged;
//            }
//            base.OnDeactivated();
//        }
//
//        private void OnGroupExpandChanged(object sender, LayoutGroupEventArgs layoutGroupEventArgs){
//            var layoutControlGroup = ((XafLayoutControlGroup) layoutGroupEventArgs.Group);
//            ((IModelLayoutGroupLayoutControlGroup) layoutControlGroup.Model).GroupExpanded = layoutControlGroup.Expanded;
//        }
//
//        private void layoutManager_CustomizeAppearance(object sender, CustomizeAppearanceEventArgs e) {
//            var layoutControlGroup = ((WinLayoutItemAppearanceAdapter)e.Item).Item as XafLayoutControlGroup;
//            if (layoutControlGroup != null && layoutControlGroup.Model.ShowCaption.HasValue && layoutControlGroup.Model.ShowCaption.Value) {
//                ApplyModel(layoutControlGroup);
//            }
//        }
//
//        private void ApplyModel(XafLayoutControlGroup layoutControlGroup){
//            new ObjectModelSynchronizer(layoutControlGroup, layoutControlGroup.Model).ApplyModel();
//            var groupExpanded = ((IModelLayoutGroupLayoutControlGroup) layoutControlGroup.Model).GroupExpanded;
//            if (groupExpanded.HasValue)
//                layoutControlGroup.Expanded = groupExpanded.Value;
//        }
//
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            extenders.Add<IModelLayoutGroup, IModelLayoutGroupLayoutControlGroup>();
//            var builder = new InterfaceBuilder(extenders);
//            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(LayoutControlGroup).Name));
//
//            builder.ExtendInteface<IModelLayoutControlGroup, LayoutControlGroup>(assembly);
//
//        }
//        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            yield return new InterfaceBuilderData(typeof(LayoutControlGroup)) {
//                Act = info => (info.DXFilter())
//            };
//        }
//
//    }

//    [ModelAbstractClass]
//    public interface IModelLayoutGroupLayoutControlGroup:IModelLayoutGroup, IModelModelAdapterLink{
//        [Category("eXpand.ModelAdapters")]
//        bool? GroupExpanded { get; set; }
//    }

//    [DomainLogic(typeof(IModelLayoutGroupLayoutControlGroup))]
//    public class ModelLayoutGroupLayoutControlGroupDomainLogic {
//        public static IModelModelAdapter Get_ModelAdapter(IModelLayoutGroupLayoutControlGroup controlGroup){
//            return controlGroup.ModelAdapterContext[typeof (IModelLayoutControlGroup).Name.Replace("IModel", "")] ;
//        }
//    }

//    public interface IModelLayoutControlGroup : IModelModelAdapter, IModelViewLayoutElement {
//        IModelLayoutControlGroupModelAdapters ModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelLayoutControlGroupAdaptersNodeGenerator))]
//    public interface IModelLayoutControlGroupModelAdapters : IModelList<IModelLayoutControlGroupModelAdapter>, IModelViewLayoutElement {
//
//    }

//    public class ModelLayoutControlGroupAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelLayoutControlGroup, IModelLayoutControlGroupModelAdapter> {
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelLayoutControlGroupModelAdapter : IModelCommonModelAdapter<IModelLayoutControlGroup> {
//    }

//    [DomainLogic(typeof(IModelLayoutControlGroupModelAdapter))]
//    public class ModelLayoutControlGroupModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelLayoutControlGroup> {
//        public static IModelList<IModelLayoutControlGroup> Get_ModelAdapters(IModelLayoutControlGroupModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
}