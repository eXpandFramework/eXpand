using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraNavBar;
using Xpand.ExpressApp.Win.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class SelectFirstNavigationItemController : WindowController {
        private ShowNavigationItemController _showNavigationItemController;
        private bool _isLocked = true;
        private SingleChoiceAction _navigationAction;
        public SelectFirstNavigationItemController() {
            TargetWindowType = WindowType.Main;
        }
        protected NavigationActionContainer FindNavigationActionContainer() {
            return Window.Template.GetContainers().OfType<NavigationActionContainer>().FirstOrDefault();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.TemplateChanged += Frame_TemplateChanged;
            _showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            _navigationAction = _showNavigationItemController.ShowNavigationItemAction;
            if (_showNavigationItemController != null) {
                _showNavigationItemController.ShowNavigationItemAction.SelectedItemChanged += ShowNavigationItemAction_SelectedItemChanged;
            }
        }
        protected override void OnDeactivated() {
            Frame.TemplateChanged -= Frame_TemplateChanged;
            if (_showNavigationItemController != null) {
                _showNavigationItemController.ShowNavigationItemAction.SelectedItemChanged -= ShowNavigationItemAction_SelectedItemChanged;
                _showNavigationItemController = null;
            }
            base.OnDeactivated();
        }
        private void ShowNavigationItemAction_SelectedItemChanged(object sender, EventArgs e) {
            _isLocked = true;
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
            _isLocked = false;
        }
        private void navBar_ActiveGroupChanged(object sender, NavBarGroupEventArgs e) {
            if (!_isLocked) {
                _isLocked = true;
                AutoSelectFirstItemInGroup(((NavBarNavigationControl)e.Group.NavBar).GroupToActionItemWrapperMap[e.Group].ActionItem);
            }
        }
        protected void AutoSelectFirstItemInGroup(ChoiceActionItem navGroupItem) {
            if (!CanAutoSelectFirstItemInGroup()) return;
            foreach (ChoiceActionItem item in navGroupItem.Items) {
                if (item.Enabled.ResultValue && item.Active.ResultValue) {
                    _navigationAction.DoExecute(item);
                    return;
                }
            }
        }
        protected bool CanAutoSelectFirstItemInGroup() {
            return ((IModelRootNavigationItemsAutoSelectedGroupItem)((IModelApplicationNavigationItems)Application.Model).NavigationItems).AutoSelectFirstItemInGroup;
        }
    }
}