;#IncludeFile .\Init.inc
*Navigate SequenceGenerator.Sequence Generator Object

*Action New(Sequence Generator Object)
*XpandFillForm
 Prefix = B
*SaveAndClose

*XpandProcessRecord
 Prefix = B
 Sequence = 0
*Action UpdateSequence
*SaveAndClose
*XpandProcessRecord
 Prefix = B
 Sequence = 1

*SaveAndClose