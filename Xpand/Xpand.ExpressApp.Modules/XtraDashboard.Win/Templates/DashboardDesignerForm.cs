using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardWin;
using DevExpress.DashboardWin.Native;
using DevExpress.ExpressApp;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon;
using Fasterflect;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Filter;
using Xpand.ExpressApp.XtraDashboard.Win.Helpers;

namespace Xpand.ExpressApp.XtraDashboard.Win.Templates {
    public partial class DashboardDesignerForm : RibbonForm, IXPObjectSpaceAwareControl {
        bool _saveDashboard;
        History _editHistory;
        IObjectSpace _objectSpace;
        IDashboardDefinition _template;

        public DevExpress.DashboardCommon.Dashboard Dashboard { get { return dashboardDesigner.Dashboard; } }
        public bool SaveDashboard { get { return _saveDashboard; } }

        public DashboardDesignerForm() {
            InitializeComponent();
            dashboardDesigner.ActionOnClose = DashboardActionOnClose.Prompt;
            _editHistory = (History) Designer.GetPropertyValue("History");
        }

        public DashboardDesigner Designer {
            get { return dashboardDesigner; }
        }

        public IDashboardDefinition Template {
            get { return _template; }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public void UpdateDataSource(IObjectSpace objectSpace) {
            _objectSpace = objectSpace;
        }

        void _EditHistory_Changed(object sender, EventArgs e) {
            UpdateActionState();
        }

        protected override void OnClosed(EventArgs e) {
            _editHistory.Changed -= _EditHistory_Changed;
            _editHistory = null;
            _template = null;
            _objectSpace = null;
            dashboardDesigner.Dashboard.Dispose();
            dashboardDesigner = null;
            base.OnClosed(e);
        }

        void Save(object sender, ItemClickEventArgs e) {
            UpdateTemplateXml();
            UpdateActionState();
        }

        void UpdateTemplateXml() {
            using (var ms = new MemoryStream()){
                ObjectSpace.SaveDashboard(Designer.Dashboard,Template, ms);
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    string xml = sr.ReadToEnd();
                    Template.Xml = xml;
                    sr.Close();
                }
                ms.Close();
                _editHistory.IsModified = false;
            }
        }

        void UpdateActionState() {
            barButtonItemSave.Enabled = _editHistory.IsModified;
            barButtonItemSaveAndClose.Enabled = _editHistory.IsModified;
        }

        void SaveAndClose(object sender, ItemClickEventArgs e) {
            Save(null, null);
            DialogResult = DialogResult.OK;
        }

        public void LoadTemplate(IDashboardDefinition dashboardDefinition) {
            _template = dashboardDefinition;
            Designer.Dashboard = _template.CreateDashBoard(ObjectSpace);
            Designer.Dashboard.ApplyModel(FilterEnabled.DesignTime, _template, ObjectSpace);
            _editHistory.Changed += _EditHistory_Changed;
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if(dashboardDesigner.IsDashboardModified) {
                DialogResult result = XtraMessageBox.Show(LookAndFeel, this, "Do you want to save changes ?", "Dashboard Designer", 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if(result == DialogResult.Cancel)
                    e.Cancel = true;
                else
                    _saveDashboard = result == DialogResult.Yes;
            }
        }

        void DashboardDesignerForm_Load(object sender, EventArgs e) {
            fileNewBarItem1.Visibility =BarItemVisibility.Never;
            fileOpenBarItem1.Visibility=BarItemVisibility.Never;
            fileSaveBarItem1.Visibility=BarItemVisibility.Never;
            fileSaveAsBarItem1.Visibility=BarItemVisibility.Never;

            ribbonControl1.Toolbar.ItemLinks.Remove(fileSaveBarItem1);
            barButtonItemSave.Enabled = false;
            barButtonItemSave.Glyph = GetImage("MenuBar_Save_32x32.png");
            barButtonItemSave.ItemClick+=Save;
            barButtonItemSaveAndClose.ItemClick+=SaveAndClose;
            barButtonItemSaveAndClose.Enabled = false;
            barButtonItemSave.Glyph = GetImage("MenuBar_SaveAndClose_32x32.png");
        }

        Image GetImage(string name) {
            var stream = GetType().Assembly.GetManifestResourceStream(GetType(), name);
            Debug.Assert(stream != null, "stream != null");
            return Image.FromStream(stream);
        }
    }
}
