namespace Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor
{
    partial class TwoDimensionEditorViewItemController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TwoDimensionViewItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // TwoDimensionViewItem
            // 
            this.TwoDimensionViewItem.Caption = "View";
            this.TwoDimensionViewItem.Category = "RecordEdit";
            this.TwoDimensionViewItem.ConfirmationMessage = null;
            this.TwoDimensionViewItem.Id = "TwoDimensionViewItem";
            this.TwoDimensionViewItem.ImageName = "Eye-icon";
            this.TwoDimensionViewItem.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.TwoDimensionViewItem.ToolTip = null;
            this.TwoDimensionViewItem.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.TwoDimensionViewItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.TwoDimensionViewItem_Execute);

            this.TargetObjectType = typeof(ITwoDimensionItem);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
        }

        #endregion

        protected DevExpress.ExpressApp.Actions.SimpleAction TwoDimensionViewItem;
    }
}
