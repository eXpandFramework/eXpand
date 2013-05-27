using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDashboard;
using DevExpress.XtraDashboard.Native;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.XtraDashboard.Win.Helpers;

namespace Xpand.ExpressApp.XtraDashboard.Win.Templates {
    public partial class DashboardDesignerForm : RibbonForm, IXPObjectSpaceAwareControl {
        History _editHistory;
        IObjectSpace _objectSpace;
        IDashboardDefinition _template;

        public DashboardDesignerForm() {
            InitializeComponent();
            dashboardDesigner.ConfirmSaveOnClose = false;
            _editHistory = Designer.GetPrivatePropertyValue<History>("History");
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
            using (var ms = new MemoryStream()) {
                Designer.Dashboard.SaveToXml(ms);
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
            fileSaveBarItem.Enabled = _editHistory.IsModified;
        }

        void SaveAndClose(object sender, ItemClickEventArgs e) {
            Save(null, null);
            DialogResult = DialogResult.OK;
        }

        void Close(object sender, ItemClickEventArgs e) {
            if (dashboardDesigner.IsDashboardModified) {
                var result = XtraMessageBox.Show("Do you want to save changes?", "Dashboard Designer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    Save(null, null);
                    DialogResult = DialogResult.Yes;
                } else if (result == DialogResult.No)
                    DialogResult = DialogResult.Cancel;
            } else {
                DialogResult = DialogResult.Yes;
            }
        }

        public void LoadTemplate(IDashboardDefinition DashboardDefinition) {
            _template = DashboardDefinition;
            Designer.Dashboard = _template.CreateDashBoard(ObjectSpace, true);
            _editHistory.Changed += _EditHistory_Changed;
        }
    }
}