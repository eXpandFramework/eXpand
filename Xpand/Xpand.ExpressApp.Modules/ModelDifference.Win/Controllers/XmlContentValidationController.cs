using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.ModelDifference.DataStore.Validation;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class XmlContentValidationController:ObjectViewController<DetailView,IXpoModelDifference>{
        private PropertyEditor _propertyEditor;

        protected override void OnActivated(){
            base.OnActivated();
            _propertyEditor = View.GetItems<PropertyEditor>().FirstOrDefault(editor => editor.MemberInfo.Name==nameof(IXpoModelDifference.XmlContent));
            if (_propertyEditor != null) _propertyEditor.ControlCreated+=PropertyEditorOnControlCreated;
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_propertyEditor != null) _propertyEditor.ControlCreated-=PropertyEditorOnControlCreated;
            ObjectSpace.Committing-=ObjectSpaceOnCommitting;
            
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            MergeWithModel();
        }

        private void PropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
            ((Control) ((PropertyEditor) sender).Control).Leave+=OnLeave;
        }

        private void OnLeave(object sender, EventArgs eventArgs){
            try{
                Validator.RuleSet.Validate(ObjectSpace, View.CurrentObject, XmlContentCodeRule.MDOXmlContentContext);
                MergeWithModel();
            }
            catch (ValidationException ){
                ((Control) sender).Focus();
                throw;
            }
        }

        private void MergeWithModel(){
            var modelEditorPropertyEditor = View.GetItems<ModelEditorPropertyEditor>().FirstOrDefault();
            modelEditorPropertyEditor?.MergeXmlWithModel();
        }
    }
}
