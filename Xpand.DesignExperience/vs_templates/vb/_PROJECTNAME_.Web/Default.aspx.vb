Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Web.UI
Imports DevExpress.ExpressApp.Templates
Imports DevExpress.ExpressApp.Web
Imports DevExpress.ExpressApp.Web.Templates
Imports DevExpress.ExpressApp.Web.Templates.ActionContainers

Partial Public Class [Default]
    Inherits BaseXafPage
    Private Sub CurrentRequestWindow_PagePreRender(ByVal sender As Object, ByVal e As EventArgs)
        Dim window As WebWindow = CType(sender, WebWindow)
        RemoveHandler window.PagePreRender, AddressOf CurrentRequestWindow_PagePreRender
        Dim isLeftPanelVisible As String = (VTC.HasActiveActions() Or DAC.HasActiveActions()).ToString().ToLower()
        window.RegisterStartupScript("OnLoadCore", String.Format("Init(""{1}"", ""DefaultCS"");OnLoadCore(""LPcell"", ""separatorCell"", ""separatorImage"", {0}, true);", isLeftPanelVisible, CurrentTheme), True)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ViewSiteControl = VSC
        WebApplication.Instance.CreateControls(Me)
        If WebWindow.CurrentRequestWindow IsNot Nothing Then
            AddHandler WebWindow.CurrentRequestWindow.PagePreRender, AddressOf CurrentRequestWindow_PagePreRender
        End If
    End Sub
	Protected Overrides Function CreateContextActionsMenu() As ContextActionsMenu
		Return New ContextActionsMenu(Me, "Edit", "RecordEdit", "ObjectsCreation", "ListView", "Reports")
	End Function
	Protected Overrides Function GetDefaultContainer() As IActionContainer
        Return TB.FindActionContainerById("View")
	End Function
	Public Overrides Sub SetStatus(ByVal statusMessages As System.Collections.Generic.ICollection(Of String))
		InfoMessagesPanel.Text = String.Join("<br>", New List(Of String)(statusMessages).ToArray())
    End Sub
End Class
