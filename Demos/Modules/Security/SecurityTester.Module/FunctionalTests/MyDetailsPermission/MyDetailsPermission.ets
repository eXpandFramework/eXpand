#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
#Application SecurityTester.Web

;-------Check actions when no permission applied--------------------
*LogOn User
*Navigate My Details
#IfDef SecurityTester.Web
*Action My Details
#EndIf

*LogOff
*LogOn Admin

;---------Create MyDetails permission----------------
*Navigate Role
*XpandProcessRecord 
 Name = User
*Action Permissions
*Action Permissions.New(MyDetails)
*XpandFillForm
 Modifier = Deny
*SaveAndClose
*SaveAndClose

*LogOff
*LogOn User

;-------Check actions when permission applied--------------------
!ActionAvailable Navigation(My Details)
#IfDef SecurityTester.Web
!ActionAvailable My Details
#EndIf