//using System;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Web.Editors.ASPx;
//using DevExpress.Web;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Web.SystemModule.ModelAdapters {
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemASPxSpinEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxSpinEditMemberViewItemVisibilityCalculator))]
//        IModelASPxSpinEditControl ASPxSpinEditControl { get; }
//    }
//
//    public class ASPxSpinEditMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxDecimalPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxSpinEditControl : IModelModelAdapter {
//        IModelASPxSpinEditControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxSpinEditControlAdaptersNodeGenerator))]
//    public interface IModelASPxSpinEditControlModelAdapters : IModelList<IModelASPxSpinEditControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxSpinEditControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxSpinEditControl, IModelASPxSpinEditControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxSpinEditControlModelAdapter : IModelCommonModelAdapter<IModelASPxSpinEditControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxSpinEditControlModelAdapter))]
//    public class ModelASPxSpinEditControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxSpinEditControl> {
//        public static IModelList<IModelASPxSpinEditControl> Get_ModelAdapters(IModelASPxSpinEditControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxSpinEditModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxSpinEdit,IModelASPxSpinEditControl, ASPxDecimalPropertyEditor> {
//        protected override Expression<Func<IModelMemberViewItemASPxSpinEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxSpinEdit modelPropertyEditorFilterControl){
//            return edit => edit.ASPxSpinEditControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxSpinEdit);
//        }
//    }
//}
