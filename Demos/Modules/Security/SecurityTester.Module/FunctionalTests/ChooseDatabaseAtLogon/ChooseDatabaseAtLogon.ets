#DropDB SecurityTesterEasyTest1
#DropDB SecurityTesterEasyTest2
#Application SecurityTester.Win
#Application SecurityTester.Web

*LogOn
#IfDef SecurityTester.Win
*XpandHandleDialog
 Message = '*"DBServer" must not be empty*'
 Respond = OK
#EndIf
#IfDef SecurityTester.Web
*CheckValidationResult
 Info = '*"DBServer" must not be empty*'
#EndIf
 

!XpandFillForm 
 DBServer = NonExistentServer

*XpandFillForm
 DBServer = DB1
*LogOn
*Navigate Choose Database At Logon Object
*Action New
*XpandCheckFieldValues
 Database = SecurityTesterEasyTest1
*SaveAndClose

#IfDef SecurityTester.Web
*Action Log Off
#EndIf
#IfDef SecurityTester.Win
*ReopenApplication
#EndIf

*XpandFillForm 
 DBServer = DB2
*LogOn
*Navigate Choose Database At Logon Object
*Action New
*XpandCheckFieldValues
 Database = SecurityTesterEasyTest2


