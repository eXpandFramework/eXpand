using System;

using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;

public partial class LoginPage : BaseXafPage {
	protected void Page_Load(object sender, EventArgs e) {
		WebApplication.Instance.CreateLogonControls(this);
	}
}
