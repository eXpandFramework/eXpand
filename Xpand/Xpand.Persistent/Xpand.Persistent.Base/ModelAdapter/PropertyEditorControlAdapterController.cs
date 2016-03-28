using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;
using Xpand.Utils.Web;

namespace Xpand.Persistent.Base.ModelAdapter{
    public abstract class PropertyEditorControlAdapterController<TModelPropertyEditorControl, TModelControl, TPropertyEditor> : ModelAdapterController, IModelExtender
        where TModelPropertyEditorControl : IModelPropertyEditor
        where TModelControl : IModelModelAdapter{
        protected PropertyEditorControlAdapterController() {
            TargetViewType = ViewType.DetailView;
        }

        protected override void OnActivated() {
            base.OnActivated();
            var detailView = ((DetailView)View);
            foreach (var item in detailView.GetItems<PropertyEditor>().OfType<TPropertyEditor>().Cast<PropertyEditor>()) {
                item.ControlCreated += ItemOnControlCreated;
            }
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
            var item = (TPropertyEditor)sender;
            var modelPropertyEditorLabelControl = (TModelPropertyEditorControl) (item).GetPropertyValue("Model");
            var propertyEditorControl = GetPropertyEditorControl(item);
            if (propertyEditorControl != null){
                var modelNodes = GetControlModelNodes(modelPropertyEditorLabelControl);
                foreach (var node in modelNodes) {
                    new ObjectModelSynchronizer(propertyEditorControl, node).ApplyModel();   
                }
            }
        }

        protected virtual IEnumerable<TModelControl> GetControlModelNodes(TModelPropertyEditorControl modelPropertyEditorLabelControl){
            var modelControl = GetControlModel(modelPropertyEditorLabelControl);
            var modelModelAdapter =
                (IModelModelAdapter) modelPropertyEditorLabelControl.GetPropertyValue(modelControl.GetMemberInfo().Name);
            var modelAdapters = ((IEnumerable<IModelNode>) modelModelAdapter.GetNode("ModelAdapters")).Select(node => node.GetValue("ModelAdapter"));
            return modelAdapters.Concat(new[]{modelModelAdapter}).Cast<TModelControl>();
        }

        protected virtual object GetPropertyEditorControl(TPropertyEditor item){
            object control = item.GetPropertyValue("Control",Flags.AllMembers|Flags.TrimExplicitlyImplemented);
            return !Application.IsHosted() ? control
                : ((Control) control).FindNestedControls(GetControlType()).FirstOrDefault();
        }

        protected abstract Expression<Func<TModelPropertyEditorControl, IModelModelAdapter>> GetControlModel(TModelPropertyEditorControl modelPropertyEditorFilterControl);

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

        protected virtual IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(GetControlType()) {
                Act = info => (info.DXFilter())
            };
        }

    }
}
