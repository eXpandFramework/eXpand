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
//    public interface IModelMemberViewItemASPxComboBox : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxComboBoxMemberViewItemVisibilityCalculator))]
//        IModelASPxComboBoxControl ASPxComboBoxControl { get; }
//    }
//
//    public class ASPxComboBoxMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<StringLookupPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxComboBoxControl : IModelModelAdapter {
//        IModelASPxComboBoxModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxComboBoxControlAdaptersNodeGenerator))]
//    public interface IModelASPxComboBoxModelAdapters : IModelList<IModelASPxComboBoxControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxComboBoxControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxComboBoxControl, IModelASPxComboBoxControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxComboBoxControlModelAdapter : IModelCommonModelAdapter<IModelASPxComboBoxControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxComboBoxControlModelAdapter))]
//    public class ModelASPxComboBoxControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelASPxComboBoxControl> {
//        public static IModelList<IModelASPxComboBoxControl> Get_ModelAdapters(IModelASPxComboBoxControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class StringLookupPropertyEditorModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxComboBox,IModelASPxComboBoxControl,StringLookupPropertyEditor>{
//        protected override Expression<Func<IModelMemberViewItemASPxComboBox, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxComboBox modelPropertyEditorFilterControl){
//            return edit => edit.ASPxComboBoxControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxComboBox);
//        }
//    }
//}
