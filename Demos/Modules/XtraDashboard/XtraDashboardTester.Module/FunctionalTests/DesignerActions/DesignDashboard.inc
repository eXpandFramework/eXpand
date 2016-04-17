*Action Dashboard Edit
;*ActivateApplicationWindow
*Sleep 5000
;--------------ShowDesigner---------------

#IfDef XtraDashboardTester.Win
*XpandHandleDialog
 Caption = Dashboard Designer
#EndIf

#IfDef XtraDashboardTester.Web
*ActionAvailable OK
*ActionAvailable Cancel
#EndIf 

;-------------------add a chart-------------------

#IfDef XtraDashboardTester.Win
*Mouse LeftButtonClick
 MoveMouseTo = [ChartRibbonButton]
*Sleep 3000
#EndIf

#IfDef XtraDashboardTester.Web
*SendKeys
 Keys = Home
;------IE crashed------
;*Mouse LeftButtonClick
; MoveMouseTo = [WebChartButton]
;*Sleep 3000
#EndIf

;-----------------Add first property as chart argument----------

#IfDef XtraDashboardTester.Win
*MouseDragDrop 
 Start = [FirstNameProperty]
 End = [DataItem1]
*Sleep 1000
#EndIf
  
;----------------------SaveAndClose--------------
#IfDef XtraDashboardTester.Win
*Mouse LeftButtonClick
 MoveMouseTo = [SaveAndCloseRibbonButton]
*Sleep 1500
#EndIf
#IfDef XtraDashboardTester.Web
*Mouse LeftButtonClick
 MoveMouseTo = [WebMenu]
*Sleep 1500
*Mouse LeftButtonClick
 MoveMouseTo = [WebSave]
*Sleep 1500
*Action OK
#EndIf