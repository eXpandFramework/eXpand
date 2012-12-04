
Imports Xpand.ExpressApp.Security.Core
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp
Imports System
Imports DevExpress.ExpressApp.Security.Strategy
Imports Xpand.ExpressApp.ModelDifference.Security

Public Class Updater
    Inherits ModuleUpdater

    Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
        MyBase.New(objectSpace, currentDBVersion)
    End Sub

    Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
        MyBase.UpdateDatabaseAfterUpdateSchema()

    End Sub

End Class
