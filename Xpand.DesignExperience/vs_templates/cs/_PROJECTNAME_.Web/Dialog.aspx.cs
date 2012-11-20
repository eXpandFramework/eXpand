using System;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Templates;

public partial class DialogPage : BaseXafPage, ILookupPopupFrameTemplate {
    protected void Page_Load(object sender, EventArgs e) {
        ViewSiteControl = viewSiteControl;
        WebApplication.Instance.CreateControls(this);
        WebWindow window = WebWindow.CurrentRequestWindow;
        if (window != null) {
            string clientScript = string.Format(@" 
            var activePopupControl = GetActivePopupControl(window.parent);
            if (activePopupControl != null){{
                var viewImageControl = document.getElementById('{0}');
                if (viewImageControl && viewImageControl.src != ''){{
                    activePopupControl.SetHeaderImageUrl(viewImageControl.src);
                }}
                var viewCaptionControl = document.getElementById('{1}');
                if (viewCaptionControl){{
                    activePopupControl.SetHeaderText(viewCaptionControl.innerText);
                }}
            }}", viewImageControl.Control.ClientID, viewCaptionControl.Control.ClientID);
            window.RegisterStartupScript("UpdatePopupControlHeader", clientScript, true);
        }
    }
    public DialogPage() { }
    #region ILookupPopupFrameTemplate Members
    public void FocusFindEditor() {

    }

    public bool IsSearchEnabled {
        get { return SearchActionContainer.Visible; }
        set { SearchActionContainer.Visible = value; }
    }

    public void SetStartSearchString(string searchString) { }

    #endregion
}
