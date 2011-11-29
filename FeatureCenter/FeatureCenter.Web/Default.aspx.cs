using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

public partial class DefaultVertical : BaseXafPage {
    private void CallbackManager_PreRender(object sender, EventArgs e) {
        ((XafCallbackManager)sender).PreRender -= new EventHandler<EventArgs>(CallbackManager_PreRender);
        UpdateTRPVisibility();
    }
    private void UpdateTRPVisibility() {
        bool isVisible = false;
        foreach (Control control in TRP.Controls) {
            if (control is ActionContainerHolder) {
                if (((ActionContainerHolder)control).HasActiveActions()) {
                    isVisible = true;
                    break;
                }
            }
        }
        TRP.Visible = isVisible;
    }
    protected void Page_PreRender(object sender, EventArgs e) {
        UpdateTRPVisibility();
    }
    protected void Page_Load(object sender, EventArgs e) {
        CallbackManager.PreRender += new EventHandler<EventArgs>(CallbackManager_PreRender);
        WebApplication.Instance.CreateControls(this);
    }
    protected override ContextActionsMenu CreateContextActionsMenu() {
        return new ContextActionsMenu(this, "Edit", "RecordEdit", "ObjectsCreation", "ListView", "Reports");
    }
    protected override IActionContainer GetDefaultContainer() {
        return TB.FindActionContainerById("View");
    }
    public override void SetStatus(System.Collections.Generic.ICollection<string> statusMessages) {
        InfoMessagesPanel.Text = string.Join("<br>", new List<string>(statusMessages).ToArray());
    }
    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        ViewSiteControl = VSC;
    }
    protected override void OnViewChanged(DevExpress.ExpressApp.View view) {
        ViewSiteControl = VSC;
        base.OnViewChanged(view);
    }
}
