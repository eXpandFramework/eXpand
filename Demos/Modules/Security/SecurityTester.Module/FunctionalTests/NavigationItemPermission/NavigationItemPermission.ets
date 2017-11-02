#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
#Application SecurityTester.Web
*UseModel

*LogOn Admin
*ScreenCapture
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)

;Test that items are in navigation for admin
*Action Navigation(NavigationItemPermission.Hidden)
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)

*CreatePermission Navigation Item Permission Object

*XpandProcessRecord
 Name = User
 Action = Edit
*Action Extras
*XpandFillForm
 Hidden Navigation Items = NavigationItemPermissionObject_ListView_Hidden
*SaveAndClose

*Action Log Off

*LogOn User
!Action Navigation(NavigationItemPermission.Hidden)
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)


