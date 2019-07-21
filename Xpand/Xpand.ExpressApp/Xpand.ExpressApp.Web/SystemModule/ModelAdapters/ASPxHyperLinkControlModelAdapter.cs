//using System;
//using System.ComponentModel;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.Web;
//using Xpand.ExpressApp.Web.PropertyEditors;
//using Xpand.Persistent.Base.General.Model;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Web.SystemModule.ModelAdapters {
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemASPxHyperLink : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxHyperLinkMemberViewItemVisibilityCalculator))]
//        IModelASPxHyperLinkControl ASPxHyperLinkControl { get; }
//    }
//
//    public class ASPxHyperLinkMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<HyperLinkPropertyEditor, IModelMemberViewItem> {
//    }
//
    public interface IModelASPxHyperLinkControl {//: IModelModelAdapter {
//        [Category(AttributeCategoryNameProvider.Xpand)]
        string HyperLinkFormat { get; set; }
//        IModelASPxHyperLinkControlModelAdapters ModelAdapters { get; }
    }
//
//    [ModelNodesGenerator(typeof(ModelASPxHyperLinkControlAdaptersNodeGenerator))]
//    public interface IModelASPxHyperLinkControlModelAdapters : IModelList<IModelASPxHyperLinkControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxHyperLinkControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxHyperLinkControl, IModelASPxHyperLinkControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxHyperLinkControlModelAdapter : IModelCommonModelAdapter<IModelASPxHyperLinkControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxHyperLinkControlModelAdapter))]
//    public class ModelASPxHyperLinkControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxHyperLinkControl> {
//        public static IModelList<IModelASPxHyperLinkControl> Get_ModelAdapters(IModelASPxHyperLinkControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxHyperLinkControlModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxHyperLink,IModelASPxHyperLinkControl,HyperLinkPropertyEditor>{
//        protected override Expression<Func<IModelMemberViewItemASPxHyperLink, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxHyperLink modelPropertyEditorFilterControl){
//            return edit => edit.ASPxHyperLinkControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxHyperLink);
//        }
//    }
//}
