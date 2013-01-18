using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.Dashboard.Controllers {
    public partial class RefreshNavigationController : ViewController {
        bool refreshdashboards;

        public RefreshNavigationController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IDashboardDefinition);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ObjectSpace.Committed += ObjectSpace_Committed;
        }

        protected override void OnDeactivated() {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Committed -= ObjectSpace_Committed;
            base.OnDeactivated();
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (e.NewValue != e.OldValue)
                refreshdashboards = true;
        }

        void ObjectSpace_Committed(object sender, EventArgs e) {
            if (refreshdashboards)
                Frame.Application.MainWindow.GetController<DashboardNavigationController>().RecreateNavigationItems();
        }
    }
}