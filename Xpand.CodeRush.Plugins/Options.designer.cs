using System.ComponentModel;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors;

namespace XpandPlugins
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
            this.General = new System.Windows.Forms.TabPage();
            this.EasyTests = new System.Windows.Forms.TabPage();
            this.ProjectConverter = new System.Windows.Forms.TabPage();
            this.publicTokenTextEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.projectConverterPathButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.DropDatabase = new System.Windows.Forms.TabPage();
            this.gridControl1 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnName = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.LoadProject = new System.Windows.Forms.TabPage();
            this.gridControl2 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheckEdit1 = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumnDirectory = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumnPrefix = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.testExecutorButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl6 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.specificVersionCheckEdit = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.formatOnSaveCheckEdit = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.ModelEditor = new System.Windows.Forms.TabPage();
            this.checkEditDebugME = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.modelEditorPathButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl1 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl9 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl11 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl12 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl13 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl14 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.General.SuspendLayout();
            this.EasyTests.SuspendLayout();
            this.ProjectConverter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).BeginInit();
            this.DropDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.LoadProject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).BeginInit();
            this.ModelEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelEditorPathButtonEdit.Properties)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // General
            // 
            this.General.Controls.Add(this.tableLayoutPanel4);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Size = new System.Drawing.Size(522, 462);
            this.General.TabIndex = 4;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // EasyTests
            // 
            this.EasyTests.Controls.Add(this.tableLayoutPanel2);
            this.EasyTests.Controls.Add(this.labelControl10);
            this.EasyTests.Location = new System.Drawing.Point(4, 22);
            this.EasyTests.Name = "EasyTests";
            this.EasyTests.Size = new System.Drawing.Size(522, 462);
            this.EasyTests.TabIndex = 3;
            this.EasyTests.Text = "EasyTests";
            this.EasyTests.UseVisualStyleBackColor = true;
            // 
            // ProjectConverter
            // 
            this.ProjectConverter.Controls.Add(this.tableLayoutPanel1);
            this.ProjectConverter.Controls.Add(this.labelControl9);
            this.ProjectConverter.Location = new System.Drawing.Point(4, 22);
            this.ProjectConverter.Name = "ProjectConverter";
            this.ProjectConverter.Size = new System.Drawing.Size(522, 462);
            this.ProjectConverter.TabIndex = 2;
            this.ProjectConverter.Text = "Project Converter";
            this.ProjectConverter.UseVisualStyleBackColor = true;
            // 
            // publicTokenTextEdit
            // 
            this.publicTokenTextEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.publicTokenTextEdit.Location = new System.Drawing.Point(120, 32);
            this.publicTokenTextEdit.Name = "publicTokenTextEdit";
            this.publicTokenTextEdit.Size = new System.Drawing.Size(399, 20);
            this.publicTokenTextEdit.TabIndex = 8;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(3, 32);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 13);
            this.labelControl2.TabIndex = 9;
            this.labelControl2.Text = "PublicToken";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(3, 3);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(112, 13);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "ProjectConverter path:";
            // 
            // projectConverterPathButtonEdit
            // 
            this.projectConverterPathButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectConverterPathButtonEdit.Location = new System.Drawing.Point(120, 3);
            this.projectConverterPathButtonEdit.Name = "projectConverterPathButtonEdit";
            this.projectConverterPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.projectConverterPathButtonEdit.Size = new System.Drawing.Size(399, 20);
            this.projectConverterPathButtonEdit.TabIndex = 11;
            // 
            // DropDatabase
            // 
            this.DropDatabase.Controls.Add(this.gridControl1);
            this.DropDatabase.Controls.Add(this.labelControl8);
            this.DropDatabase.Controls.Add(this.labelControl7);
            this.DropDatabase.Location = new System.Drawing.Point(4, 22);
            this.DropDatabase.Name = "DropDatabase";
            this.DropDatabase.Padding = new System.Windows.Forms.Padding(3);
            this.DropDatabase.Size = new System.Drawing.Size(522, 462);
            this.DropDatabase.TabIndex = 1;
            this.DropDatabase.Text = "Drop Database";
            this.DropDatabase.UseVisualStyleBackColor = true;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(3, 57);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(516, 402);
            this.gridControl1.TabIndex = 13;
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
            // LoadProject
            // 
            this.LoadProject.Controls.Add(this.button1);
            this.LoadProject.Controls.Add(this.gridControl2);
            this.LoadProject.Controls.Add(this.labelControl5);
            this.LoadProject.Controls.Add(this.labelControl4);
            this.LoadProject.Location = new System.Drawing.Point(4, 22);
            this.LoadProject.Name = "LoadProject";
            this.LoadProject.Padding = new System.Windows.Forms.Padding(3);
            this.LoadProject.Size = new System.Drawing.Size(522, 462);
            this.LoadProject.TabIndex = 0;
            this.LoadProject.Text = "Add referenced Projects ";
            this.LoadProject.UseVisualStyleBackColor = true;
            // 
            // gridControl2
            // 
            this.gridControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl2.Location = new System.Drawing.Point(3, 65);
            this.gridControl2.MainView = this.gridView2;
            this.gridControl2.Name = "gridControl2";
            this.gridControl2.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControl2.Size = new System.Drawing.Size(516, 394);
            this.gridControl2.TabIndex = 17;
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
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
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
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(3, 436);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(516, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Search Selected";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.LoadProject);
            this.tabControl1.Controls.Add(this.DropDatabase);
            this.tabControl1.Controls.Add(this.ProjectConverter);
            this.tabControl1.Controls.Add(this.EasyTests);
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.ModelEditor);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(530, 488);
            this.tabControl1.TabIndex = 32;
            // 
            // testExecutorButtonEdit
            // 
            this.testExecutorButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testExecutorButtonEdit.Location = new System.Drawing.Point(102, 3);
            this.testExecutorButtonEdit.Name = "testExecutorButtonEdit";
            this.testExecutorButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.testExecutorButtonEdit.Size = new System.Drawing.Size(417, 20);
            this.testExecutorButtonEdit.TabIndex = 31;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(3, 3);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(71, 13);
            this.labelControl6.TabIndex = 30;
            this.labelControl6.Text = "TestExecutor :";
            // 
            // specificVersionCheckEdit
            // 
            this.specificVersionCheckEdit.AutoSizeInLayoutControl = true;
            this.specificVersionCheckEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificVersionCheckEdit.Location = new System.Drawing.Point(3, 3);
            this.specificVersionCheckEdit.Name = "specificVersionCheckEdit";
            this.specificVersionCheckEdit.Properties.Caption = "When a new assembly is referenced use False Specific Version";
            this.specificVersionCheckEdit.Size = new System.Drawing.Size(516, 19);
            this.specificVersionCheckEdit.TabIndex = 33;
            // 
            // formatOnSaveCheckEdit
            // 
            this.formatOnSaveCheckEdit.AutoSizeInLayoutControl = true;
            this.formatOnSaveCheckEdit.Location = new System.Drawing.Point(3, 33);
            this.formatOnSaveCheckEdit.Name = "formatOnSaveCheckEdit";
            this.formatOnSaveCheckEdit.Properties.Caption = "Format On Save";
            this.formatOnSaveCheckEdit.Size = new System.Drawing.Size(105, 19);
            this.formatOnSaveCheckEdit.TabIndex = 32;
            // 
            // ModelEditor
            // 
            this.ModelEditor.Controls.Add(this.tableLayoutPanel3);
            this.ModelEditor.Controls.Add(this.labelControl11);
            this.ModelEditor.Location = new System.Drawing.Point(4, 22);
            this.ModelEditor.Name = "ModelEditor";
            this.ModelEditor.Size = new System.Drawing.Size(522, 462);
            this.ModelEditor.TabIndex = 5;
            this.ModelEditor.Text = "Model Editor";
            this.ModelEditor.UseVisualStyleBackColor = true;
            // 
            // checkEditDebugME
            // 
            this.checkEditDebugME.Location = new System.Drawing.Point(3, 30);
            this.checkEditDebugME.Name = "checkEditDebugME";
            this.checkEditDebugME.Properties.Caption = "Debug";
            this.checkEditDebugME.Size = new System.Drawing.Size(53, 19);
            this.checkEditDebugME.TabIndex = 33;
            // 
            // modelEditorPathButtonEdit
            // 
            this.modelEditorPathButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelEditorPathButtonEdit.Location = new System.Drawing.Point(124, 3);
            this.modelEditorPathButtonEdit.Name = "modelEditorPathButtonEdit";
            this.modelEditorPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.modelEditorPathButtonEdit.Size = new System.Drawing.Size(395, 20);
            this.modelEditorPathButtonEdit.TabIndex = 32;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(88, 13);
            this.labelControl1.TabIndex = 31;
            this.labelControl1.Text = "Model editor path:";
            // 
            // labelControl4
            // 
            this.labelControl4.AllowHtmlString = true;
            this.labelControl4.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl4.Location = new System.Drawing.Point(3, 3);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(516, 26);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "You can assign a keyboard shortcut to the <b>loadProjectFromReferenceItem</b> com" +
    "mand or use the context memu of the referenced to project assemblies ";
            // 
            // labelControl5
            // 
            this.labelControl5.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl5.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl5.Location = new System.Drawing.Point(3, 29);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelControl5.Size = new System.Drawing.Size(516, 36);
            this.labelControl5.TabIndex = 21;
            this.labelControl5.Text = "Below you can configure the location and the regex that will be used to indetify " +
    " projects. For example for Xpand projects you can use Xpand.*";
            // 
            // labelControl7
            // 
            this.labelControl7.AllowHtmlString = true;
            this.labelControl7.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl7.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl7.Location = new System.Drawing.Point(3, 3);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(516, 13);
            this.labelControl7.TabIndex = 21;
            this.labelControl7.Text = "Available command: <b>Dropdatabase</b>";
            // 
            // labelControl8
            // 
            this.labelControl8.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl8.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl8.Location = new System.Drawing.Point(3, 16);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
            this.labelControl8.Size = new System.Drawing.Size(516, 41);
            this.labelControl8.TabIndex = 22;
            this.labelControl8.Text = "Below you can configure which databases will be droped by adding the connectionst" +
    "ring names as defined in your configuration files (e.g. EasyTestConnectionString" +
    ")";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.41379F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.5862F));
            this.tableLayoutPanel1.Controls.Add(this.labelControl3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.publicTokenTextEdit, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.projectConverterPathButtonEdit, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelControl2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(522, 59);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // labelControl9
            // 
            this.labelControl9.AllowHtmlString = true;
            this.labelControl9.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl9.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl9.Location = new System.Drawing.Point(0, 0);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl9.Size = new System.Drawing.Size(522, 36);
            this.labelControl9.TabIndex = 22;
            this.labelControl9.Text = "You can assign a keyboard shortcut to the <b>ProjectConverter</b> command and pro" +
    "vide values to configure how the projectconverter will be executed. ";
            // 
            // labelControl10
            // 
            this.labelControl10.AllowHtmlString = true;
            this.labelControl10.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl10.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl10.Location = new System.Drawing.Point(0, 0);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl10.Size = new System.Drawing.Size(522, 36);
            this.labelControl10.TabIndex = 32;
            this.labelControl10.Text = "You can assign a keyboard shortcut to the <b>RunEasyTest, DebugEasyTest</b> comma" +
    "nds and configure the path of the standalone TestExecutor";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.15709F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.84291F));
            this.tableLayoutPanel2.Controls.Add(this.labelControl6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.testExecutorButtonEdit, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(522, 100);
            this.tableLayoutPanel2.TabIndex = 33;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.37165F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.62835F));
            this.tableLayoutPanel3.Controls.Add(this.labelControl12, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelControl1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkEditDebugME, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.modelEditorPathButtonEdit, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(522, 55);
            this.tableLayoutPanel3.TabIndex = 34;
            // 
            // labelControl11
            // 
            this.labelControl11.AllowHtmlString = true;
            this.labelControl11.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl11.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl11.Location = new System.Drawing.Point(0, 0);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl11.Size = new System.Drawing.Size(522, 36);
            this.labelControl11.TabIndex = 35;
            this.labelControl11.Text = "You can assign a keyboard shortcut to the <b>OpenModelEditor</b> command to open " +
    "the Models ToolWindow. You may need to manually open it in the first time from t" +
    "he CodeRush/Tools/Windows menu";
            // 
            // labelControl12
            // 
            this.labelControl12.AllowHtmlString = true;
            this.labelControl12.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl12.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl12.Location = new System.Drawing.Point(124, 30);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl12.Size = new System.Drawing.Size(395, 23);
            this.labelControl12.TabIndex = 36;
            this.labelControl12.Text = "Set to true so you can attach to the process when debugging is needed";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.specificVersionCheckEdit, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.formatOnSaveCheckEdit, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.labelControl13, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.labelControl14, 0, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(522, 462);
            this.tableLayoutPanel4.TabIndex = 34;
            // 
            // labelControl13
            // 
            this.labelControl13.AllowHtmlString = true;
            this.labelControl13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl13.Location = new System.Drawing.Point(3, 63);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(365, 13);
            this.labelControl13.TabIndex = 34;
            this.labelControl13.Text = "Assign a keyboard shortcut to the <b>collapseAllItemsInSolutionExplorer</b>";
            // 
            // labelControl14
            // 
            this.labelControl14.AllowHtmlString = true;
            this.labelControl14.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl14.Location = new System.Drawing.Point(3, 93);
            this.labelControl14.Name = "labelControl14";
            this.labelControl14.Size = new System.Drawing.Size(516, 26);
            this.labelControl14.TabIndex = 35;
            this.labelControl14.Text = "Assign a keyboard shortcut to the <b>collapseAllItemsInSolutionExplorer</b> to li" +
    "nk the XAF log with R# explore stack trace feature";
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.tabControl1);
            this.Name = "Options";
            this.Size = new System.Drawing.Size(530, 488);
            this.CommitChanges += new DevExpress.CodeRush.Core.OptionsPage.CommitChangesEventHandler(this.Options_CommitChanges);
            this.General.ResumeLayout(false);
            this.EasyTests.ResumeLayout(false);
            this.ProjectConverter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).EndInit();
            this.DropDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.LoadProject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).EndInit();
            this.ModelEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelEditorPathButtonEdit.Properties)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.TabPage EasyTests;
        private System.Windows.Forms.TabPage ProjectConverter;
        private ButtonEdit projectConverterPathButtonEdit;
        private LabelControl labelControl3;
        private LabelControl labelControl2;
        private ButtonEdit publicTokenTextEdit;
        private System.Windows.Forms.TabPage DropDatabase;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl1;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnName;
        private System.Windows.Forms.TabPage LoadProject;
        private System.Windows.Forms.Button button1;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl2;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnDirectory;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnPrefix;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.TabControl tabControl1;
        private CheckEdit specificVersionCheckEdit;
        private CheckEdit formatOnSaveCheckEdit;
        private ButtonEdit testExecutorButtonEdit;
        private LabelControl labelControl6;
        private System.Windows.Forms.TabPage ModelEditor;
        private CheckEdit checkEditDebugME;
        private ButtonEdit modelEditorPathButtonEdit;
        private LabelControl labelControl1;
        private LabelControl labelControl5;
        private LabelControl labelControl4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LabelControl labelControl10;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LabelControl labelControl9;
        private LabelControl labelControl8;
        private LabelControl labelControl7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private LabelControl labelControl11;
        private LabelControl labelControl12;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private LabelControl labelControl13;
        private LabelControl labelControl14;
    }
}