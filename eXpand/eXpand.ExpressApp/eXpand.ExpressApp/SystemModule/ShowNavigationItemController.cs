using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.SystemModule {
    public class ShowNavigationItemController : WindowController {
        public ShowNavigationItemController() {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated() {
            var controller = Frame.GetController<DevExpress.ExpressApp.SystemModule.ShowNavigationItemController>();
            if (controller != null)
                controller.CustomUpdateSelectedItem += controller_CustomUpdateSelectedItem;
        }

        protected override void OnDeactivating() {
            var controller = Frame.GetController<DevExpress.ExpressApp.SystemModule.ShowNavigationItemController>();
            if (controller != null)
                controller.CustomUpdateSelectedItem -= controller_CustomUpdateSelectedItem;
        }

        void controller_CustomUpdateSelectedItem(object sender, CustomUpdateSelectedItemEventArgs e) {
            var showNavigationItemController = ((DevExpress.ExpressApp.SystemModule.ShowNavigationItemController) sender);
            object data = showNavigationItemController.ShowNavigationItemAction.SelectedItem.Data;

            
            
            if (data is ViewShortcut )
            {
                ChoiceActionItem proposedSelectedItem =
                    showNavigationItemController.ShowNavigationItemAction.SelectedItem;
                e.ProposedSelectedItem = proposedSelectedItem;
                e.Handled = true;
            }
            return;
        }
    }
}