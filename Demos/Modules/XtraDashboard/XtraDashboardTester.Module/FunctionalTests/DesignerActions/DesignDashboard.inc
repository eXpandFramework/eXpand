#IfDef XtraDashboardTester.Win
;ShowDesigner
*Action Dashboard Edit
*Sleep 5000
*XpandHandleDialog
 Caption = Dashboard Designer
 
;add a chart
*Mouse LeftButtonClick
 MoveMouseTo = 300x96
*Sleep 3000

;Add first property as chart argument
*MouseDragDrop 
 Start = 76x330
 End = 270x236
*Sleep 1000
  
;SaveAndClose
*Mouse LeftButtonClick
 MoveMouseTo = 62x121
*Sleep 1500
#EndIf