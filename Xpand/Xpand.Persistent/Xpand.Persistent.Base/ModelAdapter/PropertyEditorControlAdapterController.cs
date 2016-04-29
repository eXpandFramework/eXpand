using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public abstract class PropertyEditorControlAdapterController<TModelMemberViewItem, TModelControl, TPropertyEditor> : ModelAdapterController, IModelExtender
        where TModelMemberViewItem : IModelMemberViewItem
        where TModelControl : IModelModelAdapter{
        List<ObjectModelSynchronizer> _objectModelSynchronizers;
        protected PropertyEditorControlAdapterController() {
            TargetViewType = ViewType.DetailView;
        }

        protected override void OnActivated() {
            base.OnActivated();
            _objectModelSynchronizers=new List<ObjectModelSynchronizer>();
            var detailView = ((DetailView)View);
            foreach (var item in detailView.GetItems<PropertyEditor>().OfType<TPropertyEditor>().Cast<PropertyEditor>()) {
                item.ControlCreated += ItemOnControlCreated;
            }
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
            var item = (TPropertyEditor)sender;
            var modelPropertyEditorLabelControl = (TModelMemberViewItem) (item).GetPropertyValue("Model");
            var propertyEditorControl = GetPropertyEditorControl(item);
            if (propertyEditorControl != null){
                ((IComponent) propertyEditorControl).Disposed+=OnDisposed;
                var modelNodes = GetControlModelNodes(modelPropertyEditorLabelControl);
                foreach (var node in modelNodes){
                    var objectModelSynchronizer = new ObjectModelSynchronizer(propertyEditorControl, node);
                    _objectModelSynchronizers.Add(objectModelSynchronizer);
                    objectModelSynchronizer.ApplyModel();
                }
            }
        }

        private void OnDisposed(object sender, EventArgs eventArgs){
            ((IComponent) sender).Disposed-=OnDisposed;
            foreach (var objectModelSynchronizer in _objectModelSynchronizers){
                objectModelSynchronizer.SynchronizeModel();
                objectModelSynchronizer.Dispose();
            }
        }

        protected virtual IEnumerable<TModelControl> GetControlModelNodes(TModelMemberViewItem modelPropertyEditorLabelControl){
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

        protected abstract Expression<Func<TModelMemberViewItem, IModelModelAdapter>> GetControlModel(TModelMemberViewItem modelPropertyEditorFilterControl);

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMemberViewItem, TModelMemberViewItem>();
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
