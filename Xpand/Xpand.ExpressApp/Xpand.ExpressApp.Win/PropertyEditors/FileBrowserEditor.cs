using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors{
    [PropertyEditor(typeof(string), EditorAliases.FileBrowserEditor, false)]
    public class FileBrowserEditor : DXPropertyEditor{
        public FileBrowserEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        private void buttonEdit_ButtonClick(object sender, ButtonPressedEventArgs e){
            using (var dialog = new OpenFileDialog()){
                if (dialog.ShowDialog() != DialogResult.Cancel) ((ButtonEdit) sender).Text = dialog.FileName;
            }
        }

        protected override object CreateControlCore(){
            return new ButtonEdit();
        }

        protected override RepositoryItem CreateRepositoryItem(){
            return new RepositoryItemButtonEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item){
            base.SetupRepositoryItem(item);
            ((RepositoryItemButtonEdit) item).ButtonClick += buttonEdit_ButtonClick;
        }

        protected override void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly){
            base.SetRepositoryItemReadOnly(item, readOnly);
            ((RepositoryItemButtonEdit) item).Buttons[0].Enabled = !readOnly;
        }
    }
}