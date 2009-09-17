using Foxhound.ExpressApp.Scheduler.BaseObjects.Ranges;

namespace Foxhound.ExpressApp.Scheduler.Controllers {
    partial class PersistentDateRangeInfoViewController {
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
            this.FilterByInfoTypeAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // FilterByInfoTypeAction
            // 
            this.FilterByInfoTypeAction.Caption = "Info Type Filter";
            this.FilterByInfoTypeAction.Id = "DateRangeInfoTypeFilter";
            this.FilterByInfoTypeAction.Tag = null;
            this.FilterByInfoTypeAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.FilterByInfoTypeAction_Execute);
            // 
            // PersistentDateRangeInfoViewController
            // 
            this.TargetObjectType = typeof(PersistentDateRangeInfo);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction FilterByInfoTypeAction;
    }
}
