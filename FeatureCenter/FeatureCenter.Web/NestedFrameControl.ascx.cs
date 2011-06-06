using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Templates.ActionContainers;

[ParentControlCssClass("NestedFrameControl")]
public partial class NestedFrameControl : System.Web.UI.UserControl, IFrameTemplate, ISupportActionsToolbarVisibility, IDynamicContainersTemplate, IViewHolder {
    private ContextActionsMenu contextMenu;
    private ActionContainerCollection actionContainers = new ActionContainerCollection();
    DevExpress.ExpressApp.View view;
    protected override void OnPreRender(EventArgs e) {
        base.OnPreRender(e);
        if (ToolBar != null) {
            ToolBar.Visible = actionsToolbarVisibility == ActionsToolbarVisibility.Hide ? false : true;
        }
    }
    public NestedFrameControl() {
        contextMenu = new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
        actionContainers.AddRange(contextMenu.Containers);
    }
    //B157146, B157117
    public override void Dispose() {
        if (ToolBar != null) {
            ToolBar.Dispose();
            ToolBar = null;
        }
        if (contextMenu != null) {
            contextMenu.Dispose();
            contextMenu = null;
        }
        base.Dispose();
    }
    #region IFrameTemplate Members
    public IActionContainer DefaultContainer {
        get { return /*ViewContainer*/ null; }
    }
    public ICollection<IActionContainer> GetContainers() {
        return actionContainers.ToArray();
    }
    public void SetView(DevExpress.ExpressApp.View view) {
        this.view = view;
        if (view != null) {
            contextMenu.CreateControls(view);
        }

        OnViewChanged(view);
    }
    #endregion
    protected virtual void OnViewChanged(DevExpress.ExpressApp.View view) {
        if (ViewChanged != null) {
            ViewChanged(this, new TemplateViewChangedEventArgs(view));
        }
    }

    #region IActionBarVisibilityManager Members
    private ActionsToolbarVisibility actionsToolbarVisibility = ActionsToolbarVisibility.Default;
    public ActionsToolbarVisibility ActionsToolbarVisibility {
        get {
            return actionsToolbarVisibility;
        }
        set {
            actionsToolbarVisibility = value;
        }
    }
    #endregion
    #region IDynamicContainersTemplate Members
    private void OnActionContainersChanged(ActionContainersChangedEventArgs args) {
        if (ActionContainersChanged != null) {
            ActionContainersChanged(this, args);
        }
    }
    public void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
        IEnumerable<IActionContainer> addedContainers = this.actionContainers.TryAdd(actionContainers);
        if (DevExpress.ExpressApp.Utils.Enumerator.Count(addedContainers) > 0) {
            OnActionContainersChanged(new ActionContainersChangedEventArgs(addedContainers, ActionContainersChangedType.Added));
        }
    }
    public void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
        IList<IActionContainer> removedContainers = new List<IActionContainer>();
        foreach (IActionContainer actionContainer in actionContainers) {
            if (this.actionContainers.Contains(actionContainer)) {
                this.actionContainers.Remove(actionContainer);
                removedContainers.Add(actionContainer);
            }
        }
        if (DevExpress.ExpressApp.Utils.Enumerator.Count(removedContainers) > 0) {
            OnActionContainersChanged(new ActionContainersChangedEventArgs(removedContainers, ActionContainersChangedType.Removed));
        }
    }
    public event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;
    #endregion
    public DevExpress.ExpressApp.View View {
        get { return view; }
    }

    #region ISupportViewChanged Members

    public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;

    #endregion
}
