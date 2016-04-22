*Mouse LeftButtonClick
 MoveMouseTo = [InsertRibbonPage]
*Mouse LeftButtonClick
 MoveMouseTo = [TableRibbonCommand]
*Sleep 1000
!HandleDialog
 Caption = Insert Table
 Respond = Cancel
*LClick [RichEditAction]
*HandleDialog
 Caption = Rich Edit
 Respond = Yes