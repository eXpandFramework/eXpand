using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using System.Linq;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberIsEmail {
        [Category("eXpand")]
        bool IsEmail { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberCursorPosition), "ModelMember")]
    public interface IModelPropertyEditorIsEmail : IModelMemberIsEmail {
    }
    public partial class EMailHighlightingController : ViewController<DetailView>, IModelExtender {
        public EMailHighlightingController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void EMailHighlightingController_Activated(object sender, EventArgs e) {
            if (View.Model.Items.OfType<IModelPropertyEditorIsEmail>().Where(email => email.IsEmail).FirstOrDefault()!=null)
                View.ControlsCreated += View_ControlsCreated;
        }
        private void View_ControlsCreated(object sender, EventArgs e) {
            foreach (PropertyEditor editor in View.GetItems<PropertyEditor>()) {
                editor.ValueRead += editor_ValueRead;
                editor.ControlValueChanged += editor_ControlValueChanged;
            }
        }

        void HighlightEditorValue(PropertyEditor editor) {
            if (!(editor is ListPropertyEditor) && (editor.ControlValue != null)) {
                var editorControl = (Control)editor.Control;
                var editorValue = editor.ControlValue as string;
                if (!string.IsNullOrEmpty(editorValue) &&
                        Regex.IsMatch(editorValue, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")) {
                    if (editorControl.Font.Style != FontStyle.Underline) {
                        editorControl.Font = new Font(editorControl.Font, FontStyle.Underline);
                        editorControl.ForeColor = Color.Blue;
                    }
                } else if (editorControl.Font.Style == FontStyle.Underline) {
                    editorControl.Font = new Font(editorControl.Font, FontStyle.Regular);
                    editorControl.ForeColor = Color.Black;
                }
            }
        }
        void editor_ControlValueChanged(object sender, EventArgs e) {
            HighlightEditorValue((PropertyEditor)sender);
        }

        void editor_ValueRead(object sender, EventArgs e) {
            HighlightEditorValue((PropertyEditor)sender);
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberIsEmail>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorIsEmail>();
        }
    }
}
