;#IncludeFile .\Init.inc

*Navigate SequenceGenerator.Sequence Generator Object

*Action New(Sequence Generator Object)
*XpandFillForm
 Prefix = Nested

;Create 2 nested records
*Action Sequence Generator Nested Objects.New
*Action Save and Close
*Action Sequence Generator Nested Objects.New
*Action Save and Close

*Action Save and Close
*XpandProcessRecord 
 Prefix = Nested

;check sequences
*XpandProcessRecord Sequence Generator Nested Objects
 Sequence = 0
*SaveAndClose
*XpandProcessRecord Sequence Generator Nested Objects
 Sequence = 1
*SaveAndClose
 

