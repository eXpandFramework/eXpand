;#IncludeFile .\Init.inc
*Action New(Sequence Generator Object)
*SaveAndClose
*CheckTable 
 Columns = Prefix,Sequence
 Row[0] = '',0
*Action New(Sequence Generator Object)
*SaveAndClose
*XpandProcessRecord
 Prefix = ''
 Sequence = 1
*Navigate Sequence Generator Object