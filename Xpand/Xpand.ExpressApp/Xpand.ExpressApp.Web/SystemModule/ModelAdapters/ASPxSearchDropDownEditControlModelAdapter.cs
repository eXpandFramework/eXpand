//using System;
//using System.Collections.Generic;
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
//    public interface IModelMemberViewItemASPxSearchDropDownEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ASPxSearchDropDownEditMemberViewItemVisibilityCalculator))]
//        IModelASPxSearchDropDownEditControl ASPxSearchDropDownEditControl { get; }
//    }
//
//    public class ASPxSearchDropDownEditMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxSearchLookupPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelASPxSearchDropDownEditControl : IModelModelAdapter {
//        IModelFindEditControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelASPxSearchDropDownEditControlAdaptersNodeGenerator))]
//    public interface IModelFindEditControlModelAdapters : IModelList<IModelASPxSearchDropDownEditControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelASPxSearchDropDownEditControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelASPxSearchDropDownEditControl, IModelASPxSearchDropDownEditControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelASPxSearchDropDownEditControlModelAdapter : IModelCommonModelAdapter<IModelASPxSearchDropDownEditControl> {
//    }
//
//    [DomainLogic(typeof(IModelASPxSearchDropDownEditControlModelAdapter))]
//    public class ASPxSearchDropDownEditControlModelAdapterLogic : ModelAdapterDomainLogicBase<IModelASPxSearchDropDownEditControl> {
//        public static IModelList<IModelASPxSearchDropDownEditControl> Get_ModelAdapters(IModelASPxSearchDropDownEditControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//
//    public class ASPxSearchDropDownEditControlModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemASPxSearchDropDownEdit,IModelASPxSearchDropDownEditControl,ASPxSearchLookupPropertyEditor>{
//        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData(){
//            yield return new InterfaceBuilderData(GetControlType()) {
//                Act = info =>(info.Name!=nameof(ASPxComboBox.SelectedIndex)&&info.DXFilter())
//            };
//        }
//
//        protected override Expression<Func<IModelMemberViewItemASPxSearchDropDownEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemASPxSearchDropDownEdit modelPropertyEditorFilterControl){
//            return edit => edit.ASPxSearchDropDownEditControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof(ASPxSearchDropDownEdit);
//        }
//    }
//}
