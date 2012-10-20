using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

public partial class Default : BaseXafPage {
    protected void Page_Load(object sender, EventArgs e) {
        ViewSiteControl = VSC;
        WebApplication.Instance.CreateControls(this);
        if(WebWindow.CurrentRequestWindow != null) {
            WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
        }
    }
    private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e) {
        WebWindow window = (WebWindow)sender;
        window.PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
        string isLeftPanelVisible = (VTC.HasActiveActions() || DAC.HasActiveActions()).ToString().ToLower();
        window.RegisterStartupScript("OnLoadCore", string.Format(@"Init(""{1}"", ""DefaultCS"");OnLoadCore(""LPcell"", ""separatorCell"", ""separatorImage"", {0}, true);", isLeftPanelVisible, CurrentTheme), true);
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
