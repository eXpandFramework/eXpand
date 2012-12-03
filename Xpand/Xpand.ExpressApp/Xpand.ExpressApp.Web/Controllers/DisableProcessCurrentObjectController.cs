using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.Controllers
{
    public class DisableProcessCurrentObjectController : ViewController
    {
        public DisableProcessCurrentObjectController()
        {
            TargetViewType = ViewType.ListView;
        }
        protected override void OnDeactivated()
        {
            if (IsMasterDetail) {
                Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active["HideViewElementsController"] = true;
                Frame.GetController<ListViewController>().EditAction.Active["DisableProcessCurrentObjectController"] = true;
            }
            base.OnDeactivated();
        }

        private bool IsMasterDetail {
            get {
                IModelListView listView = View.Model as IModelListView;
                if (listView != null)
                    return listView.MasterDetailMode == MasterDetailMode.ListViewAndDetailView;
                else
                    return false;
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (IsMasterDetail) {
                Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active["DisableProcessCurrentObjectController"] = false;
            }
        }
    }
}
