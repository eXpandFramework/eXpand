using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object),false)]
    public class SerializableObjectPropertyEditor : ASPxPropertyEditor, IComplexPropertyEditor, ISerializableObjectPropertyEditor {
        XafApplication _application;

        public SerializableObjectPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            SerializableObjectPropertyEditorBuilder.Create()
                .WithPropertyEditor(this)
                .WithApplication(() => _application)
                .Build();
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
        protected override WebControl CreateEditModeControlCore() {
            return new Panel();
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }
    }
}