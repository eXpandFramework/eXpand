#DropDB SystemTesterEasyTest
#Application SystemTester.Win
;#Application SystemTester.Web

*XpandFillForm
 DBServer = SQLite
*LogOn
*XpandCheckFileExists SystemTesterEasyTest
 CheckInBin = True
*Navigate SQLite Object
*Action New
*XpandFillForm
 Sequence = 2
*SaveAndClose
*Navigate SQLite Object
