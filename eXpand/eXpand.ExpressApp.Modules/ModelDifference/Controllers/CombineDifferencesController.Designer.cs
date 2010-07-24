using System.Diagnostics;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    partial class CombineDifferencesController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        [CoverageExclude]
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
            this.combineSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // combineSimpleAction
            // 
            this.combineSimpleAction.Caption = "Combine";
            this.combineSimpleAction.Id = "combineSimpleAction";
            this.combineSimpleAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.combineSimpleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.combineSimpleAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction combineSimpleAction;

    }
}