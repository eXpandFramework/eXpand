*Action Navigation(Reports.Dashboard Definition)
*SelectRecords
 Columns = Name
 Row = Filtered from model
*Action Dashboard Edit
#IfDef XtraDashboardTester.Win
*Sleep 5000
*XpandHandleDialog
 Caption = Dashboard Designer
 Close = True
#EndIf
#IfDef XtraDashboardTester.Web
*Action OK
#EndIf