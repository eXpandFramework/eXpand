using System;

using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;

public partial class LoginPage : BaseXafPage {
    protected void Page_Load(object sender, EventArgs e) {
        WebApplication.Instance.CreateLogonControls(this);
    }
    protected override void OnViewChanged(DevExpress.ExpressApp.View view) {
//        ViewSiteControl=viewSiteControl;
        base.OnViewChanged(view);
    }
}
