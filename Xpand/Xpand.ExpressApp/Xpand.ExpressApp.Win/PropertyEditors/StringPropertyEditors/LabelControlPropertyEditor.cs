using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [ModelAbstractClass]
    public interface IModelPropertyEditorLabelControl:IModelPropertyEditor{
        [ModelBrowsable(typeof(LabelPropertyEditorVisibilityCalculator))]
        IModelLabelControl LabelControl { get; }
    }

    public class LabelPropertyEditorVisibilityCalculator:EditorTypeVisibilityCalculator<LabelControlPropertyEditor,IModelPropertyEditor>{
    }

    public interface IModelLabelControl : IModelModelAdapter {
        IModelLabelControlModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelLabelControlAdaptersNodeGenerator))]
    public interface IModelLabelControlModelAdapters : IModelList<IModelLabelControlModelAdapter>, IModelNode {

    }

    public class ModelLabelControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelLabelControl, IModelLabelControlModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelLabelControlModelAdapter : IModelCommonModelAdapter<IModelLabelControl> {
    }

    [DomainLogic(typeof(IModelLabelControlModelAdapter))]
    public class ModelLabelControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelLabelControl> {
        public static IModelList<IModelLabelControl> Get_ModelAdapters(IModelLabelControlModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public class LabelControlModelAdapterController : PropertyEditorControlAdapterController<IModelPropertyEditorLabelControl,IModelLabelControl> {
        protected override Type GetControlType(){
            return typeof (LabelControl);
        }

        protected override void ExtendingModelInterfaces(InterfaceBuilder builder, Assembly assembly, ModelInterfaceExtenders extenders){
            var calcType = builder.CalcType(typeof(LabelControlAppearanceObject), assembly);
            extenders.Add(calcType, typeof(IModelAppearanceFont));
        }

        protected override IModelLabelControl[] GetControlModelNodes(IModelPropertyEditorLabelControl modelPropertyEditorFilterControl){
            var modelLabelControls = modelPropertyEditorFilterControl.LabelControl.ModelAdapters.Select(adapter => adapter.ModelAdapter);
            return modelLabelControls.Concat(new[] { modelPropertyEditorFilterControl.LabelControl }).ToArray();
        }
    }

    [PropertyEditor(typeof(string),false)]
    public class LabelControlPropertyEditor : WinPropertyEditor, IModelPropertyEditorControlAdapter {
        public LabelControlPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
            ControlBindingProperty = "Text";
        }

        protected override object CreateControlCore(){
            var labelControl = new LabelControl{
                BorderStyle = BorderStyles.NoBorder,
                AutoSizeMode = LabelAutoSizeMode.None,
                ShowLineShadow = false
            };
            return labelControl;
        }

    }
}
