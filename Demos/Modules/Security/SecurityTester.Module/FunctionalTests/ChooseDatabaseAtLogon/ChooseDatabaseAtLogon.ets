#DropDB SecurityTesterEasyTest1
#DropDB SecurityTesterEasyTest2
#Application SecurityTester.Win
#Application SecurityTester.Web

*UseModel
*XpandFillForm 
 DBServer = DB1
*LogOn
*Navigate Choose Database At Logon Object
*Action New
*XpandCheckFieldValues
 Database = SecurityTesterEasyTest1
*SaveAndClose

*Action Log Off

*XpandFillForm 
 DBServer = DB2
*LogOn
*Navigate Choose Database At Logon Object
*Action New
*XpandCheckFieldValues
 Database = SecurityTesterEasyTest2


