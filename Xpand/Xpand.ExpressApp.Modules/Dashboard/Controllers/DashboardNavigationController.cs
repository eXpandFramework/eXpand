using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.Dashboard.Controllers {
    public partial class DashboardNavigationController : WindowController, IModelExtender {
        Dictionary<ChoiceActionItem, DashboardDefinition> _dashboardActions;
        ShowNavigationItemController _navigationController;

        public DashboardNavigationController() {
            TargetWindowType = WindowType.Main;
        }

        protected Dictionary<ChoiceActionItem, DashboardDefinition> DashboardActions {
            get { return _dashboardActions ?? (_dashboardActions = new Dictionary<ChoiceActionItem, DashboardDefinition>()); }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptionsDashboard, IModelOptionsDashboardNavigation>();
        }

        protected override void OnDeactivated() {
            UnsubscribeFromEvents();
            base.OnDeactivated();
        }

        void SubscribeToEvents() {
            _navigationController = Frame.GetController<ShowNavigationItemController>();
            if (_navigationController != null)
                _navigationController.ItemsInitialized += _NavigationController_ItemsInitialized;
        }

        void UnsubscribeFromEvents() {
            if (_navigationController != null) {
                _navigationController.ItemsInitialized -= _NavigationController_ItemsInitialized;
                _navigationController = null;
            }
        }

        protected override void OnFrameAssigned() {
            UnsubscribeFromEvents();
            base.OnFrameAssigned();
            SubscribeToEvents();
        }

        void _NavigationController_ItemsInitialized(object sender, EventArgs e) {
            IModelView view = Application.FindModelView(Application.FindListViewId(typeof(DashboardDefinition)));
            var options = ((IModelOptionsDashboards)Application.Model.Options);
            var dashboardOptions = ((IModelOptionsDashboardNavigation)options.Dashboards);
            if (dashboardOptions.GenerateDashboardsInGroup) {
                ReloadDashboardActions();
                var actions = new List<ChoiceActionItem>();
                if (DashboardActions.Count > 0) {
                    var dashboardGroup = GetGroupFromActions(((ShowNavigationItemController)sender).ShowNavigationItemAction, dashboardOptions.Group);
                    if (dashboardGroup == null) {
                        dashboardGroup = new ChoiceActionItem(dashboardOptions.Group, null) {
                            ImageName = "BO_DashboardDefinition"
                        };
                        ((ShowNavigationItemController)sender).ShowNavigationItemAction.Items.Add(dashboardGroup);
                    }
                    while (dashboardGroup.Items.Count != 0) {
                        ChoiceActionItem item = dashboardGroup.Items[0];
                        dashboardGroup.Items.Remove(item);
                        actions.Add(item);
                    }
                    foreach (ChoiceActionItem action in DashboardActions.Keys) {
                        action.Active["HasRights"] = HasRights(action, view);
                        actions.Add(action);
                    }
                    foreach (ChoiceActionItem action in actions.OrderBy(action => action.Model.Index))
                        dashboardGroup.Items.Add(action);

                }
            }
        }

        protected virtual bool HasRights(ChoiceActionItem item, IModelView view) {
            var data = (ViewShortcut)item.Data;
            if (view == null) {
                throw new ArgumentException(string.Format("Cannot find the '{0}' view specified by the shortcut: {1}",
                                                          data.ViewId, data));
            }
            Type type = (view is IModelObjectView) ? ((IModelObjectView)view).ModelClass.TypeInfo.Type : null;
            if (type != null) {
                if (!string.IsNullOrEmpty(data.ObjectKey) && !data.ObjectKey.StartsWith("@")) {
                    try {
                        using (IObjectSpace space = CreateObjectSpace()) {
                            object objectByKey = space.GetObjectByKey(type, space.GetObjectKey(type, data.ObjectKey));
                            return (DataManipulationRight.CanRead(type, null, objectByKey, null, space) &&
                                    DataManipulationRight.CanNavigate(type, objectByKey, space));
                        }
                    } catch {
                        goto Label_00CB;
                    }
                }
                return DataManipulationRight.CanNavigate(type, null, null);
            }
        Label_00CB:
            return true;
        }

        protected virtual IObjectSpace CreateObjectSpace() {
            return Application.CreateObjectSpace();
        }

        public virtual void UpdateNavigationImages() {
        }

        void ReloadDashboardActions() {
            DashboardActions.Clear();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            IOrderedEnumerable<DashboardDefinition> templates =
                objectSpace.GetObjects<DashboardDefinition>().Where(t => t.Active).OrderBy(i => i.Index);
            foreach (DashboardDefinition template in templates) {
                var action = new ChoiceActionItem(
                    template.Oid.ToString(),
                    template.Name,
                    new ViewShortcut(typeof(DashboardDefinition), template.Oid.ToString(), "DashboardViewer_DetailView")) {
                        ImageName = "BO_DashboardDefinition"
                    };
                action.Model.Index = template.Index;
                DashboardActions.Add(action, template);
            }
        }

        public void RecreateNavigationItems() {
            _navigationController.RecreateNavigationItems();
        }

        ChoiceActionItem GetGroupFromActions(SingleChoiceAction action, String name) {
            return action.Items.FirstOrDefault(item => item.Caption.Equals(name));
        }
    }

    public interface IModelOptionsDashboardNavigation : IModelNode {
        [DefaultValue("Dashboards")]
        String Group { get; set; }

        [DefaultValue(true)]
        bool GenerateDashboardsInGroup { get; set; }
    }
}