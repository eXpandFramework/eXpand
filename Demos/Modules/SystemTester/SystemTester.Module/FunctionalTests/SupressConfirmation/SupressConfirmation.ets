#Application SystemTester.Win
#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture
*Action Navigation(Default.Supress Confirmation Object)
*DeleteAllObjects

;------------new object in detailview-------------
*Action New
*Action Navigation(Default.Supress Confirmation Object)
!HandleDialog
 Message = *save*
*CheckTable
 RowCount = 0 
;------------modify new object in detailview-------------
*Action New
*XpandFillForm
 Name = New
*Action Navigation(Default.Supress Confirmation Object)
!HandleDialog
 Message = *save*
*CheckTable
 RowCount = 0

;---------------change existing object in detail view--------
*Action New
*XpandFillForm
 Name = Tolis
*Action Save
*XpandFillForm
 Name = Bekiaris
*Action Navigation(Default.Supress Confirmation Object)
!HandleDialog
 Message = *save*

;---------------Commit all modified detailview objects--------
#IfDef SystemTester.Win
*Action New
*XpandFillForm
 Name = Bekiaris
*Action Save
*Action Navigation(Default.Supress Confirmation Object)
*XpandProcessRecord
 Name = Bekiaris
*XpandFillForm
 Name = eXpandFramework
*Action Next Record
*Action Save
*Action Navigation(Default.Supress Confirmation Object)
*CheckTable
 Columns = Name
 Row = eXpandFramework
#EndIf



 