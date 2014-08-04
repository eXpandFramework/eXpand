#DropDB SecurityTesterEasyTest

#Application SecurityTester.Win
*FillForm
 User Name = Admin
*Action Log On
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)
*SetActiveWindowSize 1024x768
*XpandCompareScreenshot AdminNavigationItemPermission.png
 Mask = /regX/Navigation.png.pic

;Create permission and logon
*Action Navigation(Default.Role)
*ProcessRecord 
 Name = User
*FillForm
 Hidden Navigation Items = NavigationItemPermissionObject_ListView_Hidden
*Action Type Permissions
*Action Type Permissions.New
*FillForm
 Target Type = Navigation Item Permission Object
 Read = True
 Write = True
 Create = True
 Delete = True
 Navigate = True
*Action Save and Close
*Action Save and Close
*Action Log Off
*FillForm
 User Name = User
*Action Log On
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)

;CompareScreenShot
*SetActiveWindowSize 1024x768
*XpandCompareScreenshot UserNavigationItemPermission.png
 Mask = /regX/Navigation.png.pic

;The items are visible in other strategies
*Action Navigation Item Permission
*Action Log Off
*FillForm
 User Name = User
*Action Log On
*Action Navigation(NavigationItemPermission.Navigation Item Permission Object)
*SetActiveWindowSize 1024x768
*XpandCompareScreenshot UserNavigationItemPermission.png
 Mask = /regX/Navigation.png.pic


