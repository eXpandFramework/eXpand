#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel
*LogOn
*ScreenCapture
*Action Navigation(PropertyEditors.RichEdit)
*Action New (Rich Edit Object)

;------Check that rich edit is shown and tables can be created--------
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;---------Check that toolbar is visible when a window from a command was shown----
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;---------Check that toolbar is visible when form loses focus------
*MinimizeApplicationWindow
*XpandActivateApplicationWindow
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel
;------Check that hypelinks are disable as per model-----
*Mouse LeftButtonClick
 MoveMouseTo = [BookmarkCommand]
*Sleep 1000
!HandleDialog
 Caption = Bookmark
*HandleDialog
 Caption = Rich Edit Object - SystemTester

;-----Check that toolbar is not available when editor is not focused-------
*XpandFillForm
 Field = somevalue
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
!HandleDialog
 Caption = Insert Table
 Respond = Cancel

;------Check that toolbar is restored when gotfocus--------
*Mouse LeftButtonClick
 MoveMouseTo = [TextField]
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel

;------Check the toolbar in a different tab--------
*Action Tab2
*Mouse LeftButtonClick
 MoveMouseTo = [TextField]
*Mouse LeftButtonClick
 MoveMouseTo = [TableCommand]
*Sleep 1000
*HandleDialog
 Caption = Insert Table
 Respond = Cancel

