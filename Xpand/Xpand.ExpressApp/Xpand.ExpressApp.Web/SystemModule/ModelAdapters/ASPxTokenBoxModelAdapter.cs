//using System;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.Web;
//using Xpand.ExpressApp.Web.PropertyEditors;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Web.SystemModule.ModelAdapters {
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemASPxTokenBox : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxTokenBoxMemberViewItemVisibilityCalculator))]
//        IModelASPxTokenBoxControl ASPxTokenBoxControl { get; }
//    }
//
//    public class ASPxTokenBoxMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxTokenListPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxTokenBoxControl : IModelModelAdapter {
//        IModelASPxTokenBoxControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxTokenBoxControlAdaptersNodeGenerator))]
//    public interface IModelASPxTokenBoxControlModelAdapters : IModelList<IModelASPxTokenBoxControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxTokenBoxControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxTokenBoxControl, IModelASPxTokenBoxControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxTokenBoxControlModelAdapter : IModelCommonModelAdapter<IModelASPxTokenBoxControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxTokenBoxControlModelAdapter))]
//    public class ModelASPxTokenBoxControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxTokenBoxControl> {
//        public static IModelList<IModelASPxTokenBoxControl> Get_ModelAdapters(IModelASPxTokenBoxControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxTokenBoxControlModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxTokenBox,IModelASPxTokenBoxControl,ASPxTokenListPropertyEditor>{
//        protected override Expression<Func<IModelMemberViewItemASPxTokenBox, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxTokenBox modelPropertyEditorFilterControl){
//            return edit => edit.ASPxTokenBoxControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxTokenBox);
//        }
//    }
//}
