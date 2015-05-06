#DropDB SystemTesterEasyTest
#Application SystemTester.Win
;#Application SystemTester.Web

*XpandFillForm
 DBServer = SQLite
*LogOn
*XpandCheckFileExists SystemTesterEasyTest.db
 CheckInBin = True
*Navigate SQLite Object
*Action New
*XpandFillForm
 Sequence = 2
*SaveAndClose
*Navigate SQLite Object
