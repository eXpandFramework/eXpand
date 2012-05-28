using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;

public partial class ErrorPage : System.Web.UI.Page {
    protected override void InitializeCulture() {
        if (WebApplication.Instance != null)
            WebApplication.Instance.InitializeCulture();
    }
    private void Page_Load(object sender, System.EventArgs e) {
        TestScriptsManager testScriptsManager = new TestScriptsManager(Page);
        testScriptsManager.RegisterControl(JSLabelTestControl.ClassName, "FormCaption", TestControlType.Field, "FormCaption");
        testScriptsManager.RegisterControl(JSLabelTestControl.ClassName, "RequestUrl", TestControlType.Field, "RequestUrl");
        testScriptsManager.RegisterControl(JSLabelTestControl.ClassName, "DescriptionTextBox", TestControlType.Field, "Description");
        testScriptsManager.RegisterControl(JSDefaultTestControl.ClassName, "ReportButton", TestControlType.Action, "Report");
        testScriptsManager.AllControlRegistered("");
        if (WebApplication.Instance != null) {
            ApplicationTitle.Text = WebApplication.Instance.Title;
        } else {
            ApplicationTitle.Text = "No application";
        }
        Header.Title = "Application Error - " + ApplicationTitle.Text;

        ErrorInfo errorInfo = ErrorHandling.GetApplicationError();
        if (errorInfo != null) {
            RequestUrl.NavigateUrl = errorInfo.Url;
            RequestUrl.Text = errorInfo.Url;
            RequestUrl2.NavigateUrl = errorInfo.Url;
            RequestUrl2.Text = errorInfo.Url;
            if (!string.IsNullOrEmpty(errorInfo.UrlReferrer)) {
                HyperLinkReturn.NavigateUrl = errorInfo.UrlReferrer;
            } else {
                LiteralReturn.Visible = false;
                HyperLinkReturn.Visible = false;
            }
            if (ErrorHandling.CanShowDetailedInformation) {
                DetailsText.Text = errorInfo.GetTextualPresentation(true);
            } else {
                Details.Visible = false;
            }
            ReportResult.Visible = false;
            ReportForm.Visible = ErrorHandling.CanSendAlertToAdmin;
        } else {
            ErrorPanel.Visible = false;
        }
    }
    #region Web Form Designer generated code
    override protected void OnInit(EventArgs e) {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent() {
        this.Load += new System.EventHandler(this.Page_Load);
        this.PreRender += new EventHandler(ErrorPage_PreRender);
    }

    private void ErrorPage_PreRender(object sender, EventArgs e) {
        RegisterThemeAssemblyController.RegisterThemeResources((Page)sender);
    }

    #endregion
    protected void ReportButton_Click(object sender, EventArgs e) {
        ErrorInfo errorInfo = ErrorHandling.GetApplicationError();
        if (errorInfo != null) {
            ErrorHandling.SendAlertToAdmin(errorInfo.Id, DescriptionTextBox.Text, errorInfo.Exception.Message);
            ErrorHandling.ClearApplicationError();
            ApologizeMessage.Visible = false;
            ReportForm.Visible = false;
            Details.Visible = false;
            ReportResult.Visible = true;
        }
    }
}
