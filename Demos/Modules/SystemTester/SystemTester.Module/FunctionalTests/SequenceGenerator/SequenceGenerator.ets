#DropDB SystemTesterEasyTest
#Application SystemTester.Win
#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture

*Navigate SequenceGenerator.Sequence Generator Object
;----------------------Simple sequence---------------------------------
*Action New
*SaveAndClose
*CheckTable 
 Columns = Sequence
 Row[0] = 0
*Action New
*SaveAndClose
*XpandProcessRecord
 Sequence = 1


;-----------------------------Nested---------------------------------

*Navigate SequenceGenerator.Sequence Generator Object

*Action New
*XpandFillForm
 Name = Nested

;Create 2 nested records
*Action Sequence Generator Nested Objects.New
*SaveAndClose
*Action Sequence Generator Nested Objects.New
*SaveAndClose

*SaveAndClose
*XpandProcessRecord 
 Name = Nested

;check sequences
*XpandProcessRecord Sequence Generator Nested Objects
 Sequence = 0
*SaveAndClose
*XpandProcessRecord Sequence Generator Nested Objects
 Sequence = 1
*SaveAndClose

;-----------------------------Update---------------------------------
*Navigate SequenceGenerator.Sequence Generator Object

*Action New
*XpandFillForm
 Name = Update
*SaveAndClose

*XpandProcessRecord
 Name = Update
*Action UpdateSequence
*SaveAndClose
*XpandProcessRecord
 Name = Update	
 Sequence = 4


;--------------------------Release---------------------------------------------------------
*Action Delete
*HandleDialog
 Respond = Yes
*Navigate SequenceGenerator.Sequence Generator Object
*Action New
*XpandFillForm
 Name = Release
*SaveAndClose
*Navigate SequenceGenerator.Sequence Generator Object
*XpandProcessRecord
 Name = Release
 Sequence = 4

;------------------------------------DC with attribute-----------------------------
*Navigate SequenceGenerator.Sequence Generator DCObject

*Action New
*SaveAndClose
*SelectRecords
 Columns = Sequence
 Row = 0
*Action New
*SaveAndClose
*SelectRecords
 Columns = Sequence
 Row = 1
*Action Delete
*HandleDialog
 Respond = Yes

*Action New
*SaveAndClose
*SelectRecords
 Columns = Sequence
 Row = 1

