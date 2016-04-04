#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel
*LogOn
*ScreenCapture
*Action Navigation(PropertyEditors.RichEdit)
*Action New (Rich Edit Object)

;------Chech that rich edit is shown and tables can be created--------
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