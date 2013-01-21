using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Win.Helpers;
using Xpand.ExpressApp.Dashboard.Win.Templates;

namespace Xpand.ExpressApp.Dashboard.Win.Controllers {
    public partial class DashboardDesignerController : ViewController {
        public DashboardDesignerController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IDashboardDefinition);
        }

        public SimpleAction DashboardEditAction {
            get { return dashboardEdit; }
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
                dashboardEdit.Active["SecurityIsGranted"] = isGranted;
            }
        }

        void dashboardEdit_Execute(object sender, SimpleActionExecuteEventArgs e) {
            using (var form = new DashboardDesignerForm()) {
                new XPObjectSpaceAwareControlInitializer(form, Application);
                form.LoadTemplate(View.CurrentObject as IDashboardDefinition);
                form.ShowDialog();
            }
        }
    }
}