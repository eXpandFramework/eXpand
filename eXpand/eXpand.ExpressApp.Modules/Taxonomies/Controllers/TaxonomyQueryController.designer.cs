using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.ExpressApp.Taxonomy.Controllers{
    partial class TaxonomyQueryController {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private SimpleAction executeTaxonomyQueryAction;

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
            this.executeTaxonomyQueryAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // executeTaxonomyQueryAction
            // 
            this.executeTaxonomyQueryAction.Caption = "Execute Query";
            this.executeTaxonomyQueryAction.Category = "Tools";
            this.executeTaxonomyQueryAction.Id = "ExecuteTaxonomyQueryActionId";
            this.executeTaxonomyQueryAction.Tag = null;
            this.executeTaxonomyQueryAction.TypeOfView = null;
            this.executeTaxonomyQueryAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ActionOnExecute);

        }
        #endregion
    }
}