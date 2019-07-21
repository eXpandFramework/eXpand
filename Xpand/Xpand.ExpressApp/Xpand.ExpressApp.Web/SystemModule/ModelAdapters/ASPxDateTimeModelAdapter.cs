//using System;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Web.Editors.ASPx;
//using DevExpress.Persistent.Base.General;
//using DevExpress.Web;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Web.SystemModule.ModelAdapters {
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemASPxDateEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxDateEditMemberViewItemVisibilityCalculator))]
//        IModelASPxDateEditControl ASPxDateEditControl { get; }
//    }
//
//    public class ASPxDateEditMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxDateTimePropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxDateEditControl : IModelModelAdapter {
//        IModelASPxDateEditControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxDateEditControlAdaptersNodeGenerator))]
//    public interface IModelASPxDateEditControlModelAdapters : IModelList<IModelASPxDateEditControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxDateEditControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxDateEditControl, IModelASPxDateEditControlModelAdapter> {
//    }
//
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxDateEditControlModelAdapter : IModelCommonModelAdapter<IModelASPxDateEditControl> {
//
//    }
//
//    [DomainLogic(typeof(IModelASPxDateEditControlModelAdapter))]
//    public class ModelASPxDateEditControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxDateEditControl> {
//        public static IModelList<IModelASPxDateEditControl> Get_ModelAdapters(IModelASPxDateEditControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxDateEditModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxDateEdit,IModelASPxDateEditControl, ASPxDateTimePropertyEditor> {
//        protected override void OnActivated() {
//            base.OnActivated();
//            if (View.ObjectTypeInfo != null)
//                Active["ReportViewer_DetailView"] = typeof(IReportData).IsAssignableFrom(View.ObjectTypeInfo.Type);
//        }
//
//        protected override Expression<Func<IModelMemberViewItemASPxDateEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxDateEdit modelPropertyEditorFilterControl){
//            return edit => edit.ASPxDateEditControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxDateEdit);
//        }
//    }
//}
