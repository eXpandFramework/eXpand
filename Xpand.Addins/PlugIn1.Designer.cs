namespace XpandAddIns
{
    partial class PlugIn1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public PlugIn1() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();
        }

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlugIn1));
            this.convertProject = new DevExpress.CodeRush.Core.Action(this.components);
            this.collapseAllItemsInSolutionExplorer = new DevExpress.CodeRush.Core.Action(this.components);
            this.exploreXafErrors = new DevExpress.CodeRush.Core.Action(this.components);
            this.events = new DevExpress.DXCore.PlugInCore.DXCoreEvents(this.components);
            this.dropDataBase = new DevExpress.CodeRush.Core.Action(this.components);
            this.actionHint1 = new DevExpress.CodeRush.PlugInCore.ActionHint(this.components);
            this.loadProjectFromReferenceItem = new DevExpress.CodeRush.Core.Action(this.components);
            this.loadProjectFromObjectBrowser = new DevExpress.CodeRush.Core.Action(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionHint1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromObjectBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // convertProject
            // 
            this.convertProject.ActionName = "convertProject";
            this.convertProject.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.convertProject.Image = ((System.Drawing.Bitmap)(resources.GetObject("convertProject.Image")));
            this.convertProject.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.convertProject.ToolbarItem.ButtonIsPressed = false;
            this.convertProject.ToolbarItem.Caption = null;
            this.convertProject.ToolbarItem.Image = null;
            this.convertProject.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.convertProject_Execute);
            // 
            // collapseAllItemsInSolutionExplorer
            // 
            this.collapseAllItemsInSolutionExplorer.ActionName = "collapseAllItemsInSolutionExplorer";
            this.collapseAllItemsInSolutionExplorer.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.collapseAllItemsInSolutionExplorer.Image = ((System.Drawing.Bitmap)(resources.GetObject("collapseAllItemsInSolutionExplorer.Image")));
            this.collapseAllItemsInSolutionExplorer.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.collapseAllItemsInSolutionExplorer.ToolbarItem.ButtonIsPressed = false;
            this.collapseAllItemsInSolutionExplorer.ToolbarItem.Caption = null;
            this.collapseAllItemsInSolutionExplorer.ToolbarItem.Image = null;
            this.collapseAllItemsInSolutionExplorer.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.collapseAllItemsInSolutionExplorer_Execute);
            // 
            // exploreXafErrors
            // 
            this.exploreXafErrors.ActionName = "exploreXafErrors";
            this.exploreXafErrors.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.exploreXafErrors.Image = ((System.Drawing.Bitmap)(resources.GetObject("exploreXafErrors.Image")));
            this.exploreXafErrors.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.exploreXafErrors.ToolbarItem.ButtonIsPressed = false;
            this.exploreXafErrors.ToolbarItem.Caption = null;
            this.exploreXafErrors.ToolbarItem.Image = null;
            this.exploreXafErrors.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.exploreXafErrors_Execute);
            // 
            // events
            // 
            this.events.ProjectBuildDone += new DevExpress.CodeRush.Core.BuildProjectDoneHandler(this.events_ProjectBuildDone);
            this.events.DocumentSaved += new DevExpress.CodeRush.Core.DocumentEventHandler(this.events_DocumentSaved);
            // 
            // dropDataBase
            // 
            this.dropDataBase.ActionName = "dropDataBase";
            this.dropDataBase.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.dropDataBase.Image = ((System.Drawing.Bitmap)(resources.GetObject("dropDataBase.Image")));
            this.dropDataBase.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.dropDataBase.ToolbarItem.ButtonIsPressed = false;
            this.dropDataBase.ToolbarItem.Caption = null;
            this.dropDataBase.ToolbarItem.Image = null;
            this.dropDataBase.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.SpAtDesignTime_Execute);
            // 
            // actionHint1
            // 
            this.actionHint1.Color = System.Drawing.Color.MediumBlue;
            this.actionHint1.Feature = null;
            this.actionHint1.OptionsPath = null;
            this.actionHint1.ResetDisplayCountOnStartup = true;
            this.actionHint1.Text = "DataBase Dropped !!!";
            // 
            // loadProjectFromReferenceItem
            // 
            this.loadProjectFromReferenceItem.ActionName = "loadProjectFromReferenceItem";
            this.loadProjectFromReferenceItem.ButtonText = "Load Project/s";
            this.loadProjectFromReferenceItem.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.loadProjectFromReferenceItem.Image = ((System.Drawing.Bitmap)(resources.GetObject("loadProjectFromReferenceItem.Image")));
            this.loadProjectFromReferenceItem.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.loadProjectFromReferenceItem.ParentMenu = "Reference Item";
            this.loadProjectFromReferenceItem.ToolbarItem.ButtonIsPressed = false;
            this.loadProjectFromReferenceItem.ToolbarItem.Caption = null;
            this.loadProjectFromReferenceItem.ToolbarItem.Image = null;
            this.loadProjectFromReferenceItem.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.loadProjects_Execute);
            // 
            // loadProjectFromObjectBrowser
            // 
            this.loadProjectFromObjectBrowser.ActionName = "loadProjectFromObjectBrowser";
            this.loadProjectFromObjectBrowser.ButtonText = "Load Project/s";
            this.loadProjectFromObjectBrowser.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.loadProjectFromObjectBrowser.Image = ((System.Drawing.Bitmap)(resources.GetObject("loadProjectFromObjectBrowser.Image")));
            this.loadProjectFromObjectBrowser.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.loadProjectFromObjectBrowser.ParentMenu = "Object Browser Objects Pane1";
            this.loadProjectFromObjectBrowser.ToolbarItem.ButtonIsPressed = false;
            this.loadProjectFromObjectBrowser.ToolbarItem.Caption = null;
            this.loadProjectFromObjectBrowser.ToolbarItem.Image = null;
            this.loadProjectFromObjectBrowser.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.loadProjects_Execute);
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionHint1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromObjectBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.CodeRush.Core.Action convertProject;
        private DevExpress.CodeRush.Core.Action collapseAllItemsInSolutionExplorer;
        private DevExpress.CodeRush.Core.Action exploreXafErrors;
        private DevExpress.DXCore.PlugInCore.DXCoreEvents events;
        private DevExpress.CodeRush.Core.Action dropDataBase;
        private DevExpress.CodeRush.PlugInCore.ActionHint actionHint1;
        private DevExpress.CodeRush.Core.Action loadProjectFromReferenceItem;
        private DevExpress.CodeRush.Core.Action loadProjectFromObjectBrowser;
    }
}