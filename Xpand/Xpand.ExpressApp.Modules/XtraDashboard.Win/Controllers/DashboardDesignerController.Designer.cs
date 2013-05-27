using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    partial class DashboardDesignerController {
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
            this._dashboardEdit = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this._dashboardExportXml = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this._dashboardImportXml = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // _dashboardEdit
            // 
            this._dashboardEdit.Caption = "Dashboard Edit";
            this._dashboardEdit.ConfirmationMessage = null;
            this._dashboardEdit.Id = "DashboardEdit";
            this._dashboardEdit.ImageName = "BO_DashboardDefinition";
            this._dashboardEdit.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this._dashboardEdit.Shortcut = null;
            this._dashboardEdit.Tag = null;
            this._dashboardEdit.TargetObjectsCriteria = null;
            this._dashboardEdit.TargetViewId = null;
            this._dashboardEdit.ToolTip = null;
            this._dashboardEdit.TypeOfView = null;
            this._dashboardEdit.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashboardEdit_Execute);
            // 
            // _dashboardExportXml
            // 
            this._dashboardExportXml.Caption = "Export to XML";
            this._dashboardExportXml.ConfirmationMessage = null;
            this._dashboardExportXml.Id = "DashboardExportXML";
            this._dashboardExportXml.ImageName = "BO_DashboardDefinition_Export";
            this._dashboardExportXml.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this._dashboardExportXml.Shortcut = null;
            this._dashboardExportXml.Tag = null;
            this._dashboardExportXml.TargetObjectsCriteria = null;
            this._dashboardExportXml.TargetViewId = null;
            this._dashboardExportXml.ToolTip = null;
            this._dashboardExportXml.TypeOfView = null;
            this._dashboardExportXml.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashbardExportXML_Execute);
            // 
            // _dashboardImportXml
            // 
            this._dashboardImportXml.Caption = "Import From XML";
            this._dashboardImportXml.ConfirmationMessage = null;
            this._dashboardImportXml.Id = "DashboardImportXml";
            this._dashboardImportXml.ImageName = "BO_DashboardDefinition_Import";
            this._dashboardImportXml.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this._dashboardImportXml.Shortcut = null;
            this._dashboardImportXml.Tag = null;
            this._dashboardImportXml.TargetObjectsCriteria = null;
            this._dashboardImportXml.TargetViewId = null;
            this._dashboardImportXml.ToolTip = null;
            this._dashboardImportXml.TypeOfView = null;
            this._dashboardImportXml.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashboardImportXML_Execute);

        }

        #endregion

        private SimpleAction _dashboardEdit;
        private SimpleAction _dashboardExportXml;
        private SimpleAction _dashboardImportXml;
    }
}
