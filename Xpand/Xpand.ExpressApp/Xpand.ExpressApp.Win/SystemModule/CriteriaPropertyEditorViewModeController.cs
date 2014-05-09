using System;
using System.Linq;
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

    public class CriteriaPropertyEditorVisibilityCalculator:EditorTypeVisibilityCalculator{
        public override bool IsVisible(IModelNode node, string propertyName){
            var types = new[]{typeof(CriteriaPropertyEditor),typeof (PopupCriteriaPropertyEditor)};
            return types.Any(type => type.IsAssignableFrom(EditorType(node)));
        }
    }

    public interface IModelFilterControl : IModelModelAdapter {
    }

    public class CriteriaPropertyEditorControlAdapterController : PropertyEditorControlAdapterController<IModelPropertyEditorFilterControl, IModelFilterControl> {
        protected override IModelFilterControl GetControlModelNode(IModelPropertyEditorFilterControl modelPropertyEditorLabelControl){
            return modelPropertyEditorLabelControl.FilterControl;
        }

        protected override Type GetControlType(){
            return typeof (FilterEditorControl);
        }
    }
}