using System.Diagnostics;

namespace eXpand.ExpressApp.SystemModule
{
    partial class RecycleBinViewController
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
            this.recylcleBinSingleChoiceAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.restoreFormRecycleBinsimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // recylcleBinSingleChoiceAction
            // 
            this.recylcleBinSingleChoiceAction.Caption = "Filter";
            this.recylcleBinSingleChoiceAction.Id = "recylcleBinSingleChoiceAction";
            this.recylcleBinSingleChoiceAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.recylcleBinSingleChoiceAction_Execute);
            // 
            // restoreFormRecycleBinsimpleAction
            // 
            this.restoreFormRecycleBinsimpleAction.Caption = "restore";
            this.restoreFormRecycleBinsimpleAction.Id = "restoreFormRecycleBinsimpleAction";
            

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction recylcleBinSingleChoiceAction;
        private DevExpress.ExpressApp.Actions.SimpleAction restoreFormRecycleBinsimpleAction;
    }
}