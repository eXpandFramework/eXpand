using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraFilterEditor;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule {

     [ModelAbstractClass]
    public interface IModelPropertyEditorFilterControl:IModelPropertyEditor{
        [ModelBrowsable(typeof(CriteriaPropertyEditorVisibilityCalculator))]
        IModelFilterControl FilterControl { get; }
    }

    public class CriteriaPropertyEditorVisibilityCalculator:EditorTypeVisibilityCalculator<IModelPropertyEditor>{
        public override bool IsVisible(IModelNode node, string propertyName){
            var types = new[]{typeof(CriteriaPropertyEditor),typeof (PopupCriteriaPropertyEditor)};
            return types.Any(type => type.IsAssignableFrom(EditorType(node)));
        }
    }

    public interface IModelFilterControl : IModelModelAdapter {
        IModelFilterControlModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelFilterControlAdaptersNodeGenerator))]
    public interface IModelFilterControlModelAdapters : IModelList<IModelFilterControlModelAdapter>, IModelNode {

    }

    public class ModelFilterControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelFilterControl, IModelFilterControlModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelFilterControlModelAdapter : IModelCommonModelAdapter<IModelFilterControl> {
    }

    [DomainLogic(typeof(IModelFilterControlModelAdapter))]
    public class ModelGridViewModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelFilterControl> {
        public static IModelList<IModelFilterControl> Get_ModelAdapters(IModelFilterControlModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public class CriteriaPropertyEditorControlAdapterController : PropertyEditorControlAdapterController<IModelPropertyEditorFilterControl, IModelFilterControl> {
        protected override IModelFilterControl[] GetControlModelNodes(IModelPropertyEditorFilterControl modelPropertyEditorFilterControl){
            var modelFilterControls = modelPropertyEditorFilterControl.FilterControl.ModelAdapters.Select(adapter => adapter.ModelAdapter);
            return modelFilterControls.Concat(new[] { modelPropertyEditorFilterControl.FilterControl }).ToArray();
        }

        protected override Type GetControlType(){
            return typeof (FilterEditorControl);
        }
    }
}