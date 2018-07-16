using System;
using System.IO;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.Dashboard.Controllers {
    public partial class DashboardDesignerController : ViewController {
        public DashboardDesignerController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IDashboardDefinition);
        }

        public event EventHandler<DashboardDesignerOpeningEventArgs> DashboardDesignerOpening;

        public SimpleAction DashboardEdit => _dashboardEdit;

        public SimpleAction DashboardExportXml => _dashboardExportXml;

        public SimpleAction DashboardImportXml => _dashboardImportXml;

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
            if (View is DetailView){
                _dashboardEdit.Active["ViewEditMode"] = true;
                _dashboardImportXml.Active["ViewEditMode"] = true;
            }
        }

        void UpdateActionState() {
            if (SecuritySystem.Instance is ISecurityComplex) {
                bool isGranted = true;
                foreach (object selectedObject in View.SelectedObjects) {
                    var clientPermissionRequest = new PermissionRequest(ObjectSpace, typeof(IDashboardDefinition), SecurityOperations.Write,selectedObject, "Xml");
                    isGranted = SecuritySystem.IsGranted(clientPermissionRequest);
                }
                _dashboardEdit.Active["SecurityIsGranted"] = isGranted;
                _dashboardExportXml.Active["SecurityIsGranted"] = isGranted;
                _dashboardImportXml.Active["SecurityIsGranted"] = isGranted;
            }

            if (View is DetailView detailView) {
                _dashboardEdit.Active["ViewEditMode"] = detailView.AllowEdit;
                _dashboardImportXml.Active["ViewEditMode"] = detailView.AllowEdit;
            }
        }

        protected string GetXMLAsString(XmlDocument myxml) {
            var sw = new StringWriter();
            var tx = new XmlTextWriter(sw);
            myxml.WriteTo(tx);

            return sw.ToString();
        }

        protected virtual void DashboardEditExecute(object sender, SimpleActionExecuteEventArgs e) {
        }

        protected virtual void DashbardExportXMLExecute(object sender, SimpleActionExecuteEventArgs e) {
        }

        protected virtual void DashboardImportXMLExecute(object sender, SimpleActionExecuteEventArgs e) {

        }

        protected virtual void OnDashboardDesignerOpening(DashboardDesignerOpeningEventArgs e){
            var handler = DashboardDesignerOpening;
            handler?.Invoke(this, e);
        }
    }
    public class DashboardDesignerOpeningEventArgs : EventArgs {
    }

}