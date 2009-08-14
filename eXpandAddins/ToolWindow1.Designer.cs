using DevExpress.DXCore.Controls.XtraEditors;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Columns;
using DevExpress.DXCore.Controls.XtraGrid.Views.Base;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;

namespace eXpandAddIns
{
    [System.Runtime.InteropServices.Guid("76ed6075-b303-4bb8-8071-6b3982623d5a")]
    partial class ToolWindow1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;
        private DevExpress.DXCore.PlugInCore.DXCoreEvents events;

        public ToolWindow1()
        {
            // Required for Windows.Forms Class Composition Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolWindow1));
            this.events = new DevExpress.DXCore.PlugInCore.DXCoreEvents(this.components);
            this.gridControl1 = new GridControl();
            this.gridView1 = new GridView();
            this.gridColumnName = new GridColumn();
            this.textEdit1 = new TextEdit();
            this.openModelEditorAction = new DevExpress.CodeRush.Core.Action(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openModelEditorAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(709, 356);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new GridColumn[] {
            this.gridColumnName});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowColumnResizing = false;
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsFilter.AllowFilterEditor = false;
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsMenu.EnableFooterMenu = false;
            this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            this.gridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyUp);
            // 
            // gridColumnName
            // 
            this.gridColumnName.Caption = "Models";
            this.gridColumnName.FieldName = "Name";
            this.gridColumnName.Name = "gridColumnName";
            this.gridColumnName.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
            this.gridColumnName.Visible = true;
            this.gridColumnName.VisibleIndex = 0;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(309, 282);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(100, 20);
            this.textEdit1.TabIndex = 1;
            // 
            // openModelEditorAction
            // 
            this.openModelEditorAction.ActionName = "OpenModelEditor";
            this.openModelEditorAction.CommonMenu = DevExpress.CodeRush.Menus.VsCommonBar.None;
            this.openModelEditorAction.Image = ((System.Drawing.Bitmap)(resources.GetObject("openModelEditorAction.Image")));
            this.openModelEditorAction.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.openModelEditorAction.RegisterInVS = true;
            this.openModelEditorAction.Execute += new DevExpress.CodeRush.Core.CommandExecuteEventHandler(this.openModelEditorAction_Execute);
            // 
            // ToolWindow1
            // 
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.textEdit1);
            this.Image = ((System.Drawing.Bitmap)(resources.GetObject("$this.Image")));
            this.ImageBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Name = "ToolWindow1";
            this.Size = new System.Drawing.Size(709, 356);
            ((System.ComponentModel.ISupportInitialize)(this.events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openModelEditorAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #region ShowWindow
        ///
        /// Displays this tool window.
        ///
        public static EnvDTE.Window ShowWindow()
        {
            return DevExpress.CodeRush.Core.CodeRush.ToolWindows.Show(typeof(ToolWindow1).GUID);
        }
        #endregion
        #region HideWindow
        ///
        /// Hides this tool window.
        ///
        public static EnvDTE.Window HideWindow()
        {
            return DevExpress.CodeRush.Core.CodeRush.ToolWindows.Hide(typeof(ToolWindow1).GUID);
        }
        #endregion

        private GridControl gridControl1;
        private GridView gridView1;
        private GridColumn gridColumnName;
        private TextEdit textEdit1;
        private DevExpress.CodeRush.Core.Action openModelEditorAction;
    }
}