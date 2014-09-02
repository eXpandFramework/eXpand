using System.ComponentModel;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors;

namespace XpandAddIns
{
    partial class Options {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components=new Container();

        public Options() {
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.publicTokenTextEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl1 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.modelEditorPathButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.projectConverterPathButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl3 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnName = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridControl2 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnDirectory = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumnPrefix = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.button1 = new System.Windows.Forms.Button();
            this.formatOnSaveCheckEdit = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.testExecutorButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl6 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.checkEditDebugME = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelEditorPathButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // publicTokenTextEdit
            // 
            this.publicTokenTextEdit.Location = new System.Drawing.Point(122, 80);
            this.publicTokenTextEdit.Name = "publicTokenTextEdit";
            this.publicTokenTextEdit.Size = new System.Drawing.Size(152, 20);
            this.publicTokenTextEdit.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(8, 27);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(88, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Model editor path:";
            // 
            // modelEditorPathButtonEdit
            // 
            this.modelEditorPathButtonEdit.Location = new System.Drawing.Point(123, 24);
            this.modelEditorPathButtonEdit.Name = "modelEditorPathButtonEdit";
            this.modelEditorPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.modelEditorPathButtonEdit.Size = new System.Drawing.Size(334, 20);
            this.modelEditorPathButtonEdit.TabIndex = 4;
            this.modelEditorPathButtonEdit.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.modelEditorPathButtonEdit_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(7, 83);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 13);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "PublicToken";
            // 
            // projectConverterPathButtonEdit
            // 
            this.projectConverterPathButtonEdit.Location = new System.Drawing.Point(122, 52);
            this.projectConverterPathButtonEdit.Name = "projectConverterPathButtonEdit";
            this.projectConverterPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.projectConverterPathButtonEdit.Size = new System.Drawing.Size(152, 20);
            this.projectConverterPathButtonEdit.TabIndex = 7;
            this.projectConverterPathButtonEdit.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.projectConverterPathButtonEdit_ButtonClick);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(7, 55);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(90, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "ProjectConverter :";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(8, 117);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(507, 117);
            this.gridControl1.TabIndex = 12;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumnName});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowColumnResizing = false;
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView1.OptionsFilter.AllowFilterEditor = false;
            this.gridView1.OptionsFilter.AllowMRUFilterList = false;
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsMenu.EnableFooterMenu = false;
            this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView1.OptionsView.NewItemRowPosition = DevExpress.DXCore.Controls.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnName
            // 
            this.gridColumnName.Caption = "ConnectionStrings";
            this.gridColumnName.FieldName = "Name";
            this.gridColumnName.Name = "gridColumnName";
            this.gridColumnName.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumnName.Visible = true;
            this.gridColumnName.VisibleIndex = 0;
            // 
            // gridControl2
            // 
            this.gridControl2.Location = new System.Drawing.Point(8, 249);
            this.gridControl2.MainView = this.gridView2;
            this.gridControl2.Name = "gridControl2";
            this.gridControl2.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControl2.Size = new System.Drawing.Size(507, 112);
            this.gridControl2.TabIndex = 13;
            this.gridControl2.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDirectory,
            this.gridColumnPrefix,
            this.gridColumn1});
            this.gridView2.GridControl = this.gridControl2;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsCustomization.AllowColumnMoving = false;
            this.gridView2.OptionsCustomization.AllowColumnResizing = false;
            this.gridView2.OptionsCustomization.AllowGroup = false;
            this.gridView2.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView2.OptionsFilter.AllowFilterEditor = false;
            this.gridView2.OptionsFilter.AllowMRUFilterList = false;
            this.gridView2.OptionsMenu.EnableColumnMenu = false;
            this.gridView2.OptionsMenu.EnableFooterMenu = false;
            this.gridView2.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView2.OptionsView.NewItemRowPosition = DevExpress.DXCore.Controls.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnDirectory
            // 
            this.gridColumnDirectory.Caption = "Root Directory";
            this.gridColumnDirectory.FieldName = "RootPath";
            this.gridColumnDirectory.Name = "gridColumnDirectory";
            this.gridColumnDirectory.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumnDirectory.Visible = true;
            this.gridColumnDirectory.VisibleIndex = 0;
            // 
            // gridColumnPrefix
            // 
            this.gridColumnPrefix.Caption = "ProjectRegex";
            this.gridColumnPrefix.FieldName = "ProjectRegex";
            this.gridColumnPrefix.Name = "gridColumnPrefix";
            this.gridColumnPrefix.Visible = true;
            this.gridColumnPrefix.VisibleIndex = 1;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Count";
            this.gridColumn1.FieldName = "Count";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.AllowFocus = false;
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(440, 371);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Search Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // formatOnSaveCheckEdit
            // 
            this.formatOnSaveCheckEdit.Location = new System.Drawing.Point(280, 80);
            this.formatOnSaveCheckEdit.Name = "formatOnSaveCheckEdit";
            this.formatOnSaveCheckEdit.Properties.Caption = "Format On Save";
            this.formatOnSaveCheckEdit.Size = new System.Drawing.Size(231, 19);
            this.formatOnSaveCheckEdit.TabIndex = 27;
            // 
            // testExecutorButtonEdit
            // 
            this.testExecutorButtonEdit.Location = new System.Drawing.Point(378, 52);
            this.testExecutorButtonEdit.Name = "testExecutorButtonEdit";
            this.testExecutorButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.testExecutorButtonEdit.Size = new System.Drawing.Size(138, 20);
            this.testExecutorButtonEdit.TabIndex = 29;
            this.testExecutorButtonEdit.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.testExecutorButtonEdit_ButtonClick_1);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(282, 55);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(71, 13);
            this.labelControl6.TabIndex = 28;
            this.labelControl6.Text = "TestExecutor :";
            // 
            // checkEditDebugME
            // 
            this.checkEditDebugME.Location = new System.Drawing.Point(463, 25);
            this.checkEditDebugME.Name = "checkEditDebugME";
            this.checkEditDebugME.Properties.Caption = "Debug";
            this.checkEditDebugME.Size = new System.Drawing.Size(53, 19);
            this.checkEditDebugME.TabIndex = 30;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.checkEditDebugME);
            this.Controls.Add(this.testExecutorButtonEdit);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.formatOnSaveCheckEdit);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.projectConverterPathButtonEdit);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.modelEditorPathButtonEdit);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.publicTokenTextEdit);
            this.Controls.Add(this.gridControl2);
            this.Name = "Options";
            this.Size = new System.Drawing.Size(530, 488);
            this.CommitChanges += new DevExpress.CodeRush.Core.OptionsPage.CommitChangesEventHandler(this.Options_CommitChanges);
            ((System.ComponentModel.ISupportInitialize)(this.Images16x16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelEditorPathButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        ///
        /// Gets a DecoupledStorage instance for this options page.
        ///
        public static DecoupledStorage Storage {
            get {
                return CodeRush.Options.GetStorage(GetCategory(), GetPageName());
            }
        }
        ///
        /// Returns the category of this options page.
        ///
        public override string Category {
            get {
                return GetCategory();
            }
        }
        ///
        /// Returns the page name of this options page.
        ///
        public override string PageName {
            get {
                return GetPageName();
            }
        }
        ///
        /// Returns the full path (Category + PageName) of this options page.
        ///
        public static string FullPath {
            get {
                return GetCategory() + "\\" + GetPageName();
            }
        }

        ///
        /// Displays the DXCore options dialog and selects this page.
        ///
        public new static void Show() {
            CodeRush.Command.Execute("Options", FullPath);
        }
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private LabelControl labelControl1;
        private ButtonEdit modelEditorPathButtonEdit;
        private LabelControl labelControl2;
        private ButtonEdit publicTokenTextEdit;
        private ButtonEdit projectConverterPathButtonEdit;
        private LabelControl labelControl3;

        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl1;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnName;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl2;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnDirectory;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnPrefix;
        private DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.Button button1;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumn1;
        private CheckEdit formatOnSaveCheckEdit;
        private ButtonEdit testExecutorButtonEdit;
        private LabelControl labelControl6;
        private CheckEdit checkEditDebugME;
    }
}