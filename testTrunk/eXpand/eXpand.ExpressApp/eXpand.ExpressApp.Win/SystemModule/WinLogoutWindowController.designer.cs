namespace eXpand.ExpressApp.Win.SystemModule
{
    partial class WinLogoutWindowController
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
            this.logOutSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // logOutSimpleAction
            // 
            this.logOutSimpleAction.Caption = "Log Out";
            this.logOutSimpleAction.Category = "Export";
            this.logOutSimpleAction.Id = "logOutSimpleAction";
            this.logOutSimpleAction.Tag = null;
            this.logOutSimpleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.logOutSimpleAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction logOutSimpleAction;
    }
}