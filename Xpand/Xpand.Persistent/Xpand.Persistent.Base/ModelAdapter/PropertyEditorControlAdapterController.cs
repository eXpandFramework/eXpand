using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Web;

namespace Xpand.Persistent.Base.ModelAdapter{
    public abstract class PropertyEditorControlAdapterController<TModelPropertyEditorControl, TModelControl> : ModelAdapterController, IModelExtender
        where TModelPropertyEditorControl : IModelPropertyEditor
        where TModelControl : IModelModelAdapter {
        protected PropertyEditorControlAdapterController() {
            TargetViewType = ViewType.DetailView;
        }

        protected override void OnActivated() {
            base.OnActivated();
            var detailView = ((DetailView)View);
            foreach (var item in detailView.GetItems<PropertyEditor>().Where(editor => editor is IModelPropertyEditorControlAdapter)) {
                item.ControlCreated += ItemOnControlCreated;
            }
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
            var item = (PropertyEditor)sender;
            var modelPropertyEditorLabelControl = (TModelPropertyEditorControl)item.Model;
            var propertyEditorControl = GetPropertyEditorControl(item);
            if (propertyEditorControl != null){
                var modelControls = GetControlModelNodes(modelPropertyEditorLabelControl);
                foreach (var modelControl in modelControls){
                    new ObjectModelSynchronizer(propertyEditorControl, modelControl).ApplyModel();   
                }                
            }
        }

        protected virtual object GetPropertyEditorControl(PropertyEditor item){
            return XpandModuleBase.IsHosted ? ((Control) item.Control).FindNestedControls(GetControlType()).FirstOrDefault() : item.Control;
        }

        protected abstract TModelControl[] GetControlModelNodes(TModelPropertyEditorControl modelPropertyEditorLabelControl);

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

    public interface IModelPropertyEditorControlAdapter {
         
    }
}
