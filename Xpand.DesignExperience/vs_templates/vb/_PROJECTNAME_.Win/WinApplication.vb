Imports Microsoft.VisualBasic
Imports DevExpress.ExpressApp
Imports System
Imports Xpand.ExpressApp.Win
Imports DevExpress.ExpressApp.Security

Partial Public Class $projectsuffix$WindowsFormsApplication
    Inherits XpandWinApplication

    Public Sub New()
        InitializeComponent()
        DelayedViewItemsInitialization = True
        AddHandler LastLogonParametersReading, AddressOf winApplication_LastLogonParametersReading
    End Sub
    Private Sub winApplication_LastLogonParametersReading(ByVal sender As Object, ByVal e As LastLogonParametersReadingEventArgs)
        Dim logonParameters As AuthenticationStandardLogonParameters = TryCast(e.LogonObject, AuthenticationStandardLogonParameters)
        If logonParameters IsNot Nothing Then
            If String.IsNullOrEmpty(logonParameters.UserName) Then
                logonParameters.UserName = "Admin"
            End If
        End If
    End Sub
	Private Sub $projectsuffix$WindowsFormsApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles MyBase.DatabaseVersionMismatch
#If EASYTEST Then
        e.Updater.Update()
        e.Handled = True
#Else
        If System.Diagnostics.Debugger.IsAttached Then
            e.Updater.Update()
            e.Handled = True
        Else
            Throw New InvalidOperationException( _
             "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application." & Constants.vbCrLf & _
             "This error occurred  because the automatic database update was disabled when the application was started without debugging." & Constants.vbCrLf & _
             "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " & _
             "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " & _
             "or manually create a database using the 'DBUpdater' tool." & Constants.vbCrLf & _
             "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " & _
             "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/")
        End If
#End If
    End Sub
End Class
