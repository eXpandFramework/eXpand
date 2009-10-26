using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;

using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

public partial class Default : System.Web.UI.Page, IWindowTemplate, IViewSiteTemplate, ISupportViewControlAdding
{
    private ContextActionsMenu contextMenu;
    private string GetApplicationInfoString()
    {
        AboutInfo aboutInfo = new AboutInfo(WebApplication.Instance);
        return aboutInfo.GetAboutInfoString();
    }
    private void ToolsRoundPanel_PreRender(object sender, EventArgs e)
    {
        bool isVisible = false;
        foreach (Control control in ToolsRoundPanel.Controls)
        {
            if (control is WebActionContainer && ((WebActionContainer)control).HasActiveActions)
            {
                isVisible = true;
                break;
            }
        }
        ToolsRoundPanel.Visible = isVisible;
    }
    protected override void InitializeCulture()
    {
        base.InitializeCulture();
        WebApplication.Instance.InitializeCulture();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        ToolsRoundPanel.PreRender += new EventHandler(ToolsRoundPanel_PreRender);
        contextMenu = new ContextActionsMenu(this, "RecordEdit", "ObjectsCreation", "ListView");
        Header.Title = WebApplication.Instance.Title;
        NavigationTabsActionContainer.ShowImages = true;
        Copyright.Text = GetApplicationInfoString();
        LogoImage.ImageUrl = string.Format("~/App_Themes/{0}/Xaf/Logo.png", Theme);
        WebApplication.Instance.CreateControls(this);
    }
    protected void Page_Prerender(object sender, EventArgs e)
    {
        if (TestScriptsManager.EasyTestEnabled)
        {
            ViewCaptionLabel.Attributes[EasyTestTagHelper.TestField] = "FormCaption";
            ViewCaptionLabel.Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;
        }
    }
    public override void Dispose()
    {
        if (contextMenu != null)
        {
            contextMenu.Dispose();
            contextMenu = null;
        }
        base.Dispose();
    }

    #region IFrameTemplate Members

    public IActionContainer DefaultContainer
    {
        get { return ViewPresentationActionContainer; }
    }

    public ICollection<IActionContainer> GetContainers()
    {
        List<IActionContainer> result = new List<IActionContainer>();
        result.AddRange(contextMenu.Containers);
        result.AddRange(new IActionContainer[] {
			NavigationTabsActionContainer, NavigationLinksActionContainer,
			SearchActionContainer,
			VerticalNewActionContainer, ContextObjectsCreationActionContainer,
			RecordsNavigationContainer,
			ListViewDataManagementActionContainer,
			ViewPresentationActionContainer,
			TopRecordEditActionContainer,
			DiagnosticActionContainer,
			VerticalToolsActionContainer,
			ViewsHistoryNavigationContainer,
			SecurityActionContainer
		});
        return result.ToArray();
    }

    public void SetView(DevExpress.ExpressApp.View view)
    {
        ViewSite.Controls.Clear();
        if (view != null)
        {
            contextMenu.CreateControls(view);
            view.CreateControls();
            if (ViewControlAdding != null)
            {
                ViewControlAdding(this, EventArgs.Empty);
            }
            ViewSite.Controls.Add((Control)view.Control);
            ViewCaptionLabel.Text = view.Caption;

            ImageInfo imageInfo = ImageLoader.Instance.GetLargeImageInfo(view.Info.GetAttributeValue("ImageName"));
            if (imageInfo.IsEmpty)
            {
                ViewImage.Visible = false;
            }
            else
            {
                ViewImage.ImageUrl = imageInfo.ImageUrl;
            }
            string colorString = view.Info.GetAttributeValue("BackColor");
        }
    }

    public void SetSettings(DictionaryNode settingsNode) { }
    public void ReloadSettings() { }
    public void SaveSettings() { }

    public void SetCaption(string caption)
    {
        Header.Title = String.IsNullOrEmpty(caption) ? caption : System.Web.HttpUtility.HtmlEncode(caption);
    }

    public void SetStatus(System.Collections.Generic.ICollection<string> statusMessages)
    {
        InfoMessagesPanel.Text = string.Join("<br>", new List<string>(statusMessages).ToArray());
    }
    private bool isSizeable;
    public bool IsSizeable
    {
        get { return isSizeable; }
        set { isSizeable = value; }
    }
    public object ViewSiteControl
    {
        get { return ViewSite; }
    }
    public event EventHandler ViewControlAdding;
    #endregion
}
