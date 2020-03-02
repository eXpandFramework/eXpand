using Xpand.ExpressApp.ModelDifference.DataStore.Validation;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Editors;
    using DevExpress.Persistent.Validation;
    using DevExpress.XtraRichEdit;

    namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
        public class XmlContentValidationController : ObjectViewController<DetailView, IXpoModelDifference> {
            private PropertyEditor _propertyEditor;
            private RichEditControl _richEditControl;

            protected override void OnActivated() {
                base.OnActivated();
                _propertyEditor = View.GetItems<RichEditWinPropertyEditor>().FirstOrDefault(editor => editor.MemberInfo.Name == nameof(IXpoModelDifference.XmlContent));
                if (_propertyEditor != null) _propertyEditor.ControlCreated += PropertyEditorOnControlCreated;
                ObjectSpace.Committing += ObjectSpaceOnCommitting;
            }

            protected override void OnDeactivated() {
                base.OnDeactivated();
                if (_propertyEditor != null) _propertyEditor.ControlCreated -= PropertyEditorOnControlCreated;
                ObjectSpace.Committing -= ObjectSpaceOnCommitting;

            }

            private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
                if (_richEditControl != null && _richEditControl.Focused)
                    MergeWithModel();
            }

            private void PropertyEditorOnControlCreated(object sender, EventArgs eventArgs) {
                _richEditControl = ((RichEditContainerBase)((PropertyEditor)sender).Control).RichEditControl;
                _richEditControl.Leave += OnLeave;
            }

            private void OnLeave(object sender, EventArgs eventArgs) {
                try {
                    if (View != null && _propertyEditor.ControlValue!=_propertyEditor.MemberInfo.GetValue(View.CurrentObject)){
                        _propertyEditor.MemberInfo.SetValue(View.CurrentObject, _propertyEditor.ControlValue);
                        _propertyEditor.MemberInfo.SetValue(View.CurrentObject, _propertyEditor.ControlValue);
                        Validator.RuleSet.Validate(ObjectSpace, View.CurrentObject, XmlContentCodeRule.MDOXmlContentContext);
                        MergeWithModel();
                    }
                }
                catch (ValidationException) {
                    ((Control)sender).Focus();
                    throw;
                }
            }

            private void MergeWithModel() {
                var modelEditorPropertyEditor = View.GetItems<ModelEditorPropertyEditor>().FirstOrDefault();
                modelEditorPropertyEditor?.MergeXmlWithModel();
            }
        }
    }

    //    public class XmlContentValidationController:ObjectViewController<DetailView,IXpoModelDifference>{
    //        private RichEditControl _richEditControl;
    //        private PropertyEditor _propertyEditor;
    //        private string _xmlContent;
    //
    //        protected override void OnActivated(){
    //            base.OnActivated();
    //            _propertyEditor = View.GetItems<PropertyEditor>().FirstOrDefault(editor => editor.MemberInfo.Name==nameof(IXpoModelDifference.XmlContent));
    //            if (_propertyEditor != null) _propertyEditor.ControlCreated+=PropertyEditorOnControlCreated;
    //            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
    //            ObjectSpace.Committed+=ObjectSpaceOnCommitted;
    //            _xmlContent = ((IXpoModelDifference) View.CurrentObject).XmlContent;
    //            View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
    //            ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
    //        }
    //
    //
    //        protected override void OnDeactivated(){
    //            base.OnDeactivated();
    //            if (_propertyEditor != null) _propertyEditor.ControlCreated-=PropertyEditorOnControlCreated;
    //            ObjectSpace.Committed -= ObjectSpaceOnCommitted;
    //            ObjectSpace.Committing-=ObjectSpaceOnCommitting;
    //            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
    //            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
    //            ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
    //        }
    //
    //        private void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs){
    //            _xmlContent = ((IXpoModelDifference)View.CurrentObject).XmlContent;
    //        }
    //
    //        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
    //            _xmlContent = ((IXpoModelDifference)View.CurrentObject).XmlContent;
    //        }
    //
    //        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
    //            string propertyName = e.PropertyName;
    //            if (View != null && (!string.IsNullOrEmpty(propertyName) && e.Object.GetType() == View.ObjectTypeInfo.Type) && e.PropertyName == nameof(IXpoModelDifference.XmlContent)) {
    //                if (_xmlContent != ((IXpoModelDifference)View.CurrentObject).XmlContent) {
    //                    Validator.RuleSet.Validate(ObjectSpace, View.CurrentObject, XmlContentCodeRule.MDOXmlContentContext);
    //                    MergeWithModel();
    //                    _xmlContent = ((IXpoModelDifference)View.CurrentObject).XmlContent;
    //                }
    //            }
    //        }
    //
    //        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
    //            if (_richEditControl != null && _richEditControl.Focused)
    //                MergeWithModel();
    //        }
    //
    //        private void PropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
    //            var richEditContainerBase = ( ((PropertyEditor) sender).Control) as RichEditContainerBase;
    //            _richEditControl = richEditContainerBase?.RichEditControl;
    //        }
    //
    //
    //        private void MergeWithModel(){
    //            var modelEditorPropertyEditor = View.GetItems<ModelEditorPropertyEditor>().FirstOrDefault();
    //            modelEditorPropertyEditor?.MergeXmlWithModel();
    //        }
    //    }
}
