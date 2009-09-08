namespace eXpand.ExpressApp.Win.SystemModule
{
    partial class EditModeViewController {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.toggleEditMode = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // toggleEditMode
            // 
            this.toggleEditMode.Caption = "Edit";
            this.toggleEditMode.Category = "RecordEdit";
            this.toggleEditMode.Id = "361c18eb-0253-4f98-9e62-0c44c03d8794";
            this.toggleEditMode.ImageName = "MenuBar_Edit";
            this.toggleEditMode.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.toggleEditMode.ToolTip = "Edit the selected record.";

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction toggleEditMode;
    }
}