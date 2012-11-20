Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Web.Configuration
Imports System.Web

Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Web
Imports DevExpress.Web.ASPxClasses

Public Class [Global]
    Inherits System.Web.HttpApplication
    Public Sub New()
        InitializeComponent()
    End Sub
	Protected Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        AddHandler ASPxWebControl.CallbackError, AddressOf Application_Error
#If EASYTEST Then
        DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = True
#End If

	End Sub
    Protected Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        WebApplication.SetInstance(Session, New Solution16AspNetApplication())
#If EASYTEST Then
        If (Not ConfigurationManager.ConnectionStrings.Item("EasyTestConnectionString") Is Nothing) Then
            WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings.Item("EasyTestConnectionString").ConnectionString
        End If
#End If
        If (Not ConfigurationManager.ConnectionStrings.Item("ConnectionString") Is Nothing) Then
            WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings.Item("ConnectionString").ConnectionString
        End If
        If System.Diagnostics.Debugger.IsAttached Then
            WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways
        End If
        WebApplication.Instance.Setup()
        WebApplication.Instance.Start()
    End Sub
    Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        Dim filePath As String = HttpContext.Current.Request.PhysicalPath
        If (Not String.IsNullOrEmpty(filePath)) AndAlso (filePath.IndexOf("Images") >= 0) AndAlso (Not System.IO.File.Exists(filePath)) Then
            HttpContext.Current.Response.End()
        End If
    End Sub
    Protected Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub
    Protected Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub
    Protected Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ErrorHandling.Instance.ProcessApplicationError()
    End Sub
    Protected Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        WebApplication.LogOff(Session)
        WebApplication.DisposeInstance(Session)
    End Sub
    Protected Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
    End Sub
#Region "Web Form Designer generated code"
    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
    End Sub
#End Region
End Class
