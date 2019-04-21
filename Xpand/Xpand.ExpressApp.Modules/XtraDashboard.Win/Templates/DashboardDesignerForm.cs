using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardWin;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Native;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.Persistent.Base;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Fasterflect;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.XtraDashboard.Win.Templates {
    public partial class DashboardDesignerForm : XtraForm {
        bool _saveDashboard;
        History _editHistory;
        IDashboardDefinition _template;
        private DashboardDesigner _dashboardDesigner;
        private BarButtonItem _barButtonItemSave;
        private BarButtonItem _barButtonItemSaveAndClose;

        public DevExpress.DashboardCommon.Dashboard Dashboard => _dashboardDesigner.Dashboard;
        public bool SaveDashboard => _saveDashboard;

        public DashboardDesignerForm() {
            InitializeComponent();
            _dashboardDesigner = new DashboardDesigner();
            KeyUp+=OnKeyUp;
            Controls.Add(_dashboardDesigner);
            _dashboardDesigner.Dock = DockStyle.Fill;
            _dashboardDesigner.CreateRibbon();
            _dashboardDesigner.ActionOnClose = DashboardActionOnClose.Discard;
            _editHistory = (History)Designer.GetPropertyValue("History");
        }

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs){
            if (keyEventArgs.Control&&keyEventArgs.KeyCode==Keys.Return)
                _barButtonItemSaveAndClose.PerformClick();
        }


        public DashboardDesigner Designer => _dashboardDesigner;

        public IDashboardDefinition Template => _template;

        public IObjectSpace ObjectSpace { get; set; }

        void _EditHistory_Changed(object sender, EventArgs e) {
            UpdateActionState();
        }

        protected override void OnClosed(EventArgs e) {
            _editHistory.Changed -= _EditHistory_Changed;
            _editHistory = null;
            _template = null;
            ObjectSpace = null;
            _dashboardDesigner.Dashboard.Dispose();
            _dashboardDesigner = null;
            base.OnClosed(e);
        }

        public void Save() {
            UpdateTemplateXml();
            UpdateActionState();
        }

        void UpdateTemplateXml(){
            Template.Xml = Designer.Dashboard.GetDashboardXml();
            _editHistory.IsModified = false;
        }

        void UpdateActionState() {
            HideButtons();
            _barButtonItemSave.Enabled = _editHistory.IsModified;
            _barButtonItemSaveAndClose.Enabled = _editHistory.IsModified;
        }

        void SaveAndClose(object sender, ItemClickEventArgs e) {
            Save();
            DialogResult = DialogResult.OK;
        }

        public void LoadTemplate(IDashboardDefinition dashboardDefinition,XafApplication application) {
            _template = dashboardDefinition;
            dashboardDefinition.GetDashboard(application, RuleMode.DesignTime,null,null,null,() => {});
            var dashboardCollectionDataSourceFillService =(IXpandDashboardDataSourceFillService)((XpandDashboardDataProvider) DashboardsModule.DataProvider)
                .AttachService(Designer.ServiceContainer, (IDashboardData) dashboardDefinition);
            dashboardCollectionDataSourceFillService.FillService.LoadBeforeParameters += (sender, args) =>
                args.Handled = new[]{RuleMode.Always, RuleMode.DesignTime}.Contains(dashboardDefinition.EditParameters);
            Designer.Dashboard = dashboardDefinition.GetDashboard(application, RuleMode.DesignTime,dashboardCollectionDataSourceFillService, Designer.DataSourceOptions,
                dashboard => Designer.DashboardChanged += (sender, args) => Designer.ShowDashboardParametersForm());
            _editHistory.Changed += _EditHistory_Changed;
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if (_dashboardDesigner.IsDashboardModified) {
                DialogResult result = XtraMessageBox.Show(LookAndFeel, this, "Do you want to save changes ?", "Dashboard Designer",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
                else
                    _saveDashboard = result == DialogResult.Yes;
            }
        }

        private TDashboardBarButtonItem GetBarItem<TDashboardBarButtonItem>() where TDashboardBarButtonItem : DashboardCommandBarButtonItem {
            return ((RibbonControl)_dashboardDesigner.MenuManager).Items.OfType<TDashboardBarButtonItem>().First();
        }

        void DashboardDesignerForm_Load(object sender, EventArgs e) {
            HideButtons();
            _barButtonItemSave = AddButton("Save", "MenuBar_Save_32x32.png",Keys.Control|Keys.S);
            _barButtonItemSave.ItemClick += BarButtonItemSaveOnItemClick;
            _barButtonItemSaveAndClose = AddButton("Save & Close", "MenuBar_SaveAndClose_32x32.png", Keys.ControlKey|Keys.Return);
            _barButtonItemSaveAndClose.ItemClick += SaveAndClose;
        }

        private void BarButtonItemSaveOnItemClick(object sender, ItemClickEventArgs itemClickEventArgs){
            Save();
        }

        private void HideButtons() {
            GetBarItem<FileNewBarItem>().Visibility = BarItemVisibility.Never;
            var fileSaveBarItem = GetBarItem<FileSaveBarItem>();
            fileSaveBarItem.Visibility = BarItemVisibility.Never;
            ((RibbonControl)_dashboardDesigner.MenuManager).Toolbar.ItemLinks.Remove(fileSaveBarItem);
            GetBarItem<FileSaveAsBarItem>().Visibility = BarItemVisibility.Never;
            GetBarItem<FileOpenBarItem>().Visibility = BarItemVisibility.Never;
        }

        private BarButtonItem AddButton(string button, string glyph,Keys keys) {
            var ribbonControl = ((RibbonControl)_dashboardDesigner.MenuManager);
            var ribbonPage = ribbonControl.Pages.First();
            var barButtonItem = new BarButtonItem(ribbonControl.Manager, button) {
                Enabled = false,
                Glyph = GetImage(glyph),ItemShortcut = new BarShortcut(keys)
            };
            ribbonPage.Groups[0].ItemLinks.Add(barButtonItem);
            return barButtonItem;
        }

        Image GetImage(string name) {
            var stream = GetType().Assembly.GetManifestResourceStream(GetType(), name);
            Debug.Assert(stream != null, "stream != null");
            return Image.FromStream(stream);
        }
    }
}
