#DropDB ModelArtifactStateTesterEasyTest
#Application ModelArtifactStateTester.Win
;#Application ModelArtifactStateTester.Web
*UseModel
*LogOn
*ScreenCapture
*Navigate Views Object


;--------------------Actions - New----------------------
*Action New (Views Object)
*HandleDialog
 Caption = *NewView*

;****************CustomProcessSelectedItem*****************************

;--------check rule when criteria match-------------
*XpandFillForm
 Name = CustomProcessSelectedItem
*SaveAndClose
*XpandProcessRecord
 Name = CustomProcessSelectedItem
*HandleDialog
 Caption = *CustomProcessSelectedItemView*
*SaveAndClose

;---check that rule is not applied when criteria do not match----------
*Action New (Views Object)
*XpandFillForm
 Name = Default
*SaveAndClose
*XpandProcessRecord
 Name = Default
*HandleDialog
 Caption = *DefaultView*
*SaveAndClose

;-----------------CurrentObjectChanged--------------------
*Action New (Views Object)
*XpandFillForm
 Name = CurrentObjectChanged
*SaveAndClose
*XpandProcessRecord
 Name = Default
*HandleDialog
 Caption = *DefaultView*
*Action Previous Record
*HandleDialog
 Caption = *DefaultView*
*Action Previous Record
*HandleDialog
 Caption = *CurrentObjectChangedView*