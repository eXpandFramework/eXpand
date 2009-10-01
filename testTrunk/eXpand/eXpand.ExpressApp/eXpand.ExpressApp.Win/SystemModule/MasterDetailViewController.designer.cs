namespace eXpand.ExpressApp.Win.SystemModule
{
    partial class MasterDetailViewController
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
            this.ExpandAllRowsSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CollapseAllRowsSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ExpandAllRowsSimpleAction
            // 
            this.ExpandAllRowsSimpleAction.Caption = "ExpandAllRows";
            this.ExpandAllRowsSimpleAction.Id = "ExpandAllRows";
            this.ExpandAllRowsSimpleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExpandAllRowsSimpleAction_Execute);
            // 
            // CollapseAllRowsSimpleAction
            // 
            this.CollapseAllRowsSimpleAction.Caption = "CollapseAllRows";
            this.CollapseAllRowsSimpleAction.Id = "CollapseAllRows";
            this.CollapseAllRowsSimpleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CollapseAllRowsSimpleAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ExpandAllRowsSimpleAction;
        private DevExpress.ExpressApp.Actions.SimpleAction CollapseAllRowsSimpleAction;
    }
}