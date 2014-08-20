#IncludeFile .\Init.inc

;Test that items are in navigation for admin
*Action Navigation(NavigationItemPermission.Hidden)
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)

*CreatePermission Navigation Item Permission Object

*XpandProcessRecord
 Name = User
 Action = Edit
*FillForm
 Hidden Navigation Items = NavigationItemPermissionObject_ListView_Hidden
*SaveAndClose

*Action Log Off
*FillForm
 User Name = User
*Action Log On
!Action Navigation(NavigationItemPermission.Hidden)
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)


