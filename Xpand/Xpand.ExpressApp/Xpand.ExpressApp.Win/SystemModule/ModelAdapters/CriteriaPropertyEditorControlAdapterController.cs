using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraFilterEditor;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule.ModelAdapters {

//     [ModelAbstractClass]
//    public interface IModelMemberViewItemFilterControl: IModelMemberViewItem {
//        [ModelBrowsable(typeof(CriteriaMemberViewItemVisibilityCalculator))]
//        IModelFilterControl FilterControl { get; }
//    }

    public class CriteriaMemberViewItemVisibilityCalculator:EditorTypeVisibilityCalculator<IModelMemberViewItem>{
        public override bool IsVisible(IModelNode node, string propertyName){
            var types = new[]{typeof(CriteriaPropertyEditor),typeof (PopupCriteriaPropertyEditor)};
            return types.Any(type => type.IsAssignableFrom(EditorType(node)));
        }
    }

//    public interface IModelFilterControl : IModelModelAdapter {
//        IModelFilterControlModelAdapters ModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelFilterControlAdaptersNodeGenerator))]
//    public interface IModelFilterControlModelAdapters : IModelList<IModelFilterControlModelAdapter>, IModelNode {
//
//    }

//    public class ModelFilterControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelFilterControl, IModelFilterControlModelAdapter> {
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelFilterControlModelAdapter : IModelCommonModelAdapter<IModelFilterControl> {
//    }

//    [DomainLogic(typeof(IModelFilterControlModelAdapter))]
//    public class ModelGridViewModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelFilterControl> {
//        public static IModelList<IModelFilterControl> Get_ModelAdapters(IModelFilterControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }

//    public class CriteriaPropertyEditorControlAdapterController : PropertyEditorControlAdapterController<IModelMemberViewItemFilterControl, IModelFilterControl,CriteriaPropertyEditor> {
//
//        protected override Expression<Func<IModelMemberViewItemFilterControl, IModelModelAdapter>> GetControlModel(IModelMemberViewItemFilterControl modelMemberViewItemFilterControl){
//            return control => control.FilterControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof (FilterEditorControl);
//        }
//    }
}