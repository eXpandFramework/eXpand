using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object),false)]
    public class SerializableObjectPropertyEditor : ASPxPropertyEditor, IComplexPropertyEditor, ISerializableObjectPropertyEditor,ISupportEditControl {
        XafApplication _application;
        Panel _editModePanel;

        public SerializableObjectPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            SerializableObjectPropertyEditorBuilder.Create()
                .WithPropertyEditor(this)
                .WithApplication(() => _application)
                .Build(propertyEditor => ((TableEx) propertyEditor.Control).Rows[0].Cells[1].Controls[0]);
        }

        internal class Panel:System.Web.UI.WebControls.Panel,ISupportControl {
            Control _control;

            public object Control {
                get { return _control; }
                set {
                    _control = (Control) value;
                    Controls.Clear();
                    Controls.Add(_control);
                }
            }
        }
        protected override void ReadValueCore() {
        }

        protected override WebControl CreateEditModeControlCore() {
            _editModePanel = new Panel();
            return _editModePanel;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }

        ISupportControl ISupportEditControl.GetControl() {
            return _editModePanel;
        }
    }

}