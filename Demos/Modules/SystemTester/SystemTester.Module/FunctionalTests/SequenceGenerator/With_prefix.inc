;#IncludeFile .\Init.inc
*Action New(Sequence Generator Object)
*XpandFillForm
 Prefix = A
*SaveAndClose
*CheckTable 
 Columns = Prefix,Sequence
 Row[0] = 'A',0
*Action New(Sequence Generator Object)
*XpandFillForm
 Prefix = A
*SaveAndClose
*CheckTable 
 Columns = Prefix,Sequence
 Row[3] = 'A',1