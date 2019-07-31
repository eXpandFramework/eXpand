//using System.ComponentModel;
//using DevExpress.DXCore.Controls.XtraEditors;
//using DevExpress.DXCore.Controls.XtraGrid;
//using DevExpress.DXCore.Controls.XtraGrid.Columns;
//using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
//
//namespace Xpand.VSIX.ModelEditor
//{
//    [System.Runtime.InteropServices.Guid("76ed6075-b303-4bb8-8071-6b3982623d5a")]
//    partial class METoolWindowControl
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components=new Container();
//
//
//
//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//
//        #region Windows Form Designer generated code
//
//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.gridView1 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
//            this.gridColumnName = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
//            this.label1 = new System.Windows.Forms.Label();
//            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
//            this.gridControl2 = new DevExpress.XtraGrid.GridControl();
//            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
//            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
//            this.SuspendLayout();
//            // 
//            // gridView1
//            // 
//            this.gridView1.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
//            this.gridColumnName});
//            this.gridView1.Name = "gridView1";
//            this.gridView1.OptionsBehavior.Editable = false;
//            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
//            this.gridView1.OptionsCustomization.AllowColumnResizing = false;
//            this.gridView1.OptionsCustomization.AllowGroup = false;
//            this.gridView1.OptionsFilter.AllowFilterEditor = false;
//            this.gridView1.OptionsMenu.EnableColumnMenu = false;
//            this.gridView1.OptionsMenu.EnableFooterMenu = false;
//            this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
//            this.gridView1.OptionsView.ShowAutoFilterRow = true;
//            this.gridView1.OptionsView.ShowGroupPanel = false;
//            this.gridView1.SortInfo.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumnSortInfo[] {
//            new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnName, DevExpress.DXCore.Controls.Data.ColumnSortOrder.Ascending)});
//            this.gridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyUp);
//            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
//            // 
//            // gridColumnName
//            // 
//            this.gridColumnName.Caption = "Models";
//            this.gridColumnName.FieldName = "Name";
//            this.gridColumnName.Name = "gridColumnName";
//            this.gridColumnName.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
//            this.gridColumnName.Visible = true;
//            this.gridColumnName.VisibleIndex = 0;
//            // 
//            // label1
//            // 
//            this.label1.AutoSize = true;
//            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
//            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
//            this.label1.Location = new System.Drawing.Point(0, 378);
//            this.label1.Name = "label1";
//            this.label1.Size = new System.Drawing.Size(281, 20);
//            this.label1.TabIndex = 2;
//            this.label1.Text = "CTRL+Return-->builds selection";
//            // 
//            // gridView2
//            // 
//            this.gridView2.Name = "gridView2";
//            // 
//            // gridControl2
//            // 
//            this.gridControl2.Location = new System.Drawing.Point(34, 37);
//            this.gridControl2.MainView = this.gridView3;
//            this.gridControl2.Name = "gridControl2";
//            this.gridControl2.Size = new System.Drawing.Size(400, 200);
//            this.gridControl2.TabIndex = 3;
//            this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
//            this.gridView3});
//            // 
//            // gridView3
//            // 
//            this.gridView3.GridControl = this.gridControl2;
//            this.gridView3.Name = "gridView3";
//            // 
//            // METoolWindowControl
//            // 
//            this.Controls.Add(this.gridControl2);
//            this.Controls.Add(this.label1);
//            this.Name = "METoolWindowControl";
//            this.Size = new System.Drawing.Size(709, 398);
//            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
//            this.ResumeLayout(false);
//            this.PerformLayout();
//
//        }
//
//        #endregion
//
//
//        private GridControl gridControl1;
//        private GridView gridView1;
//        private GridColumn gridColumnName;
//        
//        private System.Windows.Forms.Label label1;
//        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
//        private DevExpress.XtraGrid.GridControl gridControl2;
//        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
//    }
//}