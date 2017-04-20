Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports DevExpress.ExpressApp
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.ExpressApp.Security.Strategy
Imports DevExpress.Xpo
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Persistent.BaseImpl.PermissionPolicy
Imports Xpand.ExpressApp.Security.Core

Public Class Updater
    Inherits ModuleUpdater
    Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
        MyBase.New(objectSpace, currentDBVersion)
    End Sub
    Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
		MyBase.UpdateDatabaseAfterUpdateSchema()
		Dim defaultRole = DirectCast(ObjectSpace.GetDefaultRole(), PermissionPolicyRole)

		Dim adminRole = ObjectSpace.GetAdminRole("Admin")
		adminRole.GetUser("Admin")

		Dim userRole = ObjectSpace.GetRole("User")
		Dim user = DirectCast(userRole.GetUser("user"), PermissionPolicyUser)
		user.Roles.Add(defaultRole)

		ObjectSpace.CommitChanges()

	End Sub

End Class



