using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Xpand.ExpressApp.XtraDashboard.Win.Templates;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public partial class DashboardDesignerController : ViewController {
        public DashboardDesignerController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IDashboardDefinition);
        }

        public event EventHandler<DashboardDesignerOpeningEventArgs> DashboardDesignerOpening;

        public SimpleAction DashboardEdit {
            get { return _dashboardEdit; }
        }

        public SimpleAction DashboardExportXml {
            get { return _dashboardExportXml; }
        }

        public SimpleAction DashboardImportXml {
            get { return _dashboardImportXml; }
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.SelectionChanged += ViewOnSelectionChanged;
            UpdateActionState();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.SelectionChanged -= ViewOnSelectionChanged;
            ResetActionsState();
        }

        private void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            UpdateActionState();
        }

        private void ResetActionsState(){
            _dashboardEdit.Active["SecurityIsGranted"] = true;
            _dashboardExportXml.Active["SecurityIsGranted"] = true;
            _dashboardImportXml.Active["SecurityIsGranted"] = true;
            var detailView = View as DetailView;
            if (detailView != null){
                _dashboardEdit.Active["ViewEditMode"] = true;
                _dashboardImportXml.Active["ViewEditMode"] = true;
            }
        }

        void UpdateActionState() {
            if (SecuritySystem.Instance is ISecurityComplex) {
                bool isGranted = true;
                foreach (object selectedObject in View.SelectedObjects) {
                    var clientPermissionRequest = new ClientPermissionRequest(typeof(IDashboardDefinition), "Xml", ObjectSpace.GetObjectHandle(selectedObject), SecurityOperations.Write);
                    isGranted = SecuritySystem.IsGranted(clientPermissionRequest);
                }
                _dashboardEdit.Active["SecurityIsGranted"] = isGranted;
                _dashboardExportXml.Active["SecurityIsGranted"] = isGranted;
                _dashboardImportXml.Active["SecurityIsGranted"] = isGranted;
            }
            var detailView = View as DetailView;
            if (detailView != null) {
                _dashboardEdit.Active["ViewEditMode"] = detailView.AllowEdit;
                _dashboardImportXml.Active["ViewEditMode"] = detailView.AllowEdit;
            }
        }

        void dashboardEdit_Execute(object sender, SimpleActionExecuteEventArgs e) {
            using (var form = new DashboardDesignerForm { ObjectSpace = ObjectSpace }) {
                form.LoadTemplate(((IDashboardDefinition)View.CurrentObject), Application);
                DashboardDesignerOpening?.Invoke(this, new DashboardDesignerOpeningEventArgs(form.Designer));
                form.ShowDialog();
                if (View is ListView)
                    ObjectSpace.CommitChanges();
            }
        }

        private void dashbardExportXML_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var def = (IDashboardDefinition)View.CurrentObject;

            if (!string.IsNullOrEmpty(def.Xml)) {
                var saveFileDialog = new SaveFileDialog {
                    AddExtension = true,
                    Filter = @"XML files (*.xml)|*.xml",
                    FileName = def.Name + ".xml"
                };
                if (saveFileDialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
                    var xdoc = new XmlDocument();
                    xdoc.LoadXml(def.Xml);
                    xdoc.Save(saveFileDialog.FileName);
                }
            }
        }

        private void dashboardImportXML_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var def = (IDashboardDefinition)View.CurrentObject;
            var openFileDialog = new OpenFileDialog {
                AddExtension = true,
                Filter = @"XML files (*.xml)|*.xml",
                FileName = def.Name + ".xml"
            };
            if (openFileDialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
                var xdoc = new XmlDocument();
                xdoc.Load(openFileDialog.FileName);

                def.Xml = GetXMLAsString(xdoc);

            }
        }
        private string GetXMLAsString(XmlDocument myxml) {
            var sw = new StringWriter();
            var tx = new XmlTextWriter(sw);
            myxml.WriteTo(tx);

            return sw.ToString();
        }
    }
}