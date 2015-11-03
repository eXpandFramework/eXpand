using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class FolderBrowseEditor : WinPropertyEditor {
        public FolderBrowseEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            var propertyType = model.ModelMember.Type;
            var validTypes = new List<Type>{
                typeof(string) 
            };
            if (!validTypes.Contains(propertyType))
                throw new Exception("Can't use FolderBrowseEditor with property type " + propertyType.FullName);
            ControlBindingProperty = "Value";
        }

        ButtonEdit _folderPath;
        EditorButton _openFolderButton;

        private void FolderPathButtonClick(object sender, ButtonPressedEventArgs e) {
            if (e.Button.Kind == ButtonPredefines.Right) {
                Process.Start(_folderPath.Text);
            } else {
                var dialog = new FolderBrowserDialog { Description = "Select folder..." };
                if (dialog.ShowDialog() != DialogResult.Cancel) {
                    _folderPath.Text = dialog.SelectedPath;
                }
            }
        }


        private void FolderPathEditValueChanged(object sender, EventArgs e) {
            PropertyValue = _folderPath.Text;
            if (!inReadValue)
                OnControlValueChanged();
        }

        protected override object CreateControlCore() {
            _folderPath = new ButtonEdit();
            _openFolderButton = new EditorButton(ButtonPredefines.Right) { Visible = false ,Caption = "Browse"};
            var editorButtonCollection = _folderPath.Properties.Buttons;
            var editorButton = editorButtonCollection.OfType<EditorButton>().First(button => button.IsDefaultButton);
            editorButton.Caption = "Dialog";
            editorButtonCollection.Add(_openFolderButton);
            _folderPath.TextChanged += FolderPathOnTextChanged;
            _folderPath.ButtonClick += FolderPathButtonClick;
            _folderPath.EditValueChanged += FolderPathEditValueChanged;
            return _folderPath;
        }

        void FolderPathOnTextChanged(object sender, EventArgs eventArgs) {
            _openFolderButton.Visible = Directory.Exists(_folderPath.Text);
        }

        protected override void ReadValueCore(){
            var text = Convert.ToString(PropertyValue);
            _folderPath.Text = text;
        }

        protected override void Dispose(bool disposing) {
            if (_folderPath != null) {
                _folderPath.TextChanged -= FolderPathOnTextChanged;
                _folderPath.ButtonClick -= FolderPathButtonClick;
                _folderPath.EditValueChanged -= FolderPathEditValueChanged;
            }
            base.Dispose(disposing);
        }

    }
}