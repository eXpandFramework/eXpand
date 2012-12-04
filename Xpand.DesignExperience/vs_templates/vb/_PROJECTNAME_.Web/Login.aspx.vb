Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.ExpressApp.Web
Imports DevExpress.ExpressApp.Web.Templates

Partial Public Class LoginPage
	Inherits BaseXafPage
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		WebApplication.Instance.CreateLogonControls(Me)
	End Sub
End Class
