;#IncludeFile .\Init.inc
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

