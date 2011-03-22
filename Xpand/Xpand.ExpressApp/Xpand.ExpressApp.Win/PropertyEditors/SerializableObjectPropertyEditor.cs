using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils.Controls;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof(object), false)]
    public class SerializableObjectPropertyEditor : WinPropertyEditor, IComplexPropertyEditor, ISerializableObjectPropertyEditor {
        public SerializableObjectPropertyEditor(Type objectType, IModelMemberViewItem modelMemberViewItem)
            : base(objectType, modelMemberViewItem) {
            SerializableObjectPropertyEditorBuilder.Create()
                .WithApplication(() => _application)
                .WithPropertyEditor(this).
                Build();
        }



        internal class MyUserControl : UserControl, IXtraResizableControl, ISupportControl {
            #region IXtraResizableControl Members

            public event EventHandler Changed;

            public bool IsCaptionVisible {
                get {
                    if (xtraResizableControl != null) {
                        return xtraResizableControl.IsCaptionVisible;
                    }
                    return true;
                }
            }

            public Size MaxSize {
                get {
                    if (xtraResizableControl != null) {
                        return xtraResizableControl.MaxSize;
                    }
                    return Size.Empty;
                }
            }

            public Size MinSize {
                get {
                    if (xtraResizableControl != null) {
                        return xtraResizableControl.MinSize;
                    }
                    return Size.Empty;
                }
            }

            #endregion
            private Control control;
            private IXtraResizableControl xtraResizableControl;
            public Control Control {
                get {
                    return control;
                }
                set {
                    control = value;
                    Controls.Clear();
                    if (control != null) {
                        value.Dock = DockStyle.Fill;
                        Controls.Add(value);
                    }
                    xtraResizableControl = control as IXtraResizableControl;
                    if (Changed != null) {
                        Changed(this, EventArgs.Empty);
                    }
                }
            }
            object ISupportControl.Control {
                get { return Control; }
                set { Control = value as Control; }
            }
        }

        protected override object CreateControlCore() {
            return new MyUserControl();
        }

        protected override void ReadValueCore() {
        }

        

        XafApplication _application;

        #region IComplexPropertyEditor Members

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }

        #endregion
    }

}
