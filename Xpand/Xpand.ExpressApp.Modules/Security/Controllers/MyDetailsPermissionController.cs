using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Controllers {
    public class MyDetailsPermissionController : WindowController{
        MyDetailsController _myDetailsController;
        ShowNavigationItemController _showNavigationItemController;
        ChoiceActionItem _myDetailsItem;
        const string KeyDisable = "MyDetailsPermissionController";
        protected override void OnActivated() {
            base.OnActivated();
            if (!SecuritySystem.IsGranted(new IsAdministratorPermissionRequest())) {
                var isGranted = SecuritySystem.IsGranted(new MyDetailsOperationRequest(new MyDetailsPermission(Modifier.Allow)));
                
                _myDetailsController = Frame.GetController<MyDetailsController>();
                if (_myDetailsController != null) {
                    _myDetailsController.Active.SetItemValue(KeyDisable, !isGranted);
                }
                _showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
                if (_showNavigationItemController != null) {
                    _myDetailsItem = FindMyDetailsItem(_showNavigationItemController.ShowNavigationItemAction.Items);
                    if (_myDetailsItem != null) {
                        _myDetailsItem.Active.SetItemValue(KeyDisable, !isGranted);
                    }
                }
                
            }
            else {
                Active["IsAdmin"] = false;
            }
        }
        protected override void OnDeactivated() {
            if (_myDetailsController != null) {
                _myDetailsController.Active.RemoveItem(KeyDisable);
            }
            if (_myDetailsItem != null) {
                _myDetailsItem.Active.RemoveItem(KeyDisable);
            }
            base.OnDeactivated();
        }
        private ChoiceActionItem FindMyDetailsItem(IEnumerable<ChoiceActionItem> items) {
            foreach (ChoiceActionItem item in items) {
                if (item.Id == MyDetailsController.MyDetailsNavigationItemId)
                    return item;
                ChoiceActionItem t = FindMyDetailsItem(item.Items);
                if (t != null)
                    return t;
            }
            return null;
        }

    }

}
