using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Web;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.ModelAdapter{
//    public abstract class PropertyEditorControlAdapterController<TModelMemberViewItem, TModelControl, TPropertyEditor> : ModelAdapterController, IModelExtender
//        where TModelMemberViewItem : IModelMemberViewItem
//        where TModelControl : IModelModelAdapter{
//        private HashSet<ObjectModelSynchronizer> _objectModelSynchronizers;
//
//        protected override void OnActivated() {
//            base.OnActivated();
//
//	        _objectModelSynchronizers = new HashSet<ObjectModelSynchronizer>();
//			var listView = View as ListView;
//            var gridListEditor = listView?.Editor as ASPxGridListEditor;
//            if (gridListEditor != null)
//                gridListEditor.CustomizeAutoFilterCellEditor += GridListEditorOnCustomizeAutoFilterCellEditor;
//            foreach (var propertyEditor in GetPropertyEditors()) {
//                propertyEditor.ControlCreated += ItemOnControlCreated;
//            }
//        }
//
//        protected override void OnDeactivated() {
//            base.OnDeactivated();
//            var listView = View as ListView;
//            var gridListEditor = listView?.Editor as ASPxGridListEditor;
//            if (gridListEditor != null)
//                gridListEditor.CustomizeAutoFilterCellEditor -= GridListEditorOnCustomizeAutoFilterCellEditor;
//            if (_objectModelSynchronizers != null) {
//                foreach (var objectModelSynchronizer in _objectModelSynchronizers) {
//                    SynchronizeModel(objectModelSynchronizer);
//                    objectModelSynchronizer.Dispose();
//                }
//                foreach (var propertyEditor in GetPropertyEditors()) {
//                    propertyEditor.ControlCreated -= ItemOnControlCreated;
//                }
//            }
//        }
//
//        private void GridListEditorOnCustomizeAutoFilterCellEditor(object sender, CustomHandleAutoFilterCellEditorEventArgs e){
//            ((ASPxGridListEditor) sender).CustomizeAutoFilterCellEditor-=GridListEditorOnCustomizeAutoFilterCellEditor;
//            ApplyModel((TModelMemberViewItem) e.ColumnModel,e.SourceEventArgs.EditorProperties);
//        }
//
//        protected override void OnViewControlsCreated(){
//            base.OnViewControlsCreated();
//            foreach (var propertyEditor in GetPropertyEditors()){
//                propertyEditor.ControlCreated-=ItemOnControlCreated;
//                propertyEditor.ControlCreated+=ItemOnControlCreated;
//            }
//        }
//
//        private IEnumerable<PropertyEditor> GetPropertyEditors(){
//            var detailView = View as DetailView;
//            if (detailView != null)
//                foreach (var editor in detailView.GetItems<PropertyEditor>().OfType<TPropertyEditor>().Cast<PropertyEditor>()){
//                    yield return editor;
//                }
//            var listView = View as ListView;
//            if (listView!=null){
//                if (Application.GetPlatform()==Platform.Web  && listView.AllowEdit){
//                    var listViewEditor = listView.Editor as ComplexWebListEditor;
//                    if (listViewEditor != null){
//                        var propertyEditors = listViewEditor.PropertyEditors.OfType<TPropertyEditor>().Cast<PropertyEditor>();
//                        foreach (var item in propertyEditors){
//                            yield return item;
//                        }
//                    }
//                }
//            }
//        }
//
//        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
//            ((PropertyEditor) sender).ControlCreated-=ItemOnControlCreated;
//            var item = (TPropertyEditor)sender;
//            var propertyEditorControl = GetPropertyEditorControl(item);
//            if (propertyEditorControl != null){
//                var model = (TModelMemberViewItem)(item).GetPropertyValue("Model");
//                ApplyModel(model, propertyEditorControl);
//            }
//        }
//
//        private void ApplyModel(TModelMemberViewItem model, object targetObject){
//            var modelNodes = GetControlModelNodes(model);
//            foreach (var node in modelNodes){
//                var objectModelSynchronizer = new ObjectModelSynchronizer(targetObject, node);
//                _objectModelSynchronizers.Add(objectModelSynchronizer);
//                ApplyModel(objectModelSynchronizer);
//            }
//        }
//
//        protected virtual void ApplyModel(ObjectModelSynchronizer objectModelSynchronizer){
//            objectModelSynchronizer.ApplyModel();
//        }
//
//        protected virtual void SynchronizeModel(ObjectModelSynchronizer objectModelSynchronizer){
//            objectModelSynchronizer.SynchronizeModel();
//        }
//
//        protected virtual IEnumerable<TModelControl> GetControlModelNodes(TModelMemberViewItem modelPropertyEditorLabelControl){
//            var modelControl = GetControlModel(modelPropertyEditorLabelControl);
//            var modelModelAdapter =
//                (IModelModelAdapter) modelPropertyEditorLabelControl.GetPropertyValue(modelControl.GetMemberInfo().Name);
//            var modelAdapters = ((IEnumerable<IModelNode>) modelModelAdapter.GetNode("ModelAdapters")).Select(node => node.GetValue("ModelAdapter"));
//            return modelAdapters.Concat(new[]{modelModelAdapter}).Cast<TModelControl>();
//        }
//
//        protected virtual object GetPropertyEditorControl(TPropertyEditor item){
//            object control = item.GetPropertyValue("Control",Flags.AllMembers|Flags.TrimExplicitlyImplemented);
//            return Application.GetPlatform()==Platform.Win ? control
//                : ((Control) control).FindNestedControls(GetControlType()).FirstOrDefault();
//        }
//
//        protected abstract Expression<Func<TModelMemberViewItem, IModelModelAdapter>> GetControlModel(TModelMemberViewItem modelPropertyEditorFilterControl);
//
//        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            extenders.Add<IModelMemberViewItem, TModelMemberViewItem>();
//            var builder = new InterfaceBuilder(extenders);
//            var assembly = builder.Build(CreateBuilderData(), GetPath(GetControlType().Name));
//            builder.ExtendInteface(typeof(TModelControl),GetControlType(),assembly);
//            ExtendingModelInterfaces(builder, assembly, extenders);
//        }
//
//        protected abstract Type GetControlType();
//
//        protected virtual void ExtendingModelInterfaces(InterfaceBuilder builder, Assembly assembly, ModelInterfaceExtenders extenders) {
//
//        }
//
//        protected virtual IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            yield return new InterfaceBuilderData(GetControlType()) {
//                Act = info => (info.DXFilter())
//            };
//        }
//
//    }
}
