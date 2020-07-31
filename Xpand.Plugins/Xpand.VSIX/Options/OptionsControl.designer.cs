using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace Xpand.VSIX.Options
{
    partial class OptionsControl {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components=new Container();


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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsControl));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.General = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.checkEditShowErrorsInMessageBox = new DevExpress.XtraEditors.CheckEdit();
            this.specificVersionCheckEdit = new DevExpress.XtraEditors.CheckEdit();
            this.EasyTests = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.testExecutorButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.ProjectConverter = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.publicTokenTextEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.projectConverterPathButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.DropDatabase = new System.Windows.Forms.TabPage();
            this.gridControlConnectionStrings = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.LoadProject = new System.Windows.Forms.TabPage();
            this.buttonSearchProjects = new System.Windows.Forms.Button();
            this.gridControlLoadProjectFromReferenceItem = new DevExpress.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnDirectory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPrefix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gridControlCmdBindings = new DevExpress.XtraGrid.GridControl();
            this.gridViewCmdBindings = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.ModelEditor = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.checkEditKillModelEditor = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.checkEditDebugME = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl15 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.AssemblyReferences = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gridControlAssemblyFolders = new DevExpress.XtraGrid.GridControl();
            this.gridView4 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl16 = new DevExpress.XtraEditors.LabelControl();
            this.Exceptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl17 = new DevExpress.XtraEditors.LabelControl();
            this.gridControlExceptions = new DevExpress.XtraGrid.GridControl();
            this.gridView5 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnBreak = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumnException = new DevExpress.XtraGrid.Columns.GridColumn();
            this.checkEditDisableExceptions = new DevExpress.XtraEditors.CheckEdit();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridControlExternal = new DevExpress.XtraGrid.GridControl();
            this.gridView6 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Arguments = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemCheckEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.General.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditShowErrorsInMessageBox.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).BeginInit();
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
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCmdBindings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmdBindings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit6)).BeginInit();
            this.ModelEditor.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditKillModelEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).BeginInit();
            this.AssemblyReferences.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAssemblyFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).BeginInit();
            this.Exceptions.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExceptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDisableExceptions.Properties)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit5)).BeginInit();
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
            this.General.Location = new System.Drawing.Point(4, 29);
            this.General.Name = "General";
            this.General.Size = new System.Drawing.Size(1359, 823);
            this.General.TabIndex = 4;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.checkEditShowErrorsInMessageBox, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.specificVersionCheckEdit, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1359, 823);
            this.tableLayoutPanel4.TabIndex = 34;
            // 
            // checkEditShowErrorsInMessageBox
            // 
            this.checkEditShowErrorsInMessageBox.AutoSizeInLayoutControl = true;
            this.checkEditShowErrorsInMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkEditShowErrorsInMessageBox.Location = new System.Drawing.Point(3, 33);
            this.checkEditShowErrorsInMessageBox.Name = "checkEditShowErrorsInMessageBox";
            this.checkEditShowErrorsInMessageBox.Properties.Caption = "Show Errors in MessageBox";
            this.checkEditShowErrorsInMessageBox.Size = new System.Drawing.Size(1353, 24);
            this.checkEditShowErrorsInMessageBox.TabIndex = 35;
            // 
            // specificVersionCheckEdit
            // 
            this.specificVersionCheckEdit.AutoSizeInLayoutControl = true;
            this.specificVersionCheckEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificVersionCheckEdit.Location = new System.Drawing.Point(3, 3);
            this.specificVersionCheckEdit.Name = "specificVersionCheckEdit";
            this.specificVersionCheckEdit.Properties.Caption = "When a new assembly is referenced use False Specific Version";
            this.specificVersionCheckEdit.Size = new System.Drawing.Size(1353, 24);
            this.specificVersionCheckEdit.TabIndex = 33;
            // 
            // EasyTests
            // 
            this.EasyTests.Controls.Add(this.tableLayoutPanel2);
            this.EasyTests.Controls.Add(this.labelControl10);
            this.EasyTests.Location = new System.Drawing.Point(4, 29);
            this.EasyTests.Name = "EasyTests";
            this.EasyTests.Size = new System.Drawing.Size(1359, 823);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 29);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1359, 100);
            this.tableLayoutPanel2.TabIndex = 33;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(3, 3);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(102, 19);
            this.labelControl6.TabIndex = 30;
            this.labelControl6.Text = "TestExecutor :";
            // 
            // testExecutorButtonEdit
            // 
            this.testExecutorButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testExecutorButtonEdit.Location = new System.Drawing.Point(263, 3);
            this.testExecutorButtonEdit.Name = "testExecutorButtonEdit";
            this.testExecutorButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.testExecutorButtonEdit.Size = new System.Drawing.Size(1093, 26);
            this.testExecutorButtonEdit.TabIndex = 31;
            this.testExecutorButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.testExecutorButtonEdit_ButtonClick_1);
            // 
            // labelControl10
            // 
            this.labelControl10.AllowHtmlString = true;
            this.labelControl10.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl10.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl10.Location = new System.Drawing.Point(0, 0);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl10.Size = new System.Drawing.Size(1359, 29);
            this.labelControl10.TabIndex = 32;
            this.labelControl10.Text = "You can assign a keyboard shortcut to the <b>RunEasyTest, DebugEasyTest</b> comma" +
    "nds and configure the path of the standalone TestExecutor. Leave the path blank " +
    "to auto detect it.";
            // 
            // ProjectConverter
            // 
            this.ProjectConverter.Controls.Add(this.tableLayoutPanel1);
            this.ProjectConverter.Controls.Add(this.labelControl9);
            this.ProjectConverter.Location = new System.Drawing.Point(4, 29);
            this.ProjectConverter.Name = "ProjectConverter";
            this.ProjectConverter.Size = new System.Drawing.Size(1359, 823);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 29);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1359, 59);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(3, 3);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(160, 19);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "ProjectConverter path:";
            // 
            // publicTokenTextEdit
            // 
            this.publicTokenTextEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.publicTokenTextEdit.Location = new System.Drawing.Point(307, 32);
            this.publicTokenTextEdit.Name = "publicTokenTextEdit";
            this.publicTokenTextEdit.Size = new System.Drawing.Size(1049, 26);
            this.publicTokenTextEdit.TabIndex = 8;
            // 
            // projectConverterPathButtonEdit
            // 
            this.projectConverterPathButtonEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectConverterPathButtonEdit.Location = new System.Drawing.Point(307, 3);
            this.projectConverterPathButtonEdit.Name = "projectConverterPathButtonEdit";
            this.projectConverterPathButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.projectConverterPathButtonEdit.Size = new System.Drawing.Size(1049, 26);
            this.projectConverterPathButtonEdit.TabIndex = 11;
            this.projectConverterPathButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.projectConverterPathButtonEdit_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(3, 32);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(86, 19);
            this.labelControl2.TabIndex = 9;
            this.labelControl2.Text = "PublicToken";
            // 
            // labelControl9
            // 
            this.labelControl9.AllowHtmlString = true;
            this.labelControl9.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl9.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl9.Location = new System.Drawing.Point(0, 0);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl9.Size = new System.Drawing.Size(1359, 29);
            this.labelControl9.TabIndex = 22;
            this.labelControl9.Text = "You can assign a keyboard shortcut to the <b>ProjectConverter</b> command and pro" +
    "vide values to configure how the projectconverter will be executed.  Leave blank" +
    " for auto detection";
            // 
            // DropDatabase
            // 
            this.DropDatabase.Controls.Add(this.gridControlConnectionStrings);
            this.DropDatabase.Controls.Add(this.labelControl8);
            this.DropDatabase.Controls.Add(this.labelControl7);
            this.DropDatabase.Location = new System.Drawing.Point(4, 29);
            this.DropDatabase.Name = "DropDatabase";
            this.DropDatabase.Padding = new System.Windows.Forms.Padding(3);
            this.DropDatabase.Size = new System.Drawing.Size(1359, 823);
            this.DropDatabase.TabIndex = 1;
            this.DropDatabase.Text = "Drop Database";
            this.DropDatabase.UseVisualStyleBackColor = true;
            // 
            // gridControlConnectionStrings
            // 
            this.gridControlConnectionStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlConnectionStrings.Location = new System.Drawing.Point(3, 56);
            this.gridControlConnectionStrings.MainView = this.gridView1;
            this.gridControlConnectionStrings.Name = "gridControlConnectionStrings";
            this.gridControlConnectionStrings.Size = new System.Drawing.Size(1353, 764);
            this.gridControlConnectionStrings.TabIndex = 13;
            this.gridControlConnectionStrings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
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
            this.gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnName
            // 
            this.gridColumnName.Caption = "ConnectionStrings";
            this.gridColumnName.FieldName = "Name";
            this.gridColumnName.Name = "gridColumnName";
            this.gridColumnName.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumnName.Visible = true;
            this.gridColumnName.VisibleIndex = 0;
            // 
            // labelControl8
            // 
            this.labelControl8.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl8.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl8.Location = new System.Drawing.Point(3, 22);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
            this.labelControl8.Size = new System.Drawing.Size(1353, 34);
            this.labelControl8.TabIndex = 22;
            this.labelControl8.Text = "Below you can configure which databases will be droped by adding the connectionst" +
    "ring names as defined in your configuration files (e.g. EasyTestConnectionString" +
    ")";
            // 
            // labelControl7
            // 
            this.labelControl7.AllowHtmlString = true;
            this.labelControl7.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl7.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl7.Location = new System.Drawing.Point(3, 3);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(1353, 19);
            this.labelControl7.TabIndex = 21;
            this.labelControl7.Text = "Available command: <b>DropDatabase</b>";
            // 
            // LoadProject
            // 
            this.LoadProject.Controls.Add(this.buttonSearchProjects);
            this.LoadProject.Controls.Add(this.gridControlLoadProjectFromReferenceItem);
            this.LoadProject.Controls.Add(this.labelControl5);
            this.LoadProject.Controls.Add(this.labelControl4);
            this.LoadProject.Location = new System.Drawing.Point(4, 29);
            this.LoadProject.Name = "LoadProject";
            this.LoadProject.Padding = new System.Windows.Forms.Padding(3);
            this.LoadProject.Size = new System.Drawing.Size(1359, 823);
            this.LoadProject.TabIndex = 0;
            this.LoadProject.Text = "Load referenced Projects ";
            this.LoadProject.UseVisualStyleBackColor = true;
            // 
            // buttonSearchProjects
            // 
            this.buttonSearchProjects.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonSearchProjects.Location = new System.Drawing.Point(3, 787);
            this.buttonSearchProjects.Name = "buttonSearchProjects";
            this.buttonSearchProjects.Size = new System.Drawing.Size(1353, 33);
            this.buttonSearchProjects.TabIndex = 18;
            this.buttonSearchProjects.Text = "Search Selected";
            this.buttonSearchProjects.UseVisualStyleBackColor = true;
            this.buttonSearchProjects.Click += new System.EventHandler(this.button1_Click);
            // 
            // gridControlLoadProjectFromReferenceItem
            // 
            this.gridControlLoadProjectFromReferenceItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLoadProjectFromReferenceItem.Location = new System.Drawing.Point(3, 51);
            this.gridControlLoadProjectFromReferenceItem.MainView = this.gridView2;
            this.gridControlLoadProjectFromReferenceItem.Name = "gridControlLoadProjectFromReferenceItem";
            this.gridControlLoadProjectFromReferenceItem.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlLoadProjectFromReferenceItem.Size = new System.Drawing.Size(1353, 769);
            this.gridControlLoadProjectFromReferenceItem.TabIndex = 17;
            this.gridControlLoadProjectFromReferenceItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
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
            this.gridView2.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnDirectory
            // 
            this.gridColumnDirectory.Caption = "Root Directory";
            this.gridColumnDirectory.FieldName = "RootPath";
            this.gridColumnDirectory.Name = "gridColumnDirectory";
            this.gridColumnDirectory.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
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
            this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl5.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl5.Location = new System.Drawing.Point(3, 22);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelControl5.Size = new System.Drawing.Size(1353, 29);
            this.labelControl5.TabIndex = 21;
            this.labelControl5.Text = "Below you can configure the location and the regex that will be used to indetify " +
    " projects. For example for Xpand projects you can use Xpand.*";
            // 
            // labelControl4
            // 
            this.labelControl4.AllowHtmlString = true;
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl4.Location = new System.Drawing.Point(3, 3);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(1353, 19);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "You can assign a keyboard shortcut to the <b>LoadProjectFromReferenceItem</b> com" +
    "mand or use the context memu of the referenced to project assemblies ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.LoadProject);
            this.tabControl1.Controls.Add(this.DropDatabase);
            this.tabControl1.Controls.Add(this.ProjectConverter);
            this.tabControl1.Controls.Add(this.EasyTests);
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.ModelEditor);
            this.tabControl1.Controls.Add(this.AssemblyReferences);
            this.tabControl1.Controls.Add(this.Exceptions);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1367, 856);
            this.tabControl1.TabIndex = 32;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gridControlCmdBindings);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1359, 823);
            this.tabPage2.TabIndex = 9;
            this.tabPage2.Text = "Command bindings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gridControlCmdBindings
            // 
            this.gridControlCmdBindings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlCmdBindings.Location = new System.Drawing.Point(0, 0);
            this.gridControlCmdBindings.MainView = this.gridViewCmdBindings;
            this.gridControlCmdBindings.Name = "gridControlCmdBindings";
            this.gridControlCmdBindings.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit6});
            this.gridControlCmdBindings.Size = new System.Drawing.Size(1359, 823);
            this.gridControlCmdBindings.TabIndex = 18;
            this.gridControlCmdBindings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCmdBindings});
            // 
            // gridViewCmdBindings
            // 
            this.gridViewCmdBindings.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn7,
            this.gridColumn8});
            this.gridViewCmdBindings.GridControl = this.gridControlCmdBindings;
            this.gridViewCmdBindings.Name = "gridViewCmdBindings";
            this.gridViewCmdBindings.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewCmdBindings.OptionsCustomization.AllowColumnResizing = false;
            this.gridViewCmdBindings.OptionsCustomization.AllowGroup = false;
            this.gridViewCmdBindings.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridViewCmdBindings.OptionsFilter.AllowFilterEditor = false;
            this.gridViewCmdBindings.OptionsFilter.AllowMRUFilterList = false;
            this.gridViewCmdBindings.OptionsMenu.EnableColumnMenu = false;
            this.gridViewCmdBindings.OptionsMenu.EnableFooterMenu = false;
            this.gridViewCmdBindings.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridViewCmdBindings.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Command";
            this.gridColumn7.FieldName = "Command";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 0;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Shortcut";
            this.gridColumn8.FieldName = "Shortcut";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 1;
            // 
            // repositoryItemCheckEdit6
            // 
            this.repositoryItemCheckEdit6.AutoHeight = false;
            this.repositoryItemCheckEdit6.Name = "repositoryItemCheckEdit6";
            // 
            // ModelEditor
            // 
            this.ModelEditor.Controls.Add(this.tableLayoutPanel3);
            this.ModelEditor.Controls.Add(this.labelControl11);
            this.ModelEditor.Location = new System.Drawing.Point(4, 29);
            this.ModelEditor.Name = "ModelEditor";
            this.ModelEditor.Size = new System.Drawing.Size(1359, 823);
            this.ModelEditor.TabIndex = 5;
            this.ModelEditor.Text = "Model Editor";
            this.ModelEditor.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 142F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.labelControl1, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.checkEditKillModelEditor, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelControl12, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkEditDebugME, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelControl15, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1359, 109);
            this.tableLayoutPanel3.TabIndex = 34;
            // 
            // checkEditKillModelEditor
            // 
            this.checkEditKillModelEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkEditKillModelEditor.Location = new System.Drawing.Point(3, 38);
            this.checkEditKillModelEditor.Name = "checkEditKillModelEditor";
            this.checkEditKillModelEditor.Properties.Caption = "Kill b4 build";
            this.checkEditKillModelEditor.Size = new System.Drawing.Size(136, 27);
            this.checkEditKillModelEditor.TabIndex = 37;
            // 
            // labelControl12
            // 
            this.labelControl12.AllowHtmlString = true;
            this.labelControl12.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl12.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl12.Location = new System.Drawing.Point(145, 3);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl12.Size = new System.Drawing.Size(1259, 29);
            this.labelControl12.TabIndex = 36;
            this.labelControl12.Text = "Set to true so you can attach to the process when debugging is needed";
            // 
            // checkEditDebugME
            // 
            this.checkEditDebugME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkEditDebugME.Location = new System.Drawing.Point(3, 3);
            this.checkEditDebugME.Name = "checkEditDebugME";
            this.checkEditDebugME.Properties.Caption = "Debug";
            this.checkEditDebugME.Size = new System.Drawing.Size(136, 29);
            this.checkEditDebugME.TabIndex = 33;
            // 
            // labelControl15
            // 
            this.labelControl15.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl15.Location = new System.Drawing.Point(145, 38);
            this.labelControl15.Name = "labelControl15";
            this.labelControl15.Size = new System.Drawing.Size(1259, 19);
            this.labelControl15.TabIndex = 38;
            this.labelControl15.Text = "Before project build if ModelEditor instances are found there will be a prompt to" +
    " kill them to unloack all related assemblie ";
            // 
            // labelControl11
            // 
            this.labelControl11.AllowHtmlString = true;
            this.labelControl11.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl11.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl11.Location = new System.Drawing.Point(0, 0);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl11.Size = new System.Drawing.Size(1359, 48);
            this.labelControl11.TabIndex = 35;
            this.labelControl11.Text = "You can assign a keyboard shortcut to the <b>OpenModelEditor</b> command to open " +
    "the Models ToolWindow. You may need to manually open it in the first time from t" +
    "he CodeRush/Tools/Windows menu";
            // 
            // AssemblyReferences
            // 
            this.AssemblyReferences.Controls.Add(this.tableLayoutPanel5);
            this.AssemblyReferences.Location = new System.Drawing.Point(4, 29);
            this.AssemblyReferences.Name = "AssemblyReferences";
            this.AssemblyReferences.Size = new System.Drawing.Size(1359, 823);
            this.AssemblyReferences.TabIndex = 6;
            this.AssemblyReferences.Text = "Assembly references";
            this.AssemblyReferences.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1359, 823);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // gridControlAssemblyFolders
            // 
            this.gridControlAssemblyFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlAssemblyFolders.Location = new System.Drawing.Point(3, 57);
            this.gridControlAssemblyFolders.MainView = this.gridView4;
            this.gridControlAssemblyFolders.Name = "gridControlAssemblyFolders";
            this.gridControlAssemblyFolders.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit3});
            this.gridControlAssemblyFolders.Size = new System.Drawing.Size(1353, 763);
            this.gridControlAssemblyFolders.TabIndex = 46;
            this.gridControlAssemblyFolders.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView4});
            // 
            // gridView4
            // 
            this.gridView4.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
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
            this.gridView4.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Assembly folders";
            this.gridColumn3.FieldName = "Folder";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
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
            this.labelControl16.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl16.Location = new System.Drawing.Point(3, 3);
            this.labelControl16.Name = "labelControl16";
            this.labelControl16.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl16.Size = new System.Drawing.Size(1353, 48);
            this.labelControl16.TabIndex = 45;
            this.labelControl16.Text = resources.GetString("labelControl16.Text");
            // 
            // Exceptions
            // 
            this.Exceptions.Controls.Add(this.tableLayoutPanel6);
            this.Exceptions.Location = new System.Drawing.Point(4, 29);
            this.Exceptions.Name = "Exceptions";
            this.Exceptions.Padding = new System.Windows.Forms.Padding(3);
            this.Exceptions.Size = new System.Drawing.Size(1359, 823);
            this.Exceptions.TabIndex = 7;
            this.Exceptions.Text = "Exceptions";
            this.Exceptions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.labelControl17, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.gridControlExceptions, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.checkEditDisableExceptions, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1353, 817);
            this.tableLayoutPanel6.TabIndex = 48;
            // 
            // labelControl17
            // 
            this.labelControl17.AllowHtmlString = true;
            this.labelControl17.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl17.Location = new System.Drawing.Point(3, 3);
            this.labelControl17.Name = "labelControl17";
            this.labelControl17.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelControl17.Size = new System.Drawing.Size(1347, 29);
            this.labelControl17.TabIndex = 46;
            this.labelControl17.Text = "List the exceptions you do not want the debugger to break";
            // 
            // gridControlExceptions
            // 
            this.gridControlExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlExceptions.Location = new System.Drawing.Point(3, 69);
            this.gridControlExceptions.MainView = this.gridView5;
            this.gridControlExceptions.Name = "gridControlExceptions";
            this.gridControlExceptions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit4});
            this.gridControlExceptions.Size = new System.Drawing.Size(1347, 745);
            this.gridControlExceptions.TabIndex = 14;
            this.gridControlExceptions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView5});
            // 
            // gridView5
            // 
            this.gridView5.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnBreak,
            this.gridColumnException});
            this.gridView5.GridControl = this.gridControlExceptions;
            this.gridView5.Name = "gridView5";
            this.gridView5.OptionsCustomization.AllowColumnMoving = false;
            this.gridView5.OptionsCustomization.AllowColumnResizing = false;
            this.gridView5.OptionsCustomization.AllowGroup = false;
            this.gridView5.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView5.OptionsFilter.AllowFilterEditor = false;
            this.gridView5.OptionsFilter.AllowMRUFilterList = false;
            this.gridView5.OptionsMenu.EnableColumnMenu = false;
            this.gridView5.OptionsMenu.EnableFooterMenu = false;
            this.gridView5.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView5.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView5.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnBreak
            // 
            this.gridColumnBreak.Caption = "Break";
            this.gridColumnBreak.ColumnEdit = this.repositoryItemCheckEdit4;
            this.gridColumnBreak.FieldName = "Break";
            this.gridColumnBreak.Name = "gridColumnBreak";
            this.gridColumnBreak.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.gridColumnBreak.Visible = true;
            this.gridColumnBreak.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit4
            // 
            this.repositoryItemCheckEdit4.AutoHeight = false;
            this.repositoryItemCheckEdit4.Name = "repositoryItemCheckEdit4";
            // 
            // gridColumnException
            // 
            this.gridColumnException.Caption = "Exception";
            this.gridColumnException.FieldName = "Exception";
            this.gridColumnException.Name = "gridColumnException";
            this.gridColumnException.Visible = true;
            this.gridColumnException.VisibleIndex = 1;
            // 
            // checkEditDisableExceptions
            // 
            this.checkEditDisableExceptions.Location = new System.Drawing.Point(3, 38);
            this.checkEditDisableExceptions.Name = "checkEditDisableExceptions";
            this.checkEditDisableExceptions.Properties.Caption = "Disabled";
            this.checkEditDisableExceptions.Size = new System.Drawing.Size(111, 27);
            this.checkEditDisableExceptions.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridControlExternal);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1359, 823);
            this.tabPage1.TabIndex = 8;
            this.tabPage1.Text = "External";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridControlExternal
            // 
            this.gridControlExternal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlExternal.Location = new System.Drawing.Point(3, 3);
            this.gridControlExternal.MainView = this.gridView6;
            this.gridControlExternal.Name = "gridControlExternal";
            this.gridControlExternal.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit5,
            this.repositoryItemComboBox1});
            this.gridControlExternal.Size = new System.Drawing.Size(1353, 817);
            this.gridControlExternal.TabIndex = 18;
            this.gridControlExternal.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView6});
            // 
            // gridView6
            // 
            this.gridView6.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5,
            this.gridColumn4,
            this.Arguments,
            this.gridColumn6});
            this.gridView6.GridControl = this.gridControlExternal;
            this.gridView6.Name = "gridView6";
            this.gridView6.OptionsCustomization.AllowColumnMoving = false;
            this.gridView6.OptionsCustomization.AllowColumnResizing = false;
            this.gridView6.OptionsCustomization.AllowGroup = false;
            this.gridView6.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gridView6.OptionsFilter.AllowFilterEditor = false;
            this.gridView6.OptionsFilter.AllowMRUFilterList = false;
            this.gridView6.OptionsMenu.EnableColumnMenu = false;
            this.gridView6.OptionsMenu.EnableFooterMenu = false;
            this.gridView6.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView6.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.gridView6.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "SolutionRegex";
            this.gridColumn5.FieldName = "SolutionRegex";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Path";
            this.gridColumn4.FieldName = "Path";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            // 
            // Arguments
            // 
            this.Arguments.Caption = "Arguments";
            this.Arguments.FieldName = "Arguments";
            this.Arguments.Name = "Arguments";
            this.Arguments.Visible = true;
            this.Arguments.VisibleIndex = 2;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Event";
            this.gridColumn6.ColumnEdit = this.repositoryItemComboBox1;
            this.gridColumn6.FieldName = "DTEEvent";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // repositoryItemCheckEdit5
            // 
            this.repositoryItemCheckEdit5.AutoHeight = false;
            this.repositoryItemCheckEdit5.Name = "repositoryItemCheckEdit5";
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl1.Location = new System.Drawing.Point(145, 71);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(1259, 19);
            this.labelControl1.TabIndex = 40;
            this.labelControl1.Text = "Set the XpandModelEditorAppConfigPath Enviromental variable to apply the configur" +
    "ation to the XpandModelEditor instance";
            // 
            // OptionsControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(1367, 856);
            this.General.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditShowErrorsInMessageBox.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.specificVersionCheckEdit.Properties)).EndInit();
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
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCmdBindings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmdBindings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit6)).EndInit();
            this.ModelEditor.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditKillModelEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDebugME.Properties)).EndInit();
            this.AssemblyReferences.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAssemblyFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).EndInit();
            this.Exceptions.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExceptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditDisableExceptions.Properties)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExternal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.TabPage EasyTests;
        private System.Windows.Forms.TabPage ProjectConverter;
        private ButtonEdit projectConverterPathButtonEdit;
        private LabelControl labelControl3;
        private LabelControl labelControl2;
        private ButtonEdit publicTokenTextEdit;
        private System.Windows.Forms.TabPage DropDatabase;
        private GridControl gridControlConnectionStrings;
        private GridView gridView1;
        private GridColumn gridColumnName;
        private System.Windows.Forms.TabPage LoadProject;
        private System.Windows.Forms.Button buttonSearchProjects;
        private GridControl gridControlLoadProjectFromReferenceItem;
        private GridView gridView2;
        private GridColumn gridColumnDirectory;
        private GridColumn gridColumnPrefix;
        private GridColumn gridColumn1;
        private RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.TabControl tabControl1;
        private CheckEdit specificVersionCheckEdit;
        private ButtonEdit testExecutorButtonEdit;
        private LabelControl labelControl6;
        private System.Windows.Forms.TabPage ModelEditor;
        private CheckEdit checkEditDebugME;
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
        private CheckEdit checkEditKillModelEditor;
        private LabelControl labelControl15;
        private System.Windows.Forms.TabPage AssemblyReferences;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private LabelControl labelControl16;
        private GridControl gridControlAssemblyFolders;
        private GridView gridView4;
        private GridColumn gridColumn3;
        private RepositoryItemCheckEdit repositoryItemCheckEdit3;
        private System.Windows.Forms.TabPage Exceptions;
        private GridControl gridControlExceptions;
        private GridView gridView5;
        private GridColumn gridColumnBreak;
        private RepositoryItemCheckEdit repositoryItemCheckEdit4;
        private GridColumn gridColumnException;
        private LabelControl labelControl17;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private CheckEdit checkEditDisableExceptions;
        private System.Windows.Forms.TabPage tabPage1;
        private GridControl gridControlExternal;
        private GridView gridView6;
        private GridColumn gridColumn5;
        private GridColumn gridColumn4;
        private GridColumn Arguments;
        private RepositoryItemCheckEdit repositoryItemCheckEdit5;
        private GridColumn gridColumn6;
        private RepositoryItemComboBox repositoryItemComboBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private GridControl gridControlCmdBindings;
        private GridView gridViewCmdBindings;
        private GridColumn gridColumn7;
        private GridColumn gridColumn8;
        private RepositoryItemCheckEdit repositoryItemCheckEdit6;
        private CheckEdit checkEditShowErrorsInMessageBox;
        private LabelControl labelControl1;
    }
}