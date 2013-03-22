using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.Dashboard.Win.Controllers {
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
            this.dashboardEdit = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.dashboardExportXML = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.dashboardImportXML = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // dashboardEdit
            // 
            this.dashboardEdit.Caption = "Dashboard Edit";
            this.dashboardEdit.ConfirmationMessage = null;
            this.dashboardEdit.Id = "DashboardEdit";
            this.dashboardEdit.ImageName = "BO_DashboardDefinition";
            this.dashboardEdit.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.dashboardEdit.Shortcut = null;
            this.dashboardEdit.Tag = null;
            this.dashboardEdit.TargetObjectsCriteria = null;
            this.dashboardEdit.TargetViewId = null;
            this.dashboardEdit.ToolTip = null;
            this.dashboardEdit.TypeOfView = null;
            this.dashboardEdit.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashboardEdit_Execute);
            // 
            // dashboardExportXML
            // 
            this.dashboardExportXML.Caption = "Export to XML";
            this.dashboardExportXML.ConfirmationMessage = null;
            this.dashboardExportXML.Id = "DashboardExportXML";
            this.dashboardExportXML.ImageName = "BO_DashboardDefinition_Export";
            this.dashboardExportXML.Shortcut = null;
            this.dashboardExportXML.Tag = null;
            this.dashboardExportXML.TargetObjectsCriteria = null;
            this.dashboardExportXML.TargetViewId = null;
            this.dashboardExportXML.ToolTip = null;
            this.dashboardExportXML.TypeOfView = null;
            this.dashboardExportXML.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashbardExportXML_Execute);
            // 
            // dashboardImportXML
            // 
            this.dashboardImportXML.Caption = "Import From XML";
            this.dashboardImportXML.ConfirmationMessage = null;
            this.dashboardImportXML.Id = "dashboardImportXML";
            this.dashboardImportXML.ImageName = "BO_DashboardDefinition_Import";
            this.dashboardImportXML.Shortcut = null;
            this.dashboardImportXML.Tag = null;
            this.dashboardImportXML.TargetObjectsCriteria = null;
            this.dashboardImportXML.TargetViewId = null;
            this.dashboardImportXML.ToolTip = null;
            this.dashboardImportXML.TypeOfView = null;
            this.dashboardImportXML.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.dashboardImportXML_Execute);

        }

        #endregion

        private SimpleAction dashboardEdit;
        private SimpleAction dashboardExportXML;
        private SimpleAction dashboardImportXML;
    }
}
