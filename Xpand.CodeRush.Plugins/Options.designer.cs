using System.ComponentModel;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors;

namespace Xpand.CodeRush.Plugins
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.General = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.specificVersionCheckEdit = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.formatOnSaveCheckEdit = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.labelControl13 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl14 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.EasyTests = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl6 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.testExecutorButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl10 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.ProjectConverter = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl3 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.publicTokenTextEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.projectConverterPathButtonEdit = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.DropDatabase = new System.Windows.Forms.TabPage();
            this.gridControlConnectionStrings = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnName = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.labelControl8 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.LoadProject = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.gridControlLoadProjectFromReferenceItem = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnDirectory = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumnPrefix = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl5 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ModelEditor = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.gridControlME = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView3 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.checkEditKillModelEditor = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.labelControl12 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.checkEditDebugME = new DevExpress.DXCore.Controls.XtraEditors.CheckEdit();
            this.labelControl15 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.labelControl11 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gridControlAssemblyFolders = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView4 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit3 = new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl16 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.General.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).BeginInit();
            this.EasyTests.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).BeginInit();
            this.ProjectConverter.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).BeginInit();
            this.DropDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlConnectionStrings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.LoadProject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLoadProjectFromReferenceItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.ModelEditor.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlME)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditKillModelEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAssemblyFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).BeginInit();
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
            this.General.Size = new System.Drawing.Size(676, 468);
            this.General.TabIndex = 4;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel4.Size = new System.Drawing.Size(676, 468);
            this.tableLayoutPanel4.TabIndex = 34;
            // 
            // specificVersionCheckEdit
            // 
            this.specificVersionCheckEdit.AutoSizeInLayoutControl = true;
            this.specificVersionCheckEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificVersionCheckEdit.Location = new System.Drawing.Point(3, 3);
            this.specificVersionCheckEdit.Name = "specificVersionCheckEdit";
            this.specificVersionCheckEdit.Properties.Caption = "When a new assembly is referenced use False Specific Version";
            this.specificVersionCheckEdit.Size = new System.Drawing.Size(670, 19);
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
            // labelControl13
            // 
            this.labelControl13.AllowHtmlString = true;
            this.labelControl13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl13.Location = new System.Drawing.Point(3, 63);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(366, 13);
            this.labelControl13.TabIndex = 34;
            this.labelControl13.Text = "Assign a keyboard shortcut to the <b>CollapseAllItemsInSolutionExplorer</b>";
            // 
            // labelControl14
            // 
            this.labelControl14.AllowHtmlString = true;
            this.labelControl14.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl14.Location = new System.Drawing.Point(3, 93);
            this.labelControl14.Name = "labelControl14";
            this.labelControl14.Size = new System.Drawing.Size(670, 13);
            this.labelControl14.TabIndex = 35;
            this.labelControl14.Text = "Assign a keyboard shortcut to the <b>ExploreXafErrors</b> to link the XAF log wit" +
    "h R# explore stack trace feature";
            // 
            // EasyTests
            // 
            this.EasyTests.Controls.Add(this.tableLayoutPanel2);
            this.EasyTests.Controls.Add(this.labelControl10);
            this.EasyTests.Location = new System.Drawing.Point(4, 22);
            this.EasyTests.Name = "EasyTests";
            this.EasyTests.Size = new System.Drawing.Size(676, 468);
            this.EasyTests.TabIndex = 3;
            this.EasyTests.Text = "EasyTests";
            this.EasyTests.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel2.Size = new System.Drawing.Size(676, 100);
            this.tableLayoutPanel2.TabIndex = 33;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(3, 3);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(71, 13);
            this.labelControl6.TabIndex = 30;
            this.labelControl6.Text = "TestExecutor :";
            // 
            // testExecutorButtonEdit
            // 
            this.testExecutorButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testExecutorButtonEdit.Location = new System.Drawing.Point(132, 3);
            this.testExecutorButtonEdit.Name = "testExecutorButtonEdit";
            this.testExecutorButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.testExecutorButtonEdit.Size = new System.Drawing.Size(541, 20);
            this.testExecutorButtonEdit.TabIndex = 31;
            this.testExecutorButtonEdit.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.testExecutorButtonEdit_ButtonClick_1);
            // 
            // labelControl10
            // 
            this.labelControl10.AllowHtmlString = true;
            this.labelControl10.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl10.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl10.Location = new System.Drawing.Point(0, 0);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl10.Size = new System.Drawing.Size(676, 36);
            this.labelControl10.TabIndex = 32;
            this.labelControl10.Text = "You can assign a keyboard shortcut to the <b>RunEasyTest, DebugEasyTest</b> comma" +
    "nds and configure the path of the standalone TestExecutor. Leave the path blank " +
    "to auto detect it.";
            // 
            // ProjectConverter
            // 
            this.ProjectConverter.Controls.Add(this.tableLayoutPanel1);
            this.ProjectConverter.Controls.Add(this.labelControl9);
            this.ProjectConverter.Location = new System.Drawing.Point(4, 22);
            this.ProjectConverter.Name = "ProjectConverter";
            this.ProjectConverter.Size = new System.Drawing.Size(676, 468);
            this.ProjectConverter.TabIndex = 2;
            this.ProjectConverter.Text = "Project Converter";
            this.ProjectConverter.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(676, 59);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(3, 3);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(112, 13);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "ProjectConverter path:";
            // 
            // publicTokenTextEdit
            // 
            this.publicTokenTextEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.publicTokenTextEdit.Location = new System.Drawing.Point(154, 32);
            this.publicTokenTextEdit.Name = "publicTokenTextEdit";
            this.publicTokenTextEdit.Size = new System.Drawing.Size(519, 20);
            this.publicTokenTextEdit.TabIndex = 8;
            // 
            // projectConverterPathButtonEdit
            // 
            this.projectConverterPathButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectConverterPathButtonEdit.Location = new System.Drawing.Point(154, 3);
            this.projectConverterPathButtonEdit.Name = "projectConverterPathButtonEdit";
            this.projectConverterPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.projectConverterPathButtonEdit.Size = new System.Drawing.Size(519, 20);
            this.projectConverterPathButtonEdit.TabIndex = 11;
            this.projectConverterPathButtonEdit.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.projectConverterPathButtonEdit_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(3, 32);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 13);
            this.labelControl2.TabIndex = 9;
            this.labelControl2.Text = "PublicToken";
            // 
            // labelControl9
            // 
            this.labelControl9.AllowHtmlString = true;
            this.labelControl9.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl9.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl9.Location = new System.Drawing.Point(0, 0);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl9.Size = new System.Drawing.Size(676, 36);
            this.labelControl9.TabIndex = 22;
            this.labelControl9.Text = "You can assign a keyboard shortcut to the <b>ProjectConverter</b> command and pro" +
    "vide values to configure how the projectconverter will be executed. ";
            // 
            // DropDatabase
            // 
            this.DropDatabase.Controls.Add(this.gridControlConnectionStrings);
            this.DropDatabase.Controls.Add(this.labelControl8);
            this.DropDatabase.Controls.Add(this.labelControl7);
            this.DropDatabase.Location = new System.Drawing.Point(4, 22);
            this.DropDatabase.Name = "DropDatabase";
            this.DropDatabase.Padding = new System.Windows.Forms.Padding(3);
            this.DropDatabase.Size = new System.Drawing.Size(676, 468);
            this.DropDatabase.TabIndex = 1;
            this.DropDatabase.Text = "Drop Database";
            this.DropDatabase.UseVisualStyleBackColor = true;
            // 
            // gridControlConnectionStrings
            // 
            this.gridControlConnectionStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlConnectionStrings.Location = new System.Drawing.Point(3, 57);
            this.gridControlConnectionStrings.MainView = this.gridView1;
            this.gridControlConnectionStrings.Name = "gridControlConnectionStrings";
            this.gridControlConnectionStrings.Size = new System.Drawing.Size(670, 408);
            this.gridControlConnectionStrings.TabIndex = 13;
            this.gridControlConnectionStrings.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumnName});
            this.gridView1.GridControl = this.gridControlConnectionStrings;
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
            // labelControl8
            // 
            this.labelControl8.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl8.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl8.Location = new System.Drawing.Point(3, 16);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
            this.labelControl8.Size = new System.Drawing.Size(670, 41);
            this.labelControl8.TabIndex = 22;
            this.labelControl8.Text = "Below you can configure which databases will be droped by adding the connectionst" +
    "ring names as defined in your configuration files (e.g. EasyTestConnectionString" +
    ")";
            // 
            // labelControl7
            // 
            this.labelControl7.AllowHtmlString = true;
            this.labelControl7.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl7.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl7.Location = new System.Drawing.Point(3, 3);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(670, 13);
            this.labelControl7.TabIndex = 21;
            this.labelControl7.Text = "Available command: <b>DropDatabase</b>";
            // 
            // LoadProject
            // 
            this.LoadProject.Controls.Add(this.button1);
            this.LoadProject.Controls.Add(this.gridControlLoadProjectFromReferenceItem);
            this.LoadProject.Controls.Add(this.labelControl5);
            this.LoadProject.Controls.Add(this.labelControl4);
            this.LoadProject.Location = new System.Drawing.Point(4, 22);
            this.LoadProject.Name = "LoadProject";
            this.LoadProject.Padding = new System.Windows.Forms.Padding(3);
            this.LoadProject.Size = new System.Drawing.Size(676, 468);
            this.LoadProject.TabIndex = 0;
            this.LoadProject.Text = "Load referenced Projects ";
            this.LoadProject.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(3, 442);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(670, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Search Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gridControlLoadProjectFromReferenceItem
            // 
            this.gridControlLoadProjectFromReferenceItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLoadProjectFromReferenceItem.Location = new System.Drawing.Point(3, 65);
            this.gridControlLoadProjectFromReferenceItem.MainView = this.gridView2;
            this.gridControlLoadProjectFromReferenceItem.Name = "gridControlLoadProjectFromReferenceItem";
            this.gridControlLoadProjectFromReferenceItem.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlLoadProjectFromReferenceItem.Size = new System.Drawing.Size(670, 400);
            this.gridControlLoadProjectFromReferenceItem.TabIndex = 17;
            this.gridControlLoadProjectFromReferenceItem.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDirectory,
            this.gridColumnPrefix,
            this.gridColumn1});
            this.gridView2.GridControl = this.gridControlLoadProjectFromReferenceItem;
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
            // labelControl5
            // 
            this.labelControl5.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl5.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl5.Location = new System.Drawing.Point(3, 29);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelControl5.Size = new System.Drawing.Size(670, 36);
            this.labelControl5.TabIndex = 21;
            this.labelControl5.Text = "Below you can configure the location and the regex that will be used to indetify " +
    " projects. For example for Xpand projects you can use Xpand.*";
            // 
            // labelControl4
            // 
            this.labelControl4.AllowHtmlString = true;
            this.labelControl4.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl4.Location = new System.Drawing.Point(3, 3);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(670, 26);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "You can assign a keyboard shortcut to the <b>LoadProjectFromReferenceItem</b> com" +
    "mand or use the context memu of the referenced to project assemblies ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.LoadProject);
            this.tabControl1.Controls.Add(this.DropDatabase);
            this.tabControl1.Controls.Add(this.ProjectConverter);
            this.tabControl1.Controls.Add(this.EasyTests);
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.ModelEditor);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(684, 494);
            this.tabControl1.TabIndex = 32;
            // 
            // ModelEditor
            // 
            this.ModelEditor.Controls.Add(this.tableLayoutPanel3);
            this.ModelEditor.Controls.Add(this.labelControl11);
            this.ModelEditor.Location = new System.Drawing.Point(4, 22);
            this.ModelEditor.Name = "ModelEditor";
            this.ModelEditor.Size = new System.Drawing.Size(676, 468);
            this.ModelEditor.TabIndex = 5;
            this.ModelEditor.Text = "Model Editor";
            this.ModelEditor.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.37165F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.62835F));
            this.tableLayoutPanel3.Controls.Add(this.gridControlME, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkEditKillModelEditor, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelControl12, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelControl1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkEditDebugME, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelControl15, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.21828F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.78173F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(676, 232);
            this.tableLayoutPanel3.TabIndex = 34;
            // 
            // gridControlME
            // 
            this.gridControlME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlME.Location = new System.Drawing.Point(160, 3);
            this.gridControlME.MainView = this.gridView3;
            this.gridControlME.Name = "gridControlME";
            this.gridControlME.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit2});
            this.gridControlME.Size = new System.Drawing.Size(513, 154);
            this.gridControlME.TabIndex = 40;
            this.gridControlME.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView3});
            // 
            // gridView3
            // 
            this.gridView3.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2});
            this.gridView3.GridControl = this.gridControlME;
            this.gridView3.Name = "gridView3";
            this.gridView3.OptionsCustomization.AllowColumnMoving = false;
            this.gridView3.OptionsCustomization.AllowColumnResizing = false;
            this.gridView3.OptionsCustomization.AllowGroup = false;
            this.gridView3.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView3.OptionsFilter.AllowFilterEditor = false;
            this.gridView3.OptionsFilter.AllowMRUFilterList = false;
            this.gridView3.OptionsMenu.EnableColumnMenu = false;
            this.gridView3.OptionsMenu.EnableFooterMenu = false;
            this.gridView3.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView3.OptionsView.NewItemRowPosition = DevExpress.DXCore.Controls.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView3.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Model Editor Path";
            this.gridColumn2.FieldName = "Path";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // checkEditKillModelEditor
            // 
            this.checkEditKillModelEditor.Location = new System.Drawing.Point(3, 200);
            this.checkEditKillModelEditor.Name = "checkEditKillModelEditor";
            this.checkEditKillModelEditor.Properties.Caption = "Kill b4 build";
            this.checkEditKillModelEditor.Size = new System.Drawing.Size(88, 19);
            this.checkEditKillModelEditor.TabIndex = 37;
            // 
            // labelControl12
            // 
            this.labelControl12.AllowHtmlString = true;
            this.labelControl12.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl12.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl12.Location = new System.Drawing.Point(160, 163);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl12.Size = new System.Drawing.Size(513, 23);
            this.labelControl12.TabIndex = 36;
            this.labelControl12.Text = "Set to true so you can attach to the process when debugging is needed";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(88, 13);
            this.labelControl1.TabIndex = 31;
            this.labelControl1.Text = "Model editor path:";
            // 
            // checkEditDebugME
            // 
            this.checkEditDebugME.Location = new System.Drawing.Point(3, 163);
            this.checkEditDebugME.Name = "checkEditDebugME";
            this.checkEditDebugME.Properties.Caption = "Debug";
            this.checkEditDebugME.Size = new System.Drawing.Size(53, 19);
            this.checkEditDebugME.TabIndex = 33;
            // 
            // labelControl15
            // 
            this.labelControl15.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl15.Location = new System.Drawing.Point(160, 200);
            this.labelControl15.Name = "labelControl15";
            this.labelControl15.Size = new System.Drawing.Size(394, 26);
            this.labelControl15.TabIndex = 38;
            this.labelControl15.Text = "Before project build if ModelEditor instances are found there will be a prompt to" +
    " kill them to unloack all related assemblie ";
            // 
            // labelControl11
            // 
            this.labelControl11.AllowHtmlString = true;
            this.labelControl11.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl11.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl11.Location = new System.Drawing.Point(0, 0);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl11.Size = new System.Drawing.Size(676, 36);
            this.labelControl11.TabIndex = 35;
            this.labelControl11.Text = "You can assign a keyboard shortcut to the <b>OpenModelEditor</b> command to open " +
    "the Models ToolWindow. You may need to manually open it in the first time from t" +
    "he CodeRush/Tools/Windows menu";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(676, 468);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "Assembly references";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.gridControlAssemblyFolders, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.labelControl16, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(676, 468);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // gridControlAssemblyFolders
            // 
            this.gridControlAssemblyFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlAssemblyFolders.Location = new System.Drawing.Point(3, 58);
            this.gridControlAssemblyFolders.MainView = this.gridView4;
            this.gridControlAssemblyFolders.Name = "gridControlAssemblyFolders";
            this.gridControlAssemblyFolders.RepositoryItems.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit3});
            this.gridControlAssemblyFolders.Size = new System.Drawing.Size(670, 407);
            this.gridControlAssemblyFolders.TabIndex = 46;
            this.gridControlAssemblyFolders.ViewCollection.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Views.Base.BaseView[] {
            this.gridView4});
            // 
            // gridView4
            // 
            this.gridView4.Columns.AddRange(new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3});
            this.gridView4.GridControl = this.gridControlAssemblyFolders;
            this.gridView4.Name = "gridView4";
            this.gridView4.OptionsCustomization.AllowColumnMoving = false;
            this.gridView4.OptionsCustomization.AllowColumnResizing = false;
            this.gridView4.OptionsCustomization.AllowGroup = false;
            this.gridView4.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView4.OptionsFilter.AllowFilterEditor = false;
            this.gridView4.OptionsFilter.AllowMRUFilterList = false;
            this.gridView4.OptionsMenu.EnableColumnMenu = false;
            this.gridView4.OptionsMenu.EnableFooterMenu = false;
            this.gridView4.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView4.OptionsView.NewItemRowPosition = DevExpress.DXCore.Controls.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Assembly folders";
            this.gridColumn3.FieldName = "Folder";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsFilter.AutoFilterCondition = DevExpress.DXCore.Controls.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit3
            // 
            this.repositoryItemCheckEdit3.AutoHeight = false;
            this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
            // 
            // labelControl16
            // 
            this.labelControl16.AllowHtmlString = true;
            this.labelControl16.AutoSizeMode = DevExpress.DXCore.Controls.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl16.Location = new System.Drawing.Point(3, 3);
            this.labelControl16.Name = "labelControl16";
            this.labelControl16.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl16.Size = new System.Drawing.Size(670, 49);
            this.labelControl16.TabIndex = 45;
            this.labelControl16.Text = resources.GetString("labelControl16.Text");
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.tabControl1);
            this.Name = "Options";
            this.Size = new System.Drawing.Size(684, 494);
            this.CommitChanges += new DevExpress.CodeRush.Core.OptionsPage.CommitChangesEventHandler(this.Options_CommitChanges);
            this.General.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatOnSaveCheckEdit.Properties)).EndInit();
            this.EasyTests.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testExecutorButtonEdit.Properties)).EndInit();
            this.ProjectConverter.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.publicTokenTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectConverterPathButtonEdit.Properties)).EndInit();
            this.DropDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlConnectionStrings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.LoadProject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLoadProjectFromReferenceItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ModelEditor.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlME)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditKillModelEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAssemblyFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        ///
        /// Gets a DecoupledStorage instance for this options page.
        ///
        public static DecoupledStorage Storage {
            get {
                return DevExpress.CodeRush.Core.CodeRush.Options.GetStorage(GetCategory(), GetPageName());
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
            DevExpress.CodeRush.Core.CodeRush.Command.Execute("Options", FullPath);
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
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControlConnectionStrings;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnName;
        private System.Windows.Forms.TabPage LoadProject;
        private System.Windows.Forms.Button button1;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControlLoadProjectFromReferenceItem;
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
        private CheckEdit checkEditKillModelEditor;
        private LabelControl labelControl15;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControlME;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private LabelControl labelControl16;
        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControlAssemblyFolders;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView4;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.DXCore.Controls.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit3;
    }
}