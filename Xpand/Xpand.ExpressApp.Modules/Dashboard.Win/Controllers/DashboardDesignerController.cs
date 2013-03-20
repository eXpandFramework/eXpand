using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Win.Helpers;
using Xpand.ExpressApp.Dashboard.Win.Templates;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace Xpand.ExpressApp.Dashboard.Win.Controllers {
    public partial class DashboardDesignerController : ViewController {
        public DashboardDesignerController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IDashboardDefinition);
        }

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
            View.SelectionChanged += (s, e) => UpdateActionState();
            UpdateActionState();
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
        }

        void dashboardEdit_Execute(object sender, SimpleActionExecuteEventArgs e) {
            using (var form = new DashboardDesignerForm()) {
                new XPObjectSpaceAwareControlInitializer(form, Application);
                form.LoadTemplate(View.CurrentObject as IDashboardDefinition);
                form.ShowDialog();
            }
        }

        private void dashbardExportXML_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var def = (IDashboardDefinition)View.CurrentObject;

            if (!string.IsNullOrEmpty(def.Xml)) {
                var saveFileDialog = new SaveFileDialog {
                    AddExtension = true,
                    Filter = "XML files (*.xml)|*.xml",
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
                Filter = "XML files (*.xml)|*.xml",
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