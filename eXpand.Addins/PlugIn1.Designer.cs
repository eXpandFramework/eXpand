namespace eXpandAddIns
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
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionHint1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // convertProject
            // 
            this.convertProject.ActionName = "convertProject";
            this.convertProject.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.convertProject.Image = ((System.Drawing.Bitmap)(resources.GetObject("convertProject.Image")));
            this.convertProject.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.convertProject.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.convertProject_Execute);
            // 
            // collapseAllItemsInSolutionExplorer
            // 
            this.collapseAllItemsInSolutionExplorer.ActionName = "collapseAllItemsInSolutionExplorer";
            this.collapseAllItemsInSolutionExplorer.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.collapseAllItemsInSolutionExplorer.Image = ((System.Drawing.Bitmap)(resources.GetObject("collapseAllItemsInSolutionExplorer.Image")));
            this.collapseAllItemsInSolutionExplorer.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.collapseAllItemsInSolutionExplorer.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.collapseAllItemsInSolutionExplorer_Execute);
            // 
            // exploreXafErrors
            // 
            this.exploreXafErrors.ActionName = "exploreXafErrors";
            this.exploreXafErrors.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.exploreXafErrors.Image = ((System.Drawing.Bitmap)(resources.GetObject("exploreXafErrors.Image")));
            this.exploreXafErrors.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.exploreXafErrors.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.exploreXafErrors_Execute);
            // 
            // events
            // 
            this.events.DebuggerEnterDesignMode += new DevExpress.CodeRush.Core.DebuggerEnterDesignModeHandler(this.events_DebuggerEnterDesignMode);
            this.events.DebuggerEnterRunMode += new DevExpress.CodeRush.Core.DebuggerEnterRunModeHandler(this.events_DebuggerEnterRunMode);
            this.events.DebuggerContextChanged += new DevExpress.CodeRush.Core.DebuggerContextChangedHandler(this.events_DebuggerContextChanged);
            // 
            // dropDataBase
            // 
            this.dropDataBase.ActionName = "dropDataBase";
            this.dropDataBase.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.dropDataBase.Image = ((System.Drawing.Bitmap)(resources.GetObject("dropDataBase.Image")));
            this.dropDataBase.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.dropDataBase.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.SpAtDesignTime_Execute);
            // 
            // actionHint1
            // 
            this.actionHint1.Color = System.Drawing.Color.MediumBlue;
            this.actionHint1.Feature = null;
            this.actionHint1.OptionsPath = null;
            this.actionHint1.ResetDisplayCountOnStartup = true;
            this.actionHint1.Text = "DataBase Dropped !!!";
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dropDataBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionHint1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.CodeRush.Core.Action convertProject;
        private DevExpress.CodeRush.Core.Action collapseAllItemsInSolutionExplorer;
        private DevExpress.CodeRush.Core.Action exploreXafErrors;
        private DevExpress.DXCore.PlugInCore.DXCoreEvents events;
        private DevExpress.CodeRush.Core.Action dropDataBase;
        private DevExpress.CodeRush.PlugInCore.ActionHint actionHint1;
    }
}