#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel ..\RichEdit;RichEditRibbonTabbedMDI
*LogOn
*ScreenCapture
*Action Navigation(PropertyEditors.RichEdit)
*MoveWindow 
*Action New (Rich Edit Object)

;------------activate the listview tab to destroy toolbars--------
*LClick [TextField]
*Sleep 500

*LClick [ListViewTab]
*Sleep 500
#IncludeFile ..\CheckToolBarIsNotVisible.inc

;------------activate the detailview tab to activate toolbars--------
*LClick [DetailViewTab]
*LClick [DetailViewTab]
#IncludeFile ..\CheckToolBarIsVisible.inc


#IncludeFile ..\Ribbon.inc