using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.Security;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class RemoveNavigationItemsController : WindowController{
        protected override void OnActivated(){
            base.OnActivated();
            var basicUser = SecuritySystem.CurrentUser as ISimpleUser;
            if (basicUser != null && !basicUser.IsAdministrator){
                Frame.GetController<ShowNavigationItemController>(controller => {
                    for (var i = controller.ShowNavigationItemAction.Items.Count - 1; i >= 0; i--)
                    for (var j = controller.ShowNavigationItemAction.Items[i].Items.Count - 1; j >= 0; j--)
                        if (controller.ShowNavigationItemAction.Items[i].Items[j].Caption == "User Management")
                            controller.ShowNavigationItemAction.Items[i].Items.RemoveAt(j);
                });
            }
        }
    }
}