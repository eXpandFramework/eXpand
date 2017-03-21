#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel ..\RichEdit
*LogOn
*ScreenCapture

*Action Navigation(PropertyEditors.RichEdit)
*Action New (Rich Edit Object)

;------Check that rich edit is shown and tables can be created--------
*LClick [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;---------Check that toolbar is visible when a window from a command was shown----
*LClick [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;---------Check that toolbar is visible when form loses focus------
*MinimizeApplicationWindow
*ActivateWindow
*LClick [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;------Check that hypelinks are disable as per model-----
*LClick [BookmarkCommand]
*Sleep 1000
!HandleDialog
 Caption = Bookmark
*HandleDialog
 Caption = Rich Edit Object - SystemTester

;-----Check that toolbar is not available when editor is not focused-------
*XpandFillForm
 Field = somevalue
*LClick [TableCommand]
*Sleep 1000
!HandleDialog
 Caption = Insert Table
 Respond = Cancel

;------Check that toolbar is restored when gotfocus--------
*LClick [TextField]
*LClick [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel

;------Check the toolbar in a different tab--------
*Action Tab2
*LClick [TextField]
*LClick [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel

;------Check HideToolBarsAttribute--------
*Action Tab3
*LClick [TextField]
*LClick [TableCommand]
*Sleep 1000
!HandleDialog
 Caption = Insert Table

*SaveAndClose


