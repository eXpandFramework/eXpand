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
            this.action1 = new DevExpress.CodeRush.Core.Action(this.components);
            this.ImplementXpoManyPart = new DevExpress.CodeRush.Core.CodeProvider(this.components);
            this.ImplementXpoOnePart = new DevExpress.CodeRush.Core.CodeProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.action1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImplementXpoManyPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImplementXpoOnePart)).BeginInit();
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
            // action1
            // 
            this.action1.ActionName = "test";
            this.action1.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.action1.Image = ((System.Drawing.Bitmap)(resources.GetObject("action1.Image")));
            this.action1.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.action1.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.action1_Execute);
            // 
            // ImplementXpoManyPart
            // 
            this.ImplementXpoManyPart.ActionHintText = "";
            this.ImplementXpoManyPart.AutoActivate = true;
            this.ImplementXpoManyPart.AutoUndo = false;
            this.ImplementXpoManyPart.Description = "Implement Xpo Many Part";
            this.ImplementXpoManyPart.Image = ((System.Drawing.Bitmap)(resources.GetObject("ImplementXpoManyPart.Image")));
            this.ImplementXpoManyPart.NeedsSelection = false;
            this.ImplementXpoManyPart.ProviderName = "ImplementXpoManyPart";
            this.ImplementXpoManyPart.Register = true;
            this.ImplementXpoManyPart.SupportsAsyncMode = false;
            this.ImplementXpoManyPart.Apply += new DevExpress.Refactor.Core.ApplyRefactoringEventHandler(this.ImplementXpoManyPart_Apply);
            this.ImplementXpoManyPart.CheckAvailability += new DevExpress.Refactor.Core.CheckAvailabilityEventHandler(this.ImplementXpoManyPart_CheckAvailability);
            // 
            // ImplementXpoOnePart
            // 
            this.ImplementXpoOnePart.ActionHintText = "";
            this.ImplementXpoOnePart.AutoActivate = true;
            this.ImplementXpoOnePart.AutoUndo = false;
            this.ImplementXpoOnePart.Description = "Implement Xpo One Part";
            this.ImplementXpoOnePart.Image = ((System.Drawing.Bitmap)(resources.GetObject("ImplementXpoOnePart.Image")));
            this.ImplementXpoOnePart.NeedsSelection = false;
            this.ImplementXpoOnePart.ProviderName = "ImplementXpoOnePart";
            this.ImplementXpoOnePart.Register = true;
            this.ImplementXpoOnePart.SupportsAsyncMode = false;
            this.ImplementXpoOnePart.Apply += new DevExpress.Refactor.Core.ApplyRefactoringEventHandler(this.ImplementXpoOnePart_Apply);
            this.ImplementXpoOnePart.CheckAvailability += new DevExpress.Refactor.Core.CheckAvailabilityEventHandler(this.ImplementXpoOnePart_CheckAvailability);
            // 
            // PlugIn1
            // 
            this.BuildBegin += new DevExpress.CodeRush.Core.BuildEventHandler(this.PlugIn1_BuildBegin);
            ((System.ComponentModel.ISupportInitialize)(this.convertProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collapseAllItemsInSolutionExplorer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreXafErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.action1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImplementXpoManyPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImplementXpoOnePart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.CodeRush.Core.Action convertProject;
        private DevExpress.CodeRush.Core.Action collapseAllItemsInSolutionExplorer;
        private DevExpress.CodeRush.Core.Action exploreXafErrors;
        private DevExpress.CodeRush.Core.Action action1;
        private DevExpress.CodeRush.Core.CodeProvider ImplementXpoManyPart;
        private DevExpress.CodeRush.Core.CodeProvider ImplementXpoOnePart;
    }
}