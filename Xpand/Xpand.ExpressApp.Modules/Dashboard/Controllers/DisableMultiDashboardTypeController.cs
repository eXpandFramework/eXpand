using System;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Dashboard.Controllers {
    public abstract class DisableMultiDashboardTypeController : ObjectViewController<DetailView, DashboardDefinition> {
        protected override void OnActivated() {
            base.OnActivated();
            View.GetItems<IChooseFromListCollectionEditor>().First().ControlCreated += OnControlCreated;
        }

        protected virtual void OnControlCreated(object sender, EventArgs e) {
            
        }
    }
}
