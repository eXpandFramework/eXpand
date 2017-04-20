namespace Xpand.VSIX.Wizard {
    partial class WizardForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.gridControl1 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnModule = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumnPlatform = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumnInstall = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEditInstall = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditInstall)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.Location = new System.Drawing.Point(8, 8);
            this.gridControl1.MainView = this.gridView;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEditInstall});
            this.gridControl1.Size = new System.Drawing.Size(508, 681);
            this.gridControl1.TabIndex = 12;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumnModule,
            this.gridColumnPlatform,
            this.gridColumnInstall});
            this.gridView.GridControl = this.gridControl1;
            this.gridView.GroupCount = 1;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowColumnResizing = false;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsFilter.AllowFilterEditor = false;
            this.gridView.OptionsMenu.EnableColumnMenu = false;
            this.gridView.OptionsMenu.EnableFooterMenu = false;
            this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsView.ShowAutoFilterRow = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.SortInfo.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnPlatform, DevExpress.DXCore.Controls.Data.ColumnSortOrder.Descending),
            new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnModule, DevExpress.DXCore.Controls.Data.ColumnSortOrder.Ascending)});
            // 
            // gridColumnModule
            // 
            this.gridColumnModule.Caption = "Module";
            this.gridColumnModule.FieldName = "Module";
            this.gridColumnModule.Name = "gridColumnModule";
            this.gridColumnModule.OptionsColumn.AllowEdit = false;
            this.gridColumnModule.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumnModule.Visible = true;
            this.gridColumnModule.VisibleIndex = 1;
            this.gridColumnModule.Width = 446;
            // 
            // gridColumnPlatform
            // 
            this.gridColumnPlatform.Caption = "Platform";
            this.gridColumnPlatform.FieldName = "Platform";
            this.gridColumnPlatform.Name = "gridColumnPlatform";
            this.gridColumnPlatform.OptionsColumn.AllowEdit = false;
            this.gridColumnPlatform.Visible = true;
            this.gridColumnPlatform.VisibleIndex = 1;
            // 
            // gridColumnInstall
            // 
            this.gridColumnInstall.ColumnEdit = this.repositoryItemCheckEditInstall;
            this.gridColumnInstall.FieldName = "Install";
            this.gridColumnInstall.Name = "gridColumnInstall";
            this.gridColumnInstall.OptionsColumn.AllowSize = false;
            this.gridColumnInstall.Visible = true;
            this.gridColumnInstall.VisibleIndex = 0;
            this.gridColumnInstall.Width = 41;
            // 
            // repositoryItemCheckEditInstall
            // 
            this.repositoryItemCheckEditInstall.AutoHeight = false;
            this.repositoryItemCheckEditInstall.Name = "repositoryItemCheckEditInstall";
            // 
            // buttonFinish
            // 
            this.buttonFinish.Location = new System.Drawing.Point(427, 698);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(75, 23);
            this.buttonFinish.TabIndex = 13;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.Location = new System.Drawing.Point(13, 698);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(408, 33);
            this.labelMessage.TabIndex = 14;
            // 
            // WizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 733);
            this.ControlBox = false;
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.gridControl1);
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "eXpandFramework modules (Ctrl+A, Space)";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditInstall)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl1;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnModule;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnPlatform;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnInstall;
        private DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditInstall;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Label labelMessage;
    }
}