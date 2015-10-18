#DropDB SystemTesterEasyTest
#Application SystemTester.Win
;#Application SystemTester.Web

*XpandDeleteFile SystemTesterEasyTest.db
 InBin = True
*LogOn
*XpandCheckFileExists SystemTesterEasyTest.db
 CheckInBin = True
*Navigate SQLite Object
*Action New
*XpandFillForm
 Sequence = 2
*SaveAndClose
*Navigate SQLite Object
