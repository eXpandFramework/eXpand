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
        private DevExpress.CodeRush.Core.Action RunEasyTest;
        private DevExpress.CodeRush.Core.Action DebugEasyTest;


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
            this.RunEasyTest = new DevExpress.CodeRush.Core.Action(this.components);
            this.DebugEasyTest = new DevExpress.CodeRush.Core.Action(this.components);
            this.ReplaceReferencesFromFolders = new DevExpress.CodeRush.Core.Action(this.components);
            this.MissingReferencesRoot = new DevExpress.CodeRush.Core.Action(this.components);
            this.MissingReferencesProject = new DevExpress.CodeRush.Core.Action(this.components);
            this.MissingReferences = new DevExpress.CodeRush.Core.Action(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunEasyTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugEasyTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceReferencesFromFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferencesRoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferencesProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferences)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // convertProject
            // 
            this.convertProject.ActionName = "ConvertProject";
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
            this.collapseAllItemsInSolutionExplorer.ActionName = "CollapseAllItemsInSolutionExplorer";
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
            this.events.ProjectBuildBegin += new DevExpress.CodeRush.Core.BuildProjectBeginHandler(this.events_ProjectBuildBegin);
            this.events.ProjectBuildDone += new DevExpress.CodeRush.Core.BuildProjectDoneHandler(this.events_ProjectBuildDone);
            this.events.SolutionOpened += new DevExpress.CodeRush.Core.DefaultHandler(this.events_SolutionOpened);
            // 
            // loadProjectFromReferenceItem
            // 
            this.loadProjectFromReferenceItem.ActionName = "LoadProjectFromReferenceItem";
            this.loadProjectFromReferenceItem.ButtonText = "Load Project/s";
            this.loadProjectFromReferenceItem.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.loadProjectFromReferenceItem.ImageBackColor = System.Drawing.Color.Empty;
            this.loadProjectFromReferenceItem.ParentMenu = "Reference Item";
            this.loadProjectFromReferenceItem.ToolbarItem.ButtonIsPressed = false;
            this.loadProjectFromReferenceItem.ToolbarItem.Enabled = true;
            this.loadProjectFromReferenceItem.ToolbarItem.Image = null;
            this.loadProjectFromReferenceItem.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.loadProjects_Execute);
            // 
            // dropDataBase
            // 
            this.dropDataBase.ActionName = "DropDatabase";
            this.dropDataBase.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.dropDataBase.ImageBackColor = System.Drawing.Color.Empty;
            this.dropDataBase.ToolbarItem.ButtonIsPressed = false;
            this.dropDataBase.ToolbarItem.Enabled = true;
            this.dropDataBase.ToolbarItem.Image = null;
            this.dropDataBase.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.DropDataBase_Execute);
            // 
            // exploreXafErrors
            // 
            this.exploreXafErrors.ActionName = "ExploreXafErrors";
            this.exploreXafErrors.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.exploreXafErrors.ImageBackColor = System.Drawing.Color.Empty;
            this.exploreXafErrors.ToolbarItem.ButtonIsPressed = false;
            this.exploreXafErrors.ToolbarItem.Enabled = true;
            this.exploreXafErrors.ToolbarItem.Image = null;
            this.exploreXafErrors.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.exploreXafErrors_Execute);
            // 
            // RunEasyTest
            // 
            this.RunEasyTest.ActionName = "RunEasyTest";
            this.RunEasyTest.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.RunEasyTest.ImageBackColor = System.Drawing.Color.Empty;
            this.RunEasyTest.ToolbarItem.ButtonIsPressed = false;
            this.RunEasyTest.ToolbarItem.Enabled = true;
            this.RunEasyTest.ToolbarItem.Image = null;
            this.RunEasyTest.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.RunEasyTest_Execute);
            // 
            // DebugEasyTest
            // 
            this.DebugEasyTest.ActionName = "DebugEasyTest";
            this.DebugEasyTest.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.DebugEasyTest.ImageBackColor = System.Drawing.Color.Empty;
            this.DebugEasyTest.RegisterInVS = true;
            this.DebugEasyTest.ToolbarItem.ButtonIsPressed = false;
            this.DebugEasyTest.ToolbarItem.Enabled = true;
            this.DebugEasyTest.ToolbarItem.Image = null;
            this.DebugEasyTest.ToolbarItem.Index = 0;
            this.DebugEasyTest.ToolbarItem.PlaceIntoToolbar = true;
            this.DebugEasyTest.ToolbarItem.ToolbarName = "Xpand";
            this.DebugEasyTest.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.DebugEasyTest_Execute);
            // 
            // ReplaceReferencesFromFolders
            // 
            this.ReplaceReferencesFromFolders.ActionName = "ReplaceReferencesFromFolders";
            this.ReplaceReferencesFromFolders.ButtonText = "Replace selected";
            this.ReplaceReferencesFromFolders.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.ReplaceReferencesFromFolders.ImageBackColor = System.Drawing.Color.Empty;
            this.ReplaceReferencesFromFolders.ParentMenu = "Reference Item";
            this.ReplaceReferencesFromFolders.ToolbarItem.ButtonIsPressed = false;
            this.ReplaceReferencesFromFolders.ToolbarItem.Caption = null;
            this.ReplaceReferencesFromFolders.ToolbarItem.Enabled = true;
            this.ReplaceReferencesFromFolders.ToolbarItem.Image = null;
            this.ReplaceReferencesFromFolders.ToolbarItem.ToolbarName = null;
            this.ReplaceReferencesFromFolders.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.replaceReferencePath_Execute);
            // 
            // MissingReferencesRoot
            // 
            this.MissingReferencesRoot.ActionName = "MissingReferencesRoot";
            this.MissingReferencesRoot.ButtonText = "Locate missing";
            this.MissingReferencesRoot.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.MissingReferencesRoot.ImageBackColor = System.Drawing.Color.Empty;
            this.MissingReferencesRoot.ParentMenu = "Reference Root";
            this.MissingReferencesRoot.ToolbarItem.ButtonIsPressed = false;
            this.MissingReferencesRoot.ToolbarItem.Caption = null;
            this.MissingReferencesRoot.ToolbarItem.Enabled = true;
            this.MissingReferencesRoot.ToolbarItem.Image = null;
            this.MissingReferencesRoot.ToolbarItem.ToolbarName = null;
            this.MissingReferencesRoot.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.MissingReferences_Execute);
            // 
            // MissingReferencesProject
            // 
            this.MissingReferencesProject.ActionName = "MissingReferencesProject";
            this.MissingReferencesProject.ButtonText = "Locate missing references";
            this.MissingReferencesProject.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.MissingReferencesProject.ImageBackColor = System.Drawing.Color.Empty;
            this.MissingReferencesProject.ParentMenu = "Project";
            this.MissingReferencesProject.ToolbarItem.ButtonIsPressed = false;
            this.MissingReferencesProject.ToolbarItem.Caption = null;
            this.MissingReferencesProject.ToolbarItem.Enabled = true;
            this.MissingReferencesProject.ToolbarItem.Image = null;
            this.MissingReferencesProject.ToolbarItem.ToolbarName = null;
            this.MissingReferencesProject.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.MissingReferences_Execute);
            // 
            // MissingReferences
            // 
            this.MissingReferences.ActionName = "MissingReferences";
            this.MissingReferences.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.MissingReferences.ImageBackColor = System.Drawing.Color.Empty;
            this.MissingReferences.ToolbarItem.ButtonIsPressed = false;
            this.MissingReferences.ToolbarItem.Caption = null;
            this.MissingReferences.ToolbarItem.Enabled = true;
            this.MissingReferences.ToolbarItem.Image = null;
            this.MissingReferences.ToolbarItem.ToolbarName = null;
            this.MissingReferences.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.MissingReferences_Execute);
            // 
            // PlugIn
            // 
            this.DocumentActivated += new DevExpress.CodeRush.Core.DocumentEventHandler(this.PlugIn_DocumentActivated);
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadProjectFromReferenceItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunEasyTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugEasyTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceReferencesFromFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferencesRoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferencesProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MissingReferences)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.CodeRush.Core.Action ReplaceReferencesFromFolders;
        private DevExpress.CodeRush.Core.Action MissingReferencesRoot;
        private DevExpress.CodeRush.Core.Action MissingReferencesProject;
        private DevExpress.CodeRush.Core.Action MissingReferences;
    }
}