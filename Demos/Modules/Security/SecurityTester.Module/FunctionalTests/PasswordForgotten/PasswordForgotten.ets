#IncludeFile .\Init.inc
*Action Restore Password
#IfDef SecurityTester.Web
*Action Cancel
*Action Restore Password
#EndIf
*FillForm
 Email = aaa
*Action Restore Password
#IfDef SecurityTester.Win
*HandleDialog
 Respond = OK
#EndIf
#IfDef SecurityTester.Web
*CheckValidationResult
 Info = '*Email*'
#EndIf
*FillForm
 Email = newuser@mail.com
*Action Restore Password
#IfDef SecurityTester.Win
*HandleDialog
 Respond = OK
#EndIf
#IfDef SecurityTester.Web
*CheckValidationResult
 Message = 'Cannot find registered users*'
#EndIf
*FillForm
 Email = user@mail.com
*Action Restore Password
*LogOn User 