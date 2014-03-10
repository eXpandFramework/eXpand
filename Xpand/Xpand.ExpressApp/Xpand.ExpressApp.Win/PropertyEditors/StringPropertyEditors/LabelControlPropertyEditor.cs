using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
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

    public class LabelPropertyEditorVisibilityCalculator:EditorTypeVisibilityCalculator{
        public override bool IsVisible(IModelNode node, string propertyName){
            return typeof(LabelControlPropertyEditor).IsAssignableFrom(EditorType(node));
        }
    }

    public interface IModelLabelControl:IModelNode{
         
    }

    public class LabelControlSynchronizer : Persistent.Base.ModelAdapter.ModelSynchronizer<LabelControl, IModelPropertyEditorLabelControl> {
        public LabelControlSynchronizer(LabelControl component, IModelPropertyEditorLabelControl modelNode)
            : base(component, modelNode) {
        }


        protected override void ApplyModelCore() {
            ApplyModel(Model.LabelControl, Control, ApplyValues);
        }

        public override void SynchronizeModel() {
            throw new NotImplementedException();
        }
    }

    public class LabelControlModelAdapterController:ModelAdapterController,IModelExtender{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var detailView = View as DetailView;
            if (detailView!=null)
                foreach (var item in detailView.GetItems<LabelControlPropertyEditor>()){
                    var modelPropertyEditorLabelControl = (IModelPropertyEditorLabelControl)item.Model;
                    new LabelControlSynchronizer((LabelControl)item.Control,modelPropertyEditorLabelControl).ApplyModel();
                }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorLabelControl>();
            var builder = new InterfaceBuilder(extenders);
            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(LabelControl).Name));

            builder.ExtendInteface<IModelLabelControl, LabelControl>(assembly);

            var calcType = builder.CalcType(typeof(LabelControlAppearanceObject), assembly);
            extenders.Add(calcType, typeof (IModelAppearanceFont));
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(LabelControl)) {
                Act = info => (info.DXFilter())
            };
        }
    }
    [PropertyEditor(typeof(string),false)]
    public class LabelControlPropertyEditor:WinPropertyEditor {
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
