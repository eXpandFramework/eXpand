using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraNavBar;
using Xpand.ExpressApp.Win.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class WinSelectFirstNavigationItemController : WindowController {
        private ShowNavigationItemController showNavigationItemController;
        private bool isLocked = true;
        private SingleChoiceAction navigationAction;
        public WinSelectFirstNavigationItemController() {
            TargetWindowType = WindowType.Main;
        }
        protected NavigationActionContainer FindNavigationActionContainer() {
            return Window.Template.GetContainers().OfType<NavigationActionContainer>().FirstOrDefault();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.TemplateChanged += Frame_TemplateChanged;
            showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            navigationAction = showNavigationItemController.ShowNavigationItemAction;
            if (showNavigationItemController != null) {
                showNavigationItemController.ShowNavigationItemAction.SelectedItemChanged += ShowNavigationItemAction_SelectedItemChanged;
            }
        }
        protected override void OnDeactivating() {
            Frame.TemplateChanged -= Frame_TemplateChanged;
            if (showNavigationItemController != null) {
                showNavigationItemController.ShowNavigationItemAction.SelectedItemChanged -= ShowNavigationItemAction_SelectedItemChanged;
                showNavigationItemController = null;
            }
            base.OnDeactivating();
        }
        private void ShowNavigationItemAction_SelectedItemChanged(object sender, EventArgs e) {
            isLocked = true;
        }
        private void Frame_TemplateChanged(object sender, EventArgs e) {
            NavigationActionContainer result = FindNavigationActionContainer();
            if (result != null) {
                SetupNavigationControl(result);
            }
        }
        protected void SetupNavigationControl(NavigationActionContainer navigationActionContainer) {
            var navBar = navigationActionContainer.NavigationControl as NavBarNavigationControl;
            if (navBar != null) {
                navBar.ActiveGroupChanged += navBar_ActiveGroupChanged;
                navBar.Click += navBar_Click;
            }
        }
        private void navBar_Click(object sender, EventArgs e) {
            isLocked = false;
        }
        private void navBar_ActiveGroupChanged(object sender, NavBarGroupEventArgs e) {
            if (!isLocked) {
                isLocked = true;
                AutoSelectFirstItemInGroup(((NavBarNavigationControl)e.Group.NavBar).GroupToActionItemWrapperMap[e.Group].ActionItem);
            }
        }
        protected void AutoSelectFirstItemInGroup(ChoiceActionItem navGroupItem) {
            if (!CanAutoSelectFirstItemInGroup()) return;
            foreach (ChoiceActionItem item in navGroupItem.Items) {
                if (item.Enabled.ResultValue && item.Active.ResultValue) {
                    navigationAction.DoExecute(item);
                    return;
                }
            }
        }
        protected bool CanAutoSelectFirstItemInGroup() {
            return ((IModelRootNavigationItemsAutoSelectedGroupItem)((IModelApplicationNavigationItems)Application.Model).NavigationItems).AutoSelectFirstItemInGroup;
        }
    }
}