namespace eXpand.ExpressApp.TreeListEditors.Win.Controllers
{
    partial class ResursiveFilteringViewController
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
            this.RecursiveFilterPopLookUpTreeSelectionSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // RecursiveFilterPopLookUpTreeSelectionSimpleAction
            // 
            this.RecursiveFilterPopLookUpTreeSelectionSimpleAction.Caption = "Recursive Selection";
            this.RecursiveFilterPopLookUpTreeSelectionSimpleAction.Category = "PopupActions";
            this.RecursiveFilterPopLookUpTreeSelectionSimpleAction.Id = "RecursiveFilterPopLookUpTreeSelectionSimpleAction";
            this.RecursiveFilterPopLookUpTreeSelectionSimpleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.RecursiveFilterPopLookUpTreeSelectionSimpleAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction RecursiveFilterPopLookUpTreeSelectionSimpleAction;
    }
}
