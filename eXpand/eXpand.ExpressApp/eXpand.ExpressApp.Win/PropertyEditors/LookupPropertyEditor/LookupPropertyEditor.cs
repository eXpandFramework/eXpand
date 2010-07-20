using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors.LookupPropertyEditor{
    public class LookupPropertyEditor : DevExpress.ExpressApp.Win.Editors.LookupPropertyEditor
    {
        private EditorButton editButton;
        private DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit repositoryItemLookupEdit;

        public LookupPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemLookupEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            
            repositoryItemLookupEdit = (DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit) item;
            repositoryItemLookupEdit.Init(DisplayFormat, Helper);
            repositoryItemLookupEdit.EditValueChanged += RepositoryItemLookupEdit_OnEditValueChanged;
            editButton = new EditorButton(ButtonPredefines.Ellipsis, "", 20, true, true, true,
                                          HorzAlignment.Default, null,
                                          new KeyShortcut(Keys.Space),
                                          "Press this button or (Spacebar) to edit the object");
            if (View != null) View.ControlsCreated += View_OnControlsCreated;
            repositoryItemLookupEdit.Buttons.Add(editButton);
            ControlValueChanged += baseControlValueChanged;
        }

        private void RepositoryItemLookupEdit_OnEditValueChanged(object sender, EventArgs e)
        {
            try
            {
                editButton.Visible = ControlValue != null&&AllowEdit;
            }
            catch
            {
            }
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            editButton.Visible = ControlValue != null;
        }

        private void baseControlValueChanged(object sender, EventArgs e)
        {
            editButton.Visible = ControlValue != null;
        }

        protected override object CreateControlCore()
        {
            return new LookupEdit();
        }
    }
}