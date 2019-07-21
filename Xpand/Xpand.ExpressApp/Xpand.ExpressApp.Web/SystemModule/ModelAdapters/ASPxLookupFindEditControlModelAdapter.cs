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
//    public interface IModelMemberViewItemASPxLookupFindEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxLookupFindEditMemberViewItemVisibilityCalculator))]
//        IModelASPxLookupFindEditControl ASPxLookupFindEditControl { get; }
//    }
//
//    public class ASPxLookupFindEditMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxLookupPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxLookupFindEditControl : IModelModelAdapter {
//        IModelFindControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxLookupFindEditControlAdaptersNodeGenerator))]
//    public interface IModelFindControlModelAdapters : IModelList<IModelASPxLookupFindEditControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxLookupFindEditControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxLookupFindEditControl, IModelASPxLookupFindEditControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxLookupFindEditControlModelAdapter : IModelCommonModelAdapter<IModelASPxLookupFindEditControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxLookupFindEditControlModelAdapter))]
//    public class ModelASPxLookupFindEditControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxLookupFindEditControl> {
//        public static IModelList<IModelASPxLookupFindEditControl> Get_ModelAdapters(IModelASPxLookupFindEditControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxLookupFindEditControlModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxLookupFindEdit,IModelASPxLookupFindEditControl,ASPxLookupPropertyEditor>{
//        protected override Expression<Func<IModelMemberViewItemASPxLookupFindEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxLookupFindEdit modelPropertyEditorFilterControl){
//            return edit => edit.ASPxLookupFindEditControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxLookupFindEdit);
//        }
//    }
//}
