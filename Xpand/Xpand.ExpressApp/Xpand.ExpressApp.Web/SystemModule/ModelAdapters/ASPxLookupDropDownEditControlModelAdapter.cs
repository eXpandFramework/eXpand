//using System;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Web.Editors.ASPx;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Web.SystemModule.ModelAdapters {
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemASPxLookupDropDownEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxLookupDropDownEditMemberViewItemVisibilityCalculator))]
//        IModelASPxLookupDropDownEditControl ASPxLookupDropDownEditControl { get; }
//    }
//
//    public class ASPxLookupDropDownEditMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxLookupPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxLookupDropDownEditControl : IModelModelAdapter {
//        IModelDropDownControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxLookupDropDownEditControlAdaptersNodeGenerator))]
//    public interface IModelDropDownControlModelAdapters : IModelList<IModelASPxLookupDropDownEditControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxLookupDropDownEditControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxLookupDropDownEditControl, IModelASPxLookupDropDownEditControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxLookupDropDownEditControlModelAdapter : IModelCommonModelAdapter<IModelASPxLookupDropDownEditControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxLookupDropDownEditControlModelAdapter))]
//    public class ModelASPxLookupDropDownEditControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxLookupDropDownEditControl> {
//        public static IModelList<IModelASPxLookupDropDownEditControl> Get_ModelAdapters(IModelASPxLookupDropDownEditControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxLookupDropDownEditControlModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxLookupDropDownEdit,IModelASPxLookupDropDownEditControl,ASPxLookupPropertyEditor>{
//        protected override Expression<Func<IModelMemberViewItemASPxLookupDropDownEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxLookupDropDownEdit modelPropertyEditorFilterControl){
//            return edit => edit.ASPxLookupDropDownEditControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxLookupDropDownEdit);
//        }
//    }
//}
