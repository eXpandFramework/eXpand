using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors;

namespace eXpandAddIns
{
    partial class Options
    {

        public Options()
        {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textEdit1 = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl1 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.buttonEdit2 = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.buttonEdit1 = new DevExpress.DXCore.Controls.XtraEditors.ButtonEdit();
            this.labelControl3 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.labelControl5 = new DevExpress.DXCore.Controls.XtraEditors.LabelControl();

            this.gridControl1 = new DevExpress.DXCore.Controls.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView();
            this.gridColumnName = new DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).BeginInit();

            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // textEdit1
            // 
            this.textEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textEdit1.Location = new System.Drawing.Point(123, 50);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(393, 22);
            this.textEdit1.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(8, 27);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(88, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Model editor path:";
            // 
            // buttonEdit2
            // 
            this.buttonEdit2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit2.Location = new System.Drawing.Point(123, 24);
            this.buttonEdit2.Name = "buttonEdit2";
            this.buttonEdit2.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.buttonEdit2.Size = new System.Drawing.Size(393, 22);
            this.buttonEdit2.TabIndex = 4;
            this.buttonEdit2.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(8, 53);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 13);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "PublicToken";
            // 
            // buttonEdit1
            // 
            this.buttonEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit1.Location = new System.Drawing.Point(122, 77);
            this.buttonEdit1.Name = "buttonEdit1";
            this.buttonEdit1.Properties.Buttons.AddRange(new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton[] {
            new DevExpress.DXCore.Controls.XtraEditors.Controls.EditorButton()});
            this.buttonEdit1.Size = new System.Drawing.Size(393, 22);
            this.buttonEdit1.TabIndex = 7;
            this.buttonEdit1.ButtonClick += new DevExpress.DXCore.Controls.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_ButtonClick_1);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(7, 80);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(90, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "ProjectConverter :";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog1";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(7, 108);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(109, 13);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "ConnectionStringName";
            // 
            // connectionStringName
            // 
            // 
            // gridControl1
            // 
            this.gridControl1.EmbeddedNavigator.Name = "";
            this.gridControl1.Location = new System.Drawing.Point(9, 133);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(507, 142);
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
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.labelControl5);

            this.Controls.Add(this.buttonEdit1);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.buttonEdit2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.textEdit1);
            this.Name = "Options";
            this.CommitChanges += new DevExpress.CodeRush.Core.OptionsPage.CommitChangesEventHandler(this.Options_CommitChanges);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).EndInit();

            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        ///
        /// Gets a DecoupledStorage instance for this options page.
        ///
        public static DecoupledStorage Storage
        {
            get
            {
                return CodeRush.Options.GetStorage(GetCategory(), GetPageName());
            }
        }
        ///
        /// Returns the category of this options page.
        ///
        public override string Category
        {
            get
            {
                return GetCategory();
            }
        }
        ///
        /// Returns the page name of this options page.
        ///
        public override string PageName
        {
            get
            {
                return GetPageName();
            }
        }
        ///
        /// Returns the full path (Category + PageName) of this options page.
        ///
        public static string FullPath
        {
            get
            {
                return GetCategory() + "\\" + GetPageName();
            }
        }

        ///
        /// Displays the DXCore options dialog and selects this page.
        ///
        public new static void Show()
        {
            CodeRush.Command.Execute("Options", FullPath);
        }
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private LabelControl labelControl1;
        private ButtonEdit buttonEdit2;
        private LabelControl labelControl2;
        private ButtonEdit textEdit1;
        private ButtonEdit buttonEdit1;
        private LabelControl labelControl3;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private LabelControl labelControl5;

        private DevExpress.DXCore.Controls.XtraGrid.GridControl gridControl1;
        private DevExpress.DXCore.Controls.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.DXCore.Controls.XtraGrid.Columns.GridColumn gridColumnName;
    }
}