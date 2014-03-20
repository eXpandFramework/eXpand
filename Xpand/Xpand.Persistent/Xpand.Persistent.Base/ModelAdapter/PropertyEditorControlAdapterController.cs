using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.ModelAdapter{
    public abstract class PropertyEditorControlAdapterController<TModelPropertyEditorControl, TModelControl> : ModelAdapterController, IModelExtender
        where TModelPropertyEditorControl : IModelPropertyEditor
        where TModelControl : IModelNodeEnabled  {
        protected PropertyEditorControlAdapterController() {
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailView = ((DetailView)View);
            foreach (var item in detailView.GetItems<PropertyEditor>().Where(editor => editor.Model is TModelPropertyEditorControl)) {
                var modelPropertyEditorLabelControl = (TModelPropertyEditorControl)item.Model;
                new ObjectModelSynchronizer(item.Control, GetControlModelNode(modelPropertyEditorLabelControl)).ApplyModel();
            }
        }

        protected abstract TModelControl GetControlModelNode(TModelPropertyEditorControl modelPropertyEditorLabelControl);

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPropertyEditor, TModelPropertyEditorControl>();
            var builder = new InterfaceBuilder(extenders);
            var assembly = builder.Build(CreateBuilderData(), GetPath(GetControlType().Name));
            builder.ExtendInteface(typeof(TModelControl),GetControlType(),assembly);
            ExtendingModelInterfaces(builder, assembly, extenders);
        }

        protected abstract Type GetControlType();

        protected virtual void ExtendingModelInterfaces(InterfaceBuilder builder, Assembly assembly, ModelInterfaceExtenders extenders) {

        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(GetControlType()) {
                Act = info => (info.DXFilter())
            };
        }

    }
}
