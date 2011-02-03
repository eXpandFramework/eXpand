using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxGlobalEvents;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic;
using Xpand.ExpressApp.Web.Templates;

public partial class Default : XpandBaseXafPage, ISupportAdditionalViewControls {


    public override NavigationActionContainer NavigationActionContainer {
        get { return NavigationBarActionContainer; }
    }

    public void RegisterUpdatePanel(UpdatePanel panel) {
        foreach (
            MethodInfo methodInfo in typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)) {
            if (methodInfo.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")) {
                methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
            }
        }
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        ViewSiteControl = viewSiteControl;
        TopPanel.ShowLoadingPanel = false;
        BottomCallBackPanel.ShowLoadingPanel = false;
        Load += OnLoad;
    }


    void OnLoad(object sender, EventArgs eventArgs) {
        WebApplication.Instance.CreateControls(this);
        ToolsRoundPanel.PreRender += ToolsRoundPanel_PreRender;
        Splitter.PreRender += Splitter_PreRender;
    }
    private void ToolsRoundPanel_PreRender(object sender, EventArgs e) {
        bool isVisible = ToolsRoundPanel.Controls.OfType<ActionContainerHolder>().Any(control => (control).HasActiveActions());
        ToolsRoundPanel.Visible = isVisible;
    }
    private void Splitter_PreRender(object sender, EventArgs e) {
        bool isVisible = Splitter.GetPaneByName("Right").Controls.Cast<Control>().Any(control => control.Visible);
        Splitter.GetPaneByName("Right").Visible = isVisible;
    }

    protected override void OnViewChanged(DevExpress.ExpressApp.View view) {
        ViewSiteControl = viewSiteControl;
        base.OnViewChanged(view);
    }


    protected override ContextActionsMenu CreateContextActionsMenu() {
        return new ContextActionsMenu(this, "Edit", "RecordEdit", "ObjectsCreation", "ListView", "Reports");
    }
    protected override IActionContainer GetDefaultContainer() {
        return ToolBar.FindActionContainerById("View");
    }
    public override void SetStatus(ICollection<string> statusMessages) {
        InfoMessagesPanel.Text = string.Join("<br>", new List<string>(statusMessages).ToArray());
    }

    public ASPxCallbackPanel TopPanel {
        get { return TopCallBackPanel; }
    }

    public ASPxCallbackPanel BottomPanel {
        get { return BottomCallBackPanel; }
    }

    public ASPxGlobalEvents AsPxGlobalEvents {
        get { return GlobalEvents; }
    }
}

