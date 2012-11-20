Imports Xpand.ExpressApp.ModelDifference.Security
Imports Xpand.ExpressApp.Security.Core
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security.Strategy

Public Class Updater
    Inherits Xpand.Persistent.BaseImpl.Updater

    Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
        MyBase.New(objectSpace, currentDBVersion)
    End Sub

    Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
        MyBase.UpdateDatabaseAfterUpdateSchema()
        Dim defaultRole As SecuritySystemRole = ObjectSpace.GetDefaultRole()
        ObjectSpace.GetAdminRole("Admin").GetUser("Admin", "")
        Dim user As SecuritySystemUser = ObjectSpace.GetRole("User").GetUser("user", "")
        user.Roles.Add(defaultRole)
        Dim modelRole As SecuritySystemRole = ObjectSpace.GetDefaultModelRole("ModelRole")
        user.Roles.Add(modelRole)
        ObjectSpace.CommitChanges()
    End Sub


End Class
