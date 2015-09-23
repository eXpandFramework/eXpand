namespace Xpand.CodeRush.Plugins {
    partial class PlugIn {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private DevExpress.CodeRush.Core.Action convertProject;
        private DevExpress.CodeRush.Core.Action collapseAllItemsInSolutionExplorer;
        private DevExpress.DXCore.PlugInCore.DXCoreEvents events;
        private DevExpress.CodeRush.Core.Action loadProjectFromReferenceItem;
        private DevExpress.CodeRush.Core.Action dropDataBase;
        private DevExpress.CodeRush.Core.Action exploreXafErrors;
        private DevExpress.CodeRush.PlugInCore.ActionHint _actionHint;
        private DevExpress.CodeRush.Core.Action RunEasyTest;
        private DevExpress.CodeRush.Core.Action DebugEasyTest;

        public PlugIn() {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlugIn));
            this.convertProject = new DevExpress.CodeRush.Core.Action(this.components);
            this.collapseAllItemsInSolutionExplorer = new DevExpress.CodeRush.Core.Action(this.components);
            this.events = new DevExpress.DXCore.PlugInCore.DXCoreEvents(this.components);
            this.loadProjectFromReferenceItem = new DevExpress.CodeRush.Core.Action(this.components);
            this.dropDataBase = new DevExpress.CodeRush.Core.Action(this.components);
            this.exploreXafErrors = new DevExpress.CodeRush.Core.Action(this.components);
            this._actionHint = new DevExpress.CodeRush.PlugInCore.ActionHint(this.components);
            this.RunEasyTest = new DevExpress.CodeRush.Core.Action(this.components);
            this.DebugEasyTest = new DevExpress.CodeRush.Core.Action(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._actionHint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunEasyTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugEasyTest)).BeginInit();
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
            this.convertProject.ToolbarItem.Enabled = true;
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
            this.collapseAllItemsInSolutionExplorer.ToolbarItem.Enabled = true;
            this.collapseAllItemsInSolutionExplorer.ToolbarItem.Image = null;
            this.collapseAllItemsInSolutionExplorer.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.collapseAllItemsInSolutionExplorer_Execute);
            // 
            // events
            // 
            this.events.ProjectBuildDone += new DevExpress.CodeRush.Core.BuildProjectDoneHandler(this.events_ProjectBuildDone);
            this.events.SolutionOpened += new DevExpress.CodeRush.Core.DefaultHandler(this.events_SolutionOpened);
            // 
            // loadProjectFromReferenceItem
            // 
            this.loadProjectFromReferenceItem.ActionName = "loadProjectFromReferenceItem";
            this.loadProjectFromReferenceItem.ButtonText = "Load Project/s";
            this.loadProjectFromReferenceItem.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.loadProjectFromReferenceItem.ImageBackColor = System.Drawing.Color.Empty;
            this.loadProjectFromReferenceItem.ToolbarItem.ButtonIsPressed = false;
            this.loadProjectFromReferenceItem.ToolbarItem.Enabled = true;
            this.loadProjectFromReferenceItem.ToolbarItem.Image = null;
            this.loadProjectFromReferenceItem.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.loadProjects_Execute);
            // 
            // dropDataBase
            // 
            this.dropDataBase.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.dropDataBase.ImageBackColor = System.Drawing.Color.Empty;
            this.dropDataBase.ToolbarItem.ButtonIsPressed = false;
            this.dropDataBase.ToolbarItem.Enabled = true;
            this.dropDataBase.ToolbarItem.Image = null;
            this.dropDataBase.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.DropDataBase_Execute);
            // 
            // exploreXafErrors
            // 
            this.exploreXafErrors.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.exploreXafErrors.ImageBackColor = System.Drawing.Color.Empty;
            this.exploreXafErrors.ToolbarItem.ButtonIsPressed = false;
            this.exploreXafErrors.ToolbarItem.Enabled = true;
            this.exploreXafErrors.ToolbarItem.Image = null;
            this.exploreXafErrors.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.exploreXafErrors_Execute);
            // 
            // _actionHint
            // 
            this._actionHint.Color = System.Drawing.Color.DarkGray;
            this._actionHint.Feature = null;
            this._actionHint.OptionsPath = null;
            this._actionHint.ResetDisplayCountOnStartup = false;
            this._actionHint.Text = null;
            // 
            // RunEasyTest
            // 
            this.RunEasyTest.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.RunEasyTest.ImageBackColor = System.Drawing.Color.Empty;
            this.RunEasyTest.ToolbarItem.ButtonIsPressed = false;
            this.RunEasyTest.ToolbarItem.Enabled = true;
            this.RunEasyTest.ToolbarItem.Image = null;
            this.RunEasyTest.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.RunEasyTest_Execute);
            // 
            // DebugEasyTest
            // 
            this.DebugEasyTest.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.DebugEasyTest.ImageBackColor = System.Drawing.Color.Empty;
            this.DebugEasyTest.ToolbarItem.ButtonIsPressed = false;
            this.DebugEasyTest.ToolbarItem.Enabled = true;
            this.DebugEasyTest.ToolbarItem.Image = null;
            this.DebugEasyTest.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.DebugEasyTest_Execute);
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._actionHint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunEasyTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugEasyTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion
    }
}