using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

public partial class DefaultVertical : BaseXafPage {
    private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e) {
        ((WebWindow)sender).PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
        UpdateTRPVisibility();
    }
    private void UpdateTRPVisibility() {
        bool isVisible = false;
        foreach(Control control in TRP.Controls) {
            if(control is ActionContainerHolder) {
                if(((ActionContainerHolder)control).HasActiveActions()) {
                    isVisible = true;
                    break;
                }
            }
        }
        TRP.Visible = isVisible;
    }
	protected void Page_Load(object sender, EventArgs e) {
        ViewSiteControl = VSC;
        if(WebWindow.CurrentRequestWindow != null) {
            WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
        }
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
}
